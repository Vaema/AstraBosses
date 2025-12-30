using AstraBosses.Core.Globals;
using Terraria;

namespace AstraBosses;

public static partial class Utilities
{
    public static AstraGlobalNPC Astra(this NPC npc) => npc.GetGlobalNPC<AstraGlobalNPC>();
}