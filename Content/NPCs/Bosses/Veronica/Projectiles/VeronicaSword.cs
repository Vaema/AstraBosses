using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.Veronica.Projectiles;

public class VeronicaSword : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_502";

    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 72;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
        AIType = -1;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 30;
    }

    public override void AI()
    {
        int ownerIndex = (int)Projectile.ai[0];
        if (ownerIndex < 0 || ownerIndex >= Main.maxNPCs || !Main.npc[ownerIndex].active)
        {
            Projectile.Kill();
            return;
        }

        NPC owner = Main.npc[ownerIndex];
        int swingDuration = Math.Max(1, (int)Projectile.ai[1]);
        int elapsed = (int)Projectile.localAI[0]++;
        if (elapsed == 0)
            Projectile.timeLeft = swingDuration;

        float progress = Clamp(elapsed / (float)swingDuration, 0f, 1f);
        float angle = Lerp(-1.25f, 1.25f, progress) * owner.spriteDirection;
        Vector2 slashDirection = Vector2.UnitX.RotatedBy(angle);
        Projectile.Center = owner.Center + slashDirection * 78f;
        Projectile.velocity = Vector2.Zero;
        Projectile.rotation = slashDirection.ToRotation() + PiOver2;
    }
}