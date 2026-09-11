using AstraBosses.Core.Music;
using AstraBosses.Core.Systems;

using Terraria.ModLoader;

namespace AstraBosses.Core.Networking;

public sealed class BossMusicSyncPacket : Packet
{
    public override bool ResendFromServer => false;

    public override void Write(ModPacket packet, params object[] context)
    {
        BossMusicClockSnapshot snapshot = (BossMusicClockSnapshot)context[0];
        BossMusicTrack? track = context.Length > 1 ? context[1] as BossMusicTrack : null;

        packet.Write(snapshot.SyncVersion);
        packet.Write(snapshot.IsPlaying);
        packet.Write(track is not null);
        packet.Write(snapshot.TrackKey);
        packet.Write(snapshot.ElapsedSeconds);
        packet.Write(snapshot.Loop);

        if (track is not null)
        {
            packet.Write(track.BeatsPerMinute);
            packet.Write(track.DurationSeconds);
            packet.Write(track.BeatsPerBar);
            packet.Write(track.SubdivisionsPerBeat);
        }
    }

    public override void Read(BinaryReader reader)
    {
        ulong syncVersion = reader.ReadUInt64();
        bool isPlaying = reader.ReadBoolean();
        bool hasTrack = reader.ReadBoolean();
        string trackKey = reader.ReadString();
        double elapsedSeconds = reader.ReadDouble();
        int loop = reader.ReadInt32();

        BossMusicTrack? track = null;
        if (hasTrack)
        {
            double beatsPerMinute = reader.ReadDouble();
            double durationSeconds = reader.ReadDouble();
            int beatsPerBar = reader.ReadInt32();
            int subdivisionsPerBeat = reader.ReadInt32();

            try
            {
                track = new BossMusicTrack(trackKey, beatsPerMinute, durationSeconds, beatsPerBar, subdivisionsPerBeat);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        BossMusicClockSnapshot snapshot = new(
            hasTrack ? trackKey : string.Empty,
            elapsedSeconds,
            0L,
            0L,
            0d,
            0d,
            0d,
            loop,
            isPlaying,
            syncVersion);

        BossMusicSyncSystem system = BossMusicSyncSystem.Instance;
        if (track is not null)
            system.RegisterTrack(track);
        system.ApplySnapshot(snapshot);
    }
}
