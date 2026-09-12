namespace AstraBosses.Core.Music;

public sealed class BossMusicTimelineClip(
    double startBeat,
    double durationBeats,
    Func<BossMusicClockSnapshot, bool>? condition = null)
{
    public double StartBeat { get; } = startBeat >= 0d && double.IsFinite(startBeat) ? startBeat : throw new ArgumentOutOfRangeException(nameof(startBeat));

    public double DurationBeats { get; } = durationBeats > 0d && double.IsFinite(durationBeats) ? durationBeats : throw new ArgumentOutOfRangeException(nameof(durationBeats));

    public double EndBeat => StartBeat + DurationBeats;

    public Func<BossMusicClockSnapshot, bool>? Condition { get; } = condition;

    public bool IsActive(double beat, BossMusicClockSnapshot snapshot)
    {
        return beat >= StartBeat && beat < EndBeat && (Condition is null || Condition(snapshot));
    }
}

public readonly record struct BossMusicTimelineSample<T>(
    string TimelineId,
    double PositionBeats,
    double Progress,
    int Loop,
    IReadOnlyList<BossMusicTimelineClip> ActiveClips)
{
    public bool IsActive => ActiveClips.Count > 0;
}

public sealed class BossMusicTimeline<T>
{
    private readonly List<BossMusicTimelineClip> clips = [];

    public BossMusicTimeline(string id, double lengthBeats, bool loop = true)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("A timeline needs an ID.", nameof(id));
        if (!double.IsFinite(lengthBeats) || lengthBeats <= 0d)
            throw new ArgumentOutOfRangeException(nameof(lengthBeats));

        ID = id;
        LengthBeats = lengthBeats;
        Loops = loop;
    }

    public string ID { get; }

    public double LengthBeats { get; }

    public bool Loops { get; }

    public BossMusicTimeline<T> Add(BossMusicTimelineClip clip)
    {
        ArgumentNullException.ThrowIfNull(clip);
        if (clip.EndBeat > LengthBeats)
            throw new ArgumentOutOfRangeException(nameof(clip), "A clip must fit within the timeline length.");

        clips.Add(clip);
        clips.Sort((left, right) => left.StartBeat.CompareTo(right.StartBeat));
        return this;
    }

    public BossMusicTimelineSample<T> Sample(BossMusicClockSnapshot snapshot, BossMusicTrack track)
    {
        ArgumentNullException.ThrowIfNull(track);
        double absoluteBeat = snapshot.ElapsedSeconds / track.SecondsPerBeat;
        double timelineBeat = Loops ? Modulo(absoluteBeat, LengthBeats) : Math.Clamp(absoluteBeat, 0d, LengthBeats);
        int timelineLoop = Loops ? (int)Math.Floor(absoluteBeat / LengthBeats) : 0;
        List<BossMusicTimelineClip> active = clips.Where(clip => clip.IsActive(timelineBeat, snapshot)).ToList();
        double progress = LengthBeats <= 0d ? 1d : timelineBeat / LengthBeats;

        return new BossMusicTimelineSample<T>(ID, timelineBeat, progress, timelineLoop, active);
    }

    public IReadOnlyList<BossMusicTimelineClip> GetEnteredClips(
        BossMusicClockSnapshot previous,
        BossMusicClockSnapshot current,
        BossMusicTrack track)
    {
        ArgumentNullException.ThrowIfNull(track);
        double previousBeat = previous.ElapsedSeconds / track.SecondsPerBeat;
        double currentBeat = current.ElapsedSeconds / track.SecondsPerBeat;
        double previousAbsoluteBeat = AbsoluteBeat(previous, track);
        double currentAbsoluteBeat = AbsoluteBeat(current, track);

        if (double.IsFinite(previousAbsoluteBeat) && double.IsFinite(currentAbsoluteBeat) && currentAbsoluteBeat > previousAbsoluteBeat)
            return GetForwardEnteredClips(previousAbsoluteBeat, currentAbsoluteBeat, current, track);

        return GetLegacyWrappedClips(previousBeat, currentBeat, previous, current, track);
    }

    private IReadOnlyList<BossMusicTimelineClip> GetForwardEnteredClips(
        double previousBeat,
        double currentBeat,
        BossMusicClockSnapshot snapshot,
        BossMusicTrack track)
    {
        List<(double Beat, BossMusicTimelineClip Clip)> occurrences = [];

        foreach (BossMusicTimelineClip clip in clips)
        {
            if (!Loops)
            {
                if (clip.StartBeat > previousBeat && clip.StartBeat <= currentBeat)
                    AddOccurrence(occurrences, clip.StartBeat, clip, snapshot, track);

                continue;
            }

            double cycle = Math.Max(0d, Math.Ceiling((previousBeat - clip.StartBeat) / LengthBeats));
            double occurrenceBeat = clip.StartBeat + cycle * LengthBeats;

            while (occurrenceBeat <= currentBeat)
            {
                if (occurrenceBeat > previousBeat)
                    AddOccurrence(occurrences, occurrenceBeat, clip, snapshot, track);

                occurrenceBeat += LengthBeats;
            }
        }

        occurrences.Sort((left, right) => left.Beat.CompareTo(right.Beat));
        return occurrences.Select(occurrence => occurrence.Clip).ToList();
    }

    private IReadOnlyList<BossMusicTimelineClip> GetLegacyWrappedClips(
        double previousBeat,
        double currentBeat,
        BossMusicClockSnapshot previous,
        BossMusicClockSnapshot current,
        BossMusicTrack track)
    {
        bool wrapped = current.Loop > previous.Loop || currentBeat < previousBeat;
        if (!wrapped)
            return [];

        List<(double Beat, BossMusicTimelineClip Clip)> occurrences = [];
        double trackBeats = track.DurationSeconds / track.SecondsPerBeat;

        foreach (BossMusicTimelineClip clip in clips)
        {
            if (clip.StartBeat > previousBeat)
                AddOccurrence(occurrences, previous.Loop * trackBeats + clip.StartBeat, clip, current, track);

            if (clip.StartBeat <= currentBeat)
                AddOccurrence(occurrences, current.Loop * trackBeats + clip.StartBeat, clip, current, track);
        }

        occurrences.Sort((left, right) => left.Beat.CompareTo(right.Beat));
        return occurrences.Select(occurrence => occurrence.Clip).ToList();
    }

    private static void AddOccurrence(
        List<(double Beat, BossMusicTimelineClip Clip)> occurrences,
        double occurrenceBeat,
        BossMusicTimelineClip clip,
        BossMusicClockSnapshot snapshot,
        BossMusicTrack track)
    {
        if (!double.IsFinite(occurrenceBeat))
            return;

        BossMusicClockSnapshot occurrenceSnapshot = SnapshotAtBeat(snapshot, occurrenceBeat, track);
        if (clip.Condition is null || clip.Condition(occurrenceSnapshot))
            occurrences.Add((occurrenceBeat, clip));
    }

    private static double AbsoluteBeat(BossMusicClockSnapshot snapshot, BossMusicTrack track)
    {
        return snapshot.Loop * (track.DurationSeconds / track.SecondsPerBeat)
            + snapshot.ElapsedSeconds / track.SecondsPerBeat;
    }

    private static BossMusicClockSnapshot SnapshotAtBeat(
        BossMusicClockSnapshot source,
        double absoluteBeat,
        BossMusicTrack track)
    {
        double trackBeats = track.DurationSeconds / track.SecondsPerBeat;
        double trackLoopValue = Math.Floor(absoluteBeat / trackBeats);
        int trackLoop = trackLoopValue >= int.MaxValue
            ? int.MaxValue
            : trackLoopValue <= 0d
                ? 0
                : (int)trackLoopValue;
        double elapsed = (absoluteBeat - trackLoopValue * trackBeats) * track.SecondsPerBeat;

        if (elapsed >= track.DurationSeconds)
        {
            elapsed = 0d;
            trackLoop = trackLoop == int.MaxValue ? int.MaxValue : trackLoop + 1;
        }

        double beatPosition = elapsed / track.SecondsPerBeat;
        double subdivisionPosition = elapsed / track.SecondsPerSubdivision;
        long beat = (long)Math.Floor(beatPosition);
        long subdivision = (long)Math.Floor(subdivisionPosition);

        return new BossMusicClockSnapshot(
            source.TrackKey,
            elapsed,
            beat,
            subdivision,
            beatPosition - beat,
            subdivisionPosition - subdivision,
            elapsed / track.DurationSeconds,
            trackLoop,
            source.IsPlaying,
            source.SyncVersion);
    }

    private static double Modulo(double value, double modulus)
    {
        double result = value % modulus;
        return result < 0d ? result + modulus : result;
    }
}
