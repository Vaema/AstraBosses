namespace AstraBosses.Core.Music;

public interface IBossMusicMetadataProvider
{
    bool TryGetTrack(string trackKey, out BossMusicTrack track);
}

public sealed class BossMusicMetadataRegistry : IBossMusicMetadataProvider
{
    private readonly Dictionary<string, BossMusicTrack> tracks = new(StringComparer.OrdinalIgnoreCase);

    public void Register(BossMusicTrack track)
    {
        ArgumentNullException.ThrowIfNull(track);
        tracks[track.Key] = track;
    }

    public bool TryGetTrack(string trackKey, out BossMusicTrack track) => tracks.TryGetValue(trackKey, out track!);

    public void Clear() => tracks.Clear();
}
