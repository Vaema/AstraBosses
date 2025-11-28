global using static AstraBosses.Core.Utilities.Utilities;

using AstraBosses.Core.Networking;
using Luminance.Core.ModCalls;
using Terraria.ModLoader;

namespace AstraBosses;

public class AstraBosses : Mod
{
    // Defer packet reading to a separate class.
    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);

    // Use Luminance's mod call system for cross-mod compatibility.
    public override object Call(params object[] args) => ModCallManager.ProcessAllModCalls(this, args);
}