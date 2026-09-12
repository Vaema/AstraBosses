using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.Veronica;

public partial class VeronicaBoss : ModNPC
{
    #region Enumerations

    public enum State
    {
        ShootBolts,
        TrailCharge,
    }

    #endregion

    #region Fields and Properties

    public State CurrentState
    {
        get => (State)(int)NPC.ai[0];
        set => NPC.ai[0] = (int)value;
    }

    public ref float AttackTimer => ref NPC.ai[1];

    public Player Target => Main.player[NPC.target];

    #endregion

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
        NPC.width = 72;
        NPC.height = 94;

        NPC.lifeMax = 125000;
        NPC.damage = 125;
        NPC.defense = 25;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 50f;

        NPC.aiStyle = -1;
        AIType = -1;

        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.lavaImmune = true;
        NPC.dontTakeDamage = true;
        NPC.netAlways = true;
        NPC.value = Item.sellPrice(platinum: 2, gold: 50);

        NPC.HitSound = null;
        NPC.DeathSound = null;
    }

    #endregion

    #region AI

    public override void AI()
    {
        // Find the nearest target.
        NPC.TargetClosest();

        // Despawn if all remaining targets are dead.
        if (Target.dead || !Target.active)
        {
            NPC.TargetClosest();
            if (Target.dead || !Target.active)
            {
                NPC.velocity.Y--;
                if (Target.Distance(NPC.Center) > 300f)
                {
                    NPC.active = false;
                    NPC.netUpdate = true;
                }
                return;
            }
        }

        // Perform the following AI states.
        switch (CurrentState)
        {
            case State.ShootBolts:
                //ShootBolts();
                break;
        }

        // Increment the attack timer.
        AttackTimer++;
    }

    #endregion
}