using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AstraBosses.Content.NPCs.Bosses.Ultimus.Projectiles;

public class SpiritualStar : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.StarWrath;

    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.aiStyle = ProjAIStyleID.Beam;
        Projectile.light = 0.25f;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
    }
}