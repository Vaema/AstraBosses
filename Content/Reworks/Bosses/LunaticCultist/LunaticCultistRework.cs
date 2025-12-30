//using AstraBosses.Core.BehaviorOverrides;

//using Microsoft.Xna.Framework;

//using Terraria;
//using Terraria.ID;

//namespace AstraBosses.Content.Reworks.Bosses.LunaticCultist;

//public class LunaticCultistRework : NPCBehaviorOverride
//{
//    public override int NPCOverrideID => NPCID.CultistBoss;

//    #region Enumerations

//    public enum CultistFrame
//    {
//        AbsorbEffect,
//        Hover,
//        RaiseArmsUp,
//        HoldArmsOut,
//        Laugh,
//    }

//    public enum CultistState
//    {
//        // Summon animation.
//        SummonAnimation,

//        // General attacks.
//        FireballBarrage,
//        LightningAndVortexes,
//        IceMistAndWaves,
//        NebulaOrbsAndLaser,
//        HomingStardust,
//        Ritual,

//        // Desperation attacks.
//        LaserSpinningBulletHell,
//        LunarWrath
//    }

//    #endregion Enumerations

//    #region Fields and Properties

//    public CultistState CurrentState => (CultistState)(int)NPC.ai[0];

//    public ref float AttackTimer => ref NPC.ai[1];

//    public Player Target => Main.player[NPC.target];

//    public const float Phase2LifeRatio = 0.7f;

//    public const float TransitionAnimationTime = 90f;

//    public static readonly Color[] PillarsPallete =
//    [
//        // Solar.
//        new(255, 93, 30),

//        // Nebula.
//        new(232, 76, 183),

//        // Vortex.
//        new(0, 170, 221),

//        // Stardust.
//        new(0, 170, 221)
//    ];

//    #endregion Fields and Properties

//    #region AI

//    public override void AI()
//    {
//        // Find the nearest target.
//        NPC.TargetClosest();

//        // Despawn if all remaining targets are invalid.
//        if (!Main.player.IndexInRange(NPC.target) || Target.dead || !Target.active)
//        {
//            NPC.TargetClosest();
//            if (!Main.player.IndexInRange(NPC.target) || Target.dead || !Target.active)
//            {
//                Despawn();
//                return;
//            }
//        }

//        // Universally disable contact damage.
//        NPC.damage = 0;

//        // Reset not taking damage, just to be safe.
//        NPC.dontTakeDamage = false;

//        // Switch between AI states.
//        switch (CurrentState)
//        {
//            case CultistState.SummonAnimation:
//                SummonAnimation();
//                break;
//        }

//        // Increment the AI attack timer.
//        AttackTimer++;
//    }

//    public void SummonAnimation()
//    {

//    }

//    public void Despawn()
//    {
//        NPC.velocity = Vector2.Zero;
//        NPC.dontTakeDamage = true;

//        if (NPC.timeLeft > 25)
//            NPC.timeLeft = 25;

//        NPC.alpha = Utils.Clamp(NPC.alpha + 40, 0, 255);
//        if (NPC.alpha >= 255)
//        {
//            NPC.active = false;
//            NPC.netUpdate = true;
//        }
//    }

//    #endregion AI
//}