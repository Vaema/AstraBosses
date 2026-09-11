namespace AstraBosses.Core.Music;

public sealed record BossMusicTrack
{
    public BossMusicTrack(
        string key,
        double beatsPerMinute,
        double durationSeconds,
        int beatsPerBar = 4,
        int subdivisionsPerBeat = 1)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("A music track must have a non-empty key.", nameof(key));
        if (!double.IsFinite(beatsPerMinute) || beatsPerMinute <= 0d)
            throw new ArgumentOutOfRangeException(nameof(beatsPerMinute), "BPM must be finite and greater than zero.");
        if (!double.IsFinite(durationSeconds) || durationSeconds <= 0d)
            throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration must be finite and greater than zero.");
        if (beatsPerBar <= 0)
            throw new ArgumentOutOfRangeException(nameof(beatsPerBar), "Beats per bar must be greater than zero.");
        if (subdivisionsPerBeat <= 0)
            throw new ArgumentOutOfRangeException(nameof(subdivisionsPerBeat), "Subdivisions per beat must be greater than zero.");

        Key = key;
        BeatsPerMinute = beatsPerMinute;
        DurationSeconds = durationSeconds;
        BeatsPerBar = beatsPerBar;
        SubdivisionsPerBeat = subdivisionsPerBeat;
    }

    public string Key { get; }

    public double BeatsPerMinute { get; }

    public double DurationSeconds { get; }

    public int BeatsPerBar { get; }

    public int SubdivisionsPerBeat { get; }

    public double SecondsPerBeat => 60d / BeatsPerMinute;

    public double SecondsPerSubdivision => SecondsPerBeat / SubdivisionsPerBeat;

    public long TotalBeats => (long)Math.Ceiling(DurationSeconds / SecondsPerBeat);
}
