global using static System.MathF;
global using static Microsoft.Xna.Framework.MathHelper;

using AstraBosses.Core.Networking;
using Terraria.ModLoader;

namespace AstraBosses;

public class AstraBosses : Mod
{
    // Defer packet reading to a separate class.
    public override void HandlePacket(BinaryReader reader, int whoAmI) => PacketManager.ReceivePacket(reader);
}