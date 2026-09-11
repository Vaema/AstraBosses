namespace AstraBosses.Core.Music;

public enum BossMusicMetadataStatus
{
    Resolved,
    Unavailable,
    Invalid
}

public readonly record struct BossMusicMetadataResult(BossMusicMetadataStatus Status, BossMusicTrack? Track)
{
    public bool IsResolved => Status == BossMusicMetadataStatus.Resolved && Track is not null;

    public static BossMusicMetadataResult Unavailable => new(BossMusicMetadataStatus.Unavailable, null);
}

public interface IBossMusicRuntimeMetadataSource
{
    BossMusicMetadataResult Resolve(string trackKey);
}

public sealed class BossMusicMetadataResolver
{
    private readonly Dictionary<string, BossMusicMetadataResult> cache = new(StringComparer.OrdinalIgnoreCase);
    private IBossMusicRuntimeMetadataSource? source;

    public void SetSource(IBossMusicRuntimeMetadataSource? metadataSource)
    {
        source = metadataSource;
        cache.Clear();
    }

    public BossMusicMetadataResult Resolve(string trackKey)
    {
        if (string.IsNullOrWhiteSpace(trackKey))
            return BossMusicMetadataResult.Unavailable;

        if (cache.TryGetValue(trackKey, out BossMusicMetadataResult result))
            return result;

        result = source?.Resolve(trackKey) ?? BossMusicMetadataResult.Unavailable;
        if (result.Status == BossMusicMetadataStatus.Resolved && result.Track is null)
            result = new BossMusicMetadataResult(BossMusicMetadataStatus.Invalid, null);

        cache[trackKey] = result;
        return result;
    }

    public void Clear() => cache.Clear();
}
