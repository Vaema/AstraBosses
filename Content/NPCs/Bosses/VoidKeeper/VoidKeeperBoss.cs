using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.VoidKeeper;

public partial class VoidKeeperBoss : ModNPC
{
    #region Enumerations

    /// <summary>
    ///     The Void Keeper's AI states.
    /// </summary>
    public enum VKState
    {
        ShootBolts,
        TrailCharge,
    }

    #endregion Enumerations

    #region Fields and Properties

    /// <summary>
    ///     The current AI state of the Void Keeper boss.
    /// </summary>
    public VKState CurrentState
    {
        get => (VKState)(int)NPC.ai[0];
        set => NPC.ai[0] = (int)value;
    }

    /// <summary>
    ///     The AI attack timer for the Void Keeper boss.
    /// </summary>
    public ref float AttackTimer => ref NPC.ai[1];

    /// <summary>
    ///     Private backing field for <see cref="Myself"/>.
    /// </summary>
    private static NPC? myself;

    /// <summary>
    ///     A shorthand accessor for the Void Keeper boss. Returns null if not currently present.
    /// </summary>
    public static NPC? Myself
    {
        get
        {
            if (Main.gameMenu)
                return myself = null;

            if (myself is not null && !myself.active)
                return null;
            if (myself is not null && myself.type != ModContent.NPCType<VoidKeeperBoss>())
                return myself = null;

            return myself;
        }
        internal set => myself = value;
    }

    /// <summary>
    ///     The Void Keeper's current target.
    /// </summary>
    public Player Target => Main.player[NPC.target];

    #endregion Fields and Properties

    #region Initialization

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;

        NPCID.Sets.TrailCacheLength[Type] = 3;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        NPC.width = 120;
        NPC.height = 120;

        NPC.lifeMax = 7500;
        NPC.damage = 45;
        NPC.defense = 25;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 15f;

        NPC.aiStyle = -1;
        AIType = -1;

        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.lavaImmune = true;
        NPC.netAlways = true;
        NPC.value = Item.sellPrice(gold: 15);

        NPC.HitSound = SoundID.NPCHit4;
        NPC.DeathSound = SoundID.NPCDeath14;
    }

    #endregion Initialization

    #region AI

    public override void AI()
    {
        // Find the nearest target.
        NPC.TargetClosest();

        // Perform the following AI states.
        switch (CurrentState)
        {
            case VKState.ShootBolts:
                ShootBolts();
                break;
        }
    }

    #endregion AI
}