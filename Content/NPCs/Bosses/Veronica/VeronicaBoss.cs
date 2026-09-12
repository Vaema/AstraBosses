using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AstraBosses.Core.Music;
using AstraBosses.Core.Systems;
using AstraBosses.Content.NPCs.Bosses.Veronica.Projectiles;

namespace AstraBosses.Content.NPCs.Bosses.Veronica;

public partial class VeronicaBoss : ModNPC
{
    #region Enumerations

    public enum State
    {
        Idle,
        SwordSwinging,
        BulletHell,
        Dashing,
        RadialBurst,
        Finished
    }

    #endregion

    #region Fields and Properties

    public State CurrentState
    {
        get => (State)(int)NPC.ai[0];
        set => NPC.ai[0] = (int)value;
    }

    public ref float AttackTimer => ref NPC.ai[1];

    public ref float TimelineTimer => ref NPC.ai[2];

    public ref float AttackVariant => ref NPC.ai[3];

    public Player Target => Main.player[NPC.target];

    private const int TicksPerSecond = 60;
    private const int TimelineLength = 30 * TicksPerSecond;
    private const int SwordOpening = 120;
    private const int SwordOpeningEnd = 360;
    private const int SwordSecond = 420;
    private const int SwordSecondEnd = 720;
    private const int BulletHellOpening = 780;
    private const int BulletHellEnd = 1080;
    private const int DashOpening = 1140;
    private const int DashEnd = 1260;
    private const int RadialBurstOpening = 1320;
    private const int RadialBurstEnd = 1500;
    private const int SwordFinal = 1560;
    private const int SwordFinalEnd = TimelineLength;
    private const string MusicTrackKey = "Veronica";
    private static readonly BossMusicTrack MusicTrack = new(MusicTrackKey, 120d, 180d);

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
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Veronica");

        BossMusicSyncSystem.Instance.RegisterTrack(MusicTrack);
    }

    #endregion

    #region AI

    public override void AI()
    {
        NPC.TargetClosest();

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

        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            BossMusicSyncSystem.Instance.TryStart(MusicTrackKey, restart: false);

            State scheduledState = GetScheduledState((int)TimelineTimer);
            if (CurrentState != scheduledState)
                SetState(scheduledState);
        }

        switch (CurrentState)
        {
            case State.Idle:
                HoverNearTarget();
                break;
            case State.SwordSwinging:
                SwordSwinging();
                break;
            case State.BulletHell:
                BulletHell();
                break;
            case State.Dashing:
                Dashing();
                break;
            case State.RadialBurst:
                RadialBurst();
                break;
            case State.Finished:
                NPC.velocity *= 0.96f;
                break;
        }

        AttackTimer++;
        if (TimelineTimer < TimelineLength)
            TimelineTimer++;
    }

    public override void OnKill()
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
            BossMusicSyncSystem.Instance.Stop();
    }

    private State GetScheduledState(int timelineTick)
    {
        if (timelineTick >= SwordOpening && timelineTick < SwordOpeningEnd ||
            timelineTick >= SwordSecond && timelineTick < SwordSecondEnd ||
            timelineTick >= SwordFinal && timelineTick < SwordFinalEnd)
            return State.SwordSwinging;

        if (timelineTick >= BulletHellOpening && timelineTick < BulletHellEnd)
            return State.BulletHell;

        if (timelineTick >= DashOpening && timelineTick < DashEnd)
            return State.Dashing;

        if (timelineTick >= RadialBurstOpening && timelineTick < RadialBurstEnd)
            return State.RadialBurst;

        return timelineTick >= TimelineLength ? State.Finished : State.Idle;
    }

    private void SetState(State state)
    {
        CurrentState = state;
        AttackTimer = 0f;
        AttackVariant = 0f;
        NPC.netUpdate = true;
    }

    private void HoverNearTarget()
    {
        Vector2 desiredPosition = Target.Center - Vector2.UnitY * 260f;
        Vector2 offset = desiredPosition - NPC.Center;
        NPC.velocity = Vector2.Lerp(NPC.velocity, offset.SafeNormalize(Vector2.Zero) * Math.Min(offset.Length() * 0.08f, 12f), 0.12f);
        NPC.rotation = NPC.velocity.X * 0.02f;
    }

    private void SwordSwinging()
    {
        HoverNearTarget();

        const int ChargeDuration = 42;
        const int PauseDuration = 18;
        const int SwingDuration = 20;
        const int RecoveryDuration = 54;
        const int SwingCycle = ChargeDuration + PauseDuration + SwingDuration + RecoveryDuration;

        int cycleTimer = (int)AttackTimer % SwingCycle;
        if (cycleTimer == ChargeDuration + PauseDuration && Main.netMode != NetmodeID.MultiplayerClient)
        {
            Vector2 direction = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX * NPC.direction);
            int damage = NPC.damage / 2;
            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center + direction * 42f,
                direction * 9f,
                ModContent.ProjectileType<VeronicaSword>(),
                damage,
                6f,
                Main.myPlayer,
                NPC.whoAmI,
                SwingDuration);
        }

        NPC.rotation = cycleTimer < ChargeDuration ? NPC.direction * 0.18f : 0f;
    }

    private void BulletHell()
    {
        HoverNearTarget();

        if (Main.netMode == NetmodeID.MultiplayerClient || AttackTimer < 18f || (AttackTimer - 18f) % 48f != 0f)
            return;

        Vector2 aim = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
        for (int index = -3; index <= 3; index++)
        {
            float spread = index * 0.14f;
            Vector2 velocity = aim.RotatedBy(spread) * 8f;
            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<VeronicaBullet>(),
                NPC.damage / 4,
                2f,
                Main.myPlayer,
                0f,
                1f);
        }
    }

    private void Dashing()
    {
        if (AttackTimer < 36f)
        {
            HoverNearTarget();
            return;
        }

        if (AttackTimer == 36f)
        {
            Vector2 direction = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
            NPC.velocity = direction * 18f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.netUpdate = true;
        }
        else if (AttackTimer < 78f)
            NPC.velocity *= 0.995f;
        else
            NPC.velocity *= 0.9f;

        NPC.rotation = NPC.velocity.ToRotation() + PiOver2;
    }

    private void RadialBurst()
    {
        HoverNearTarget();

        if (Main.netMode == NetmodeID.MultiplayerClient || (AttackTimer != 18f && AttackTimer != 96f))
            return;

        const int ProjectileCount = 16;
        float startingAngle = AttackTimer == 18f ? 0f : Pi / ProjectileCount;
        for (int index = 0; index < ProjectileCount; index++)
        {
            Vector2 velocity = Vector2.UnitX.RotatedBy(startingAngle + TwoPi * index / ProjectileCount) * 7f;
            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<VeronicaBullet>(),
                NPC.damage / 4,
                2f,
                Main.myPlayer,
                0f,
                0f);
        }
    }

    #endregion
}