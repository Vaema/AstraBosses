namespace AstraBosses.Core.Music;

public readonly record struct BossMusicClockSnapshot(
    string TrackKey,
    double ElapsedSeconds,
    long Beat,
    long Subdivision,
    double BeatProgress,
    double SubdivisionProgress,
    double TrackProgress,
    int Loop,
    bool IsPlaying,
    ulong SyncVersion)
{
    public bool HasTrack => !string.IsNullOrEmpty(TrackKey);

    public int BeatInBar(BossMusicTrack track) => (int)(Beat % track.BeatsPerBar);

    public int SubdivisionInBeat(BossMusicTrack track) => (int)(Subdivision % track.SubdivisionsPerBeat);

    public static BossMusicClockSnapshot Empty => new(
        string.Empty,
        0d,
        0L,
        0L,
        0d,
        0d,
        0d,
        0,
        false,
        0UL);
}
