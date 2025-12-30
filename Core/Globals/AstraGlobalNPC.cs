using AstraBosses.Core.Configuration;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Core.Globals;

public partial class AstraGlobalNPC : GlobalNPC
{
    #region Fields and Properties

    public float[] ExtraAI = new float[ExtraAISlots];

    public const int ExtraAISlots = 100;

    internal bool[] HasAIBeenUsed = new bool[ExtraAISlots];

    public int? TotalPlayersAtStart;

    public static int CipherRetinazer = -1;

    public static int CipherSpazmatism = -1;

    public override bool InstancePerEntity => true;

    #endregion Fields and Properties

    #region Initialization

    public override void SetDefaults(NPC npc)
    {
        for (int i = 0; i < ExtraAI.Length; i++)
            ExtraAI[i] = 0f;

        // Modify the health of various vanilla bosses.
        if (AstraServerConfig.BossReworksEnabled)
        {
            switch (npc.type)
            {
                case NPCID.CultistBoss:
                    npc.lifeMax = 115000;
                    break;
            }
        }

        // Thanks, Redigit.
        if (npc.type == NPCID.WallofFleshEye)
            npc.netAlways = true;
    }

    #endregion Initialization

    #region AI

    public override bool PreAI(NPC npc)
    {
        // Initialize the amount of players the NPC had when it spawned.
        if (!npc.Astra().TotalPlayersAtStart.HasValue)
        {
            int activePlayerCount = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                    activePlayerCount++;
            }

            npc.Astra().TotalPlayersAtStart = activePlayerCount;
            npc.netUpdate = true;
        }

        // Disable networking offset effects.
        npc.netOffset = Vector2.Zero;

        return base.PreAI(npc);
    }

    public override void PostAI(NPC npc)
    {
        for (int i = 0; i < ExtraAI.Length; i++)
        {
            if (ExtraAI[i] != 0f)
                HasAIBeenUsed[i] = true;
        }
    }

    #endregion AI

    #region Hit Effects

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        if (!AstraServerConfig.BossReworksEnabled)
            return;

        HitEffectsEvent?.Invoke(npc, ref hit);
    }

    #endregion Hit Effects
}