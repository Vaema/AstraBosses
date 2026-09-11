using Terraria;
using Terraria.ModLoader;

namespace AstraBosses.Core.Globals;

public partial class AstraGlobalNPC : GlobalNPC
{
    #region Fields and Properties

    public float[] ExtraAI = new float[ExtraAISlots];

    public const int ExtraAISlots = 10;

    internal bool[] HasAIBeenUsed = new bool[ExtraAISlots];

    public int? TotalPlayersAtStart;

    public override bool InstancePerEntity => true;

    #endregion

    #region Initialization

    public override void SetDefaults(NPC npc)
    {
        for (int i = 0; i < ExtraAI.Length; i++)
            ExtraAI[i] = 0f;
    }

    #endregion

    #region AI

    public override void PostAI(NPC npc)
    {
        for (int i = 0; i < ExtraAI.Length; i++)
        {
            if (ExtraAI[i] != 0f)
                HasAIBeenUsed[i] = true;
        }
    }

    #endregion
}