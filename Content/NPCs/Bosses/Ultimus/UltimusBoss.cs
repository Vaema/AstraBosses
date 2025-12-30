using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.Ultimus;

public class UltimusBoss : ModNPC
{
    public ref float AttackTimer => ref NPC.ai[1];

    public ref float SpiritsSpawnedFlag => ref NPC.ai[3];

    public float LifeRatio => NPC.life / (float)NPC.lifeMax;

    public Player Target => Main.player[NPC.target];

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 1;
        NPCID.Sets.TrailCacheLength[Type] = 5;
        NPCID.Sets.TrailingMode[Type] = 0;
        NPCID.Sets.BossBestiaryPriority.Add(Type);
    }

    public override void SetDefaults()
    {
        NPC.width = 100;
        NPC.height = 100;

        NPC.SetLifeMaxByMode(450000, 500000, 525000);
        NPC.damage = 165;
        NPC.defense = 120;
        NPC.knockBackResist = 0f;
        NPC.npcSlots = 20f;
        NPC.alpha = 255;

        NPC.aiStyle = -1;
        AIType = -1;

        NPC.boss = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.lavaImmune = true;
        NPC.netAlways = true;

        NPC.HitSound = SoundID.NPCHit41;
        NPC.DeathSound = SoundID.NPCDeath10;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Ultimus");
        SceneEffectPriority = SceneEffectPriority.BossHigh;
    }

    public override void AI()
    {
        NPC.netUpdate = true;
        NPC.spriteDirection = NPC.direction;
        Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.35f);

        // Appear when summoned.
        if (NPC.alpha > 0)
            NPC.alpha -= 5;

        if (NPC.alpha <= 0)
            NPC.alpha = 0;

        // Find the nearest target.
        NPC.TargetClosest();

        // Despawn if all players are down or if it's daytime.
        if (Target.dead || !Target.active || Main.dayTime)
        {
            NPC.velocity.Y--;
            if (NPC.velocity.Y <= -50f)
                NPC.active = false;
        }

        // After appearing, follow a specific attack pattern.
        if (NPC.alpha <= 0)
        {
            AttackTimer++;
            
            if (AttackTimer >= 10 && AttackTimer <= 480)
                NPC.velocity = NPC.DirectionTo(Target.Center) * 9;

            if (AttackTimer > 480 && AttackTimer <= 540)
            {

            }
        }

        // Summon minions that orbit around.
        SpiritsSpawnedFlag++;
        if (SpiritsSpawnedFlag == 1f)
        {
            float radius = 1200f;
            float rotation = PI / 12f;
            for (int i = 0; i < 25; i++)
            {
                Vector2 position = NPC.Center + radius * (i * rotation).ToRotationVector2();
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)position.X, (int)position.Y, ModContent.NPCType<ArenaSpirit>(), NPC.whoAmI, NPC.whoAmI, i * rotation, radius, 0f, 255);
            }

            float radius2 = 120f;
            float rotation2 = PI / 4f;
            for (int j = 0; j < 15; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 position2 = NPC.Center + radius2 * (i * rotation2).ToRotationVector2();
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)position2.X, (int)position2.Y, ModContent.NPCType<AnarchicSpirit>(), NPC.whoAmI, NPC.whoAmI, i * rotation2, radius2, 0f, 255);
                }
            }
        }
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        scale = 1.5f;
        return null;
    }

    public override bool CheckActive() => false;
}