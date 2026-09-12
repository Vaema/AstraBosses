using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.Veronica.Projectiles;

public class VeronicaBullet : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_466";

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 180;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.velocity *= 1.006f;

        if (Projectile.ai[1] > 0f && Projectile.localAI[0]++ < 45f)
        {
            int targetIndex = Player.FindClosest(Projectile.Center, 1, 1);
            if (targetIndex >= 0 && Main.player[targetIndex].active && !Main.player[targetIndex].dead)
            {
                Vector2 desiredVelocity = (Main.player[targetIndex].Center - Projectile.Center).SafeNormalize(Projectile.velocity) * Projectile.velocity.Length();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.018f);
            }
        }
    }
}
