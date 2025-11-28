using Terraria;

namespace AstraBosses.Core.Globals;

public partial class AstraGlobalNPC
{
    public delegate void HitEffectsDelegate(NPC npc, ref NPC.HitInfo hit);

    public static event HitEffectsDelegate? HitEffectsEvent;
}