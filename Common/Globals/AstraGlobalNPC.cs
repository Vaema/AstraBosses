using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Common.Globals;

public class AstraGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;

    public override void SetDefaults(NPC npc)
    {
        switch (npc.type)
        {
            case NPCID.CultistBoss:
                npc.lifeMax = 100000;
                break;
        }
    }
}