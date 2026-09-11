namespace AstraBosses.Core.Music;

public readonly record struct BossMusicDiagnostics(
    string TrackKey,
    bool IsPlaying,
    bool IsAuthoritative,
    double ElapsedSeconds,
    long Beat,
    long Subdivision,
    int Loop,
    double TrackProgress,
    ulong SyncVersion,
    BossMusicMetadataStatus MetadataStatus);
