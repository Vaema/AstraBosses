using AstraBosses.Content.NPCs.Bosses.Ultimus.Projectiles;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.Ultimus;

public class AnarchicSpirit : ModNPC
{
    public ref float Timer => ref NPC.ai[1];

    public ref float AttackTimer => ref NPC.ai[3];

    public override string Texture => "AstraBosses/Content/NPCs/Bosses/Ultimus/UltimusBoss";

    public override void SetDefaults()
    {
        NPC.width = 56;
        NPC.height = 56;

        NPC.lifeMax = 2500;
        NPC.damage = 125;
        NPC.defense = 0;
        NPC.npcSlots = 0f;
        NPC.alpha = 255;

        NPC.aiStyle = -1;
        AIType = -1;

        NPC.friendly = false;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.dontTakeDamage = true;
        NPC.dontCountMe = true;
        NPC.chaseable = false;
        NPC.netAlways = true;
    }

    public override void AI()
    {
        NPC.TargetClosest();

        NPC.alpha -= 5;
        if (NPC.alpha >= 255)
            NPC.alpha -= 5;

        if (NPC.alpha <= 0)
            NPC.alpha = 0;

        Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.5f);

        int owner = (int)NPC.ai[0];
        if (owner < 0 || owner >= 200 || !Main.npc[owner].active || Main.npc[owner].type != ModContent.NPCType<UltimusBoss>())
        {
            NPC.active = false;
            return;
        }

        AttackTimer++;
        if (AttackTimer == 60 || AttackTimer == 120 || AttackTimer == 180 || AttackTimer == 240 || AttackTimer == 300 || AttackTimer == 360)
        {
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.position, NPC.DirectionTo(Main.player[NPC.target].Center) * 18,
                ModContent.ProjectileType<SpiritualStar>(), 125, 0f, Main.myPlayer);
        }

        Timer += 0.03f;
        NPC.Center = Main.npc[owner].Center + NPC.ai[2] * NPC.ai[1].ToRotationVector2();
    }

    public override bool CheckActive() => false;
}