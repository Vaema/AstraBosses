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
        bool wrapped = current.Loop > previous.Loop || currentBeat < previousBeat;
        List<BossMusicTimelineClip> entered = [];

        foreach (BossMusicTimelineClip clip in clips)
        {
            bool enteredCurrent = clip.StartBeat > previousBeat && clip.StartBeat <= currentBeat;
            if (wrapped)
                enteredCurrent = clip.StartBeat > previousBeat || clip.StartBeat <= currentBeat;

            if (enteredCurrent && (clip.Condition is null || clip.Condition(current)))
                entered.Add(clip);
        }

        return entered;
    }

    private static double Modulo(double value, double modulus)
    {
        double result = value % modulus;
        return result < 0d ? result + modulus : result;
    }
}
