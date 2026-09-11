using AstraBosses.Core.Networking;
using AstraBosses.Core.Music;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Core.Systems;

public class BossMusicSyncSystem : ModSystem
{
    public const double TicksPerSecond = 60d;

    private BossMusicTrack? activeTrack;
    private double elapsedSeconds;
    private int loop;
    private bool isPlaying;
    private ulong syncVersion;
    private ulong lastBroadcastVersion;
    private int broadcastTimer;

    public static BossMusicSyncSystem Instance => ModContent.GetInstance<BossMusicSyncSystem>();

    public BossMusicMetadataRegistry Metadata { get; } = new();

    public BossMusicMetadataResolver MetadataResolver { get; } = new();

    public BossMusicClockSnapshot Snapshot => CreateSnapshot();

    public BossMusicDiagnostics Diagnostics
    {
        get
        {
            BossMusicClockSnapshot snapshot = Snapshot;
            BossMusicMetadataStatus metadataStatus = activeTrack is null
                ? BossMusicMetadataStatus.Unavailable
                : BossMusicMetadataStatus.Resolved;
            return new BossMusicDiagnostics(
                snapshot.TrackKey,
                snapshot.IsPlaying,
                IsAuthoritative,
                snapshot.ElapsedSeconds,
                snapshot.Beat,
                snapshot.Subdivision,
                snapshot.Loop,
                snapshot.TrackProgress,
                snapshot.SyncVersion,
                metadataStatus);
        }
    }

    public bool IsAuthoritative => Main.netMode != NetmodeID.MultiplayerClient;

    public override void OnWorldLoad() => Reset();

    public override void OnWorldUnload() => Reset();

    public override void PostUpdateEverything()
    {
        if (isPlaying && activeTrack is not null)
            Advance(1d / TicksPerSecond);

        if (Main.netMode == NetmodeID.Server && (++broadcastTimer >= 15 && activeTrack is not null || lastBroadcastVersion != syncVersion))
        {
            broadcastTimer = 0;
            lastBroadcastVersion = syncVersion;
            PacketManager.SendPacket<BossMusicSyncPacket>(Snapshot, activeTrack);
        }
    }

    public void RegisterTrack(BossMusicTrack track) => Metadata.Register(track);

    public bool TryStart(string trackKey, double elapsed = 0d, bool restart = true)
    {
        if (!IsAuthoritative || !Metadata.TryGetTrack(trackKey, out BossMusicTrack? track))
            return false;

        if (!restart && activeTrack?.Key.Equals(trackKey, StringComparison.OrdinalIgnoreCase) == true)
        {
            isPlaying = true;
            return true;
        }

        activeTrack = track;
        this.elapsedSeconds = NormalizeElapsed(track, elapsed, out loop);
        isPlaying = true;
        syncVersion++;
        return true;
    }

    public bool TryRegisterRuntimeTrack(string trackKey)
    {
        BossMusicMetadataResult result = MetadataResolver.Resolve(trackKey);
        if (!result.IsResolved)
            return false;

        Metadata.Register(result.Track!);
        return true;
    }

    public void Pause()
    {
        if (!IsAuthoritative)
            return;

        isPlaying = false;
        syncVersion++;
    }

    public void Resume()
    {
        if (!IsAuthoritative || activeTrack is null)
            return;

        isPlaying = true;
        syncVersion++;
    }

    public void Stop()
    {
        if (!IsAuthoritative)
            return;

        activeTrack = null;
        elapsedSeconds = 0d;
        loop = 0;
        isPlaying = false;
        syncVersion++;
    }

    public void Seek(double elapsed)
    {
        if (!IsAuthoritative || activeTrack is null || !double.IsFinite(elapsed))
            return;

        elapsedSeconds = NormalizeElapsed(activeTrack, elapsed, out loop);
        syncVersion++;
    }

    public void ApplySnapshot(BossMusicClockSnapshot snapshot)
    {
        if (IsAuthoritative || snapshot.SyncVersion < syncVersion)
            return;

        if (!snapshot.HasTrack || !Metadata.TryGetTrack(snapshot.TrackKey, out BossMusicTrack? track))
        {
            activeTrack = null;
            elapsedSeconds = 0d;
            loop = 0;
            isPlaying = false;
            syncVersion = snapshot.SyncVersion;
            return;
        }

        activeTrack = track;
        elapsedSeconds = Math.Clamp(snapshot.ElapsedSeconds, 0d, track.DurationSeconds);
        loop = Math.Max(0, snapshot.Loop);
        isPlaying = snapshot.IsPlaying;
        syncVersion = snapshot.SyncVersion;
    }

    public void Reset()
    {
        activeTrack = null;
        elapsedSeconds = 0d;
        loop = 0;
        isPlaying = false;
        syncVersion = 0UL;
        lastBroadcastVersion = 0UL;
        broadcastTimer = 0;
        Metadata.Clear();
        MetadataResolver.Clear();
    }

    private void Advance(double seconds)
    {
        if (activeTrack is null || !double.IsFinite(seconds) || seconds <= 0d)
            return;

        elapsedSeconds += seconds;
        while (elapsedSeconds >= activeTrack.DurationSeconds)
        {
            elapsedSeconds -= activeTrack.DurationSeconds;
            loop++;
        }
    }

    private BossMusicClockSnapshot CreateSnapshot()
    {
        if (activeTrack is null)
            return BossMusicClockSnapshot.Empty with { SyncVersion = syncVersion };

        double beatPosition = elapsedSeconds / activeTrack.SecondsPerBeat;
        double subdivisionPosition = elapsedSeconds / activeTrack.SecondsPerSubdivision;
        long beat = (long)Math.Floor(beatPosition);
        long subdivision = (long)Math.Floor(subdivisionPosition);

        return new BossMusicClockSnapshot(
            activeTrack.Key,
            elapsedSeconds,
            beat,
            subdivision,
            beatPosition - beat,
            subdivisionPosition - subdivision,
            elapsedSeconds / activeTrack.DurationSeconds,
            loop,
            isPlaying,
            syncVersion);
    }

    private static double NormalizeElapsed(BossMusicTrack track, double elapsed, out int loop)
    {
        double normalized = Math.Max(0d, elapsed);
        loop = (int)Math.Floor(normalized / track.DurationSeconds);
        return normalized - loop * track.DurationSeconds;
    }
}
