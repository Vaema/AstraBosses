//using AstraBosses.Core.Globals;

//using Luminance.Common.Utilities;

//using Microsoft.Xna.Framework;

//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace AstraBosses.Content.NPCs.Bosses.Cipher.Eyes;

//public class CipherRetinazerBoss : ModNPC
//{
//    public enum RetinazerState
//    {
//        Idle,
//        Charge,
//        Lasers,
//        Deathray,
//        PhaseTransition
//    }

//    public RetinazerState CurrentState
//    {
//        get => (RetinazerState)(int)NPC.ai[0];
//        set => NPC.ai[0] = (int)value;
//    }

//    public ref float AttackTimer => ref NPC.ai[1];

//    public float LifeRatio => NPC.life / (float)NPC.lifeMax;

//    public Player Target => Main.player[NPC.target];

//    public override void SetStaticDefaults()
//    {
        
//    }

//    public override void SetDefaults()
//    {
//        NPC.width = 204;
//        NPC.height = 220;

//        NPC.SetLifeMaxByMode(425000, 450000, 500000);
//        NPC.damage = 150;
//        NPC.defense = 100;
//        NPC.knockBackResist = 0f;
//        NPC.npcSlots = 5f;

//        NPC.aiStyle = -1;
//        AIType = -1;

//        NPC.boss = true;
//        NPC.noGravity = true;
//        NPC.noTileCollide = true;
//        NPC.lavaImmune = true;
//        NPC.netAlways = true;
//        NPC.Opacity = 0f;

//        NPC.HitSound = SoundID.NPCHit4;
//        NPC.DeathSound = SoundID.NPCDeath14;
//    }

//    public override void AI()
//    {
//        // Set the global "whoAmI" variable.
//        AstraGlobalNPC.CipherRetinazer = NPC.whoAmI;

//        // Find the nearest target.
//        if (NPC.target < 0 || NPC.target == Main.maxPlayers || Target.dead || !Target.active || Vector2.Distance(Target.Center, NPC.Center) > 4200)
//            NPC.TargetClosest();

//        // Check if the other "Cipher" bosses are alive.
//        bool spazmatismAlive = false;

//        if (AstraGlobalNPC.CipherSpazmatism != 1)
//        {
//            if (Main.npc[AstraGlobalNPC.CipherSpazmatism].active)
//            {
//                // Ensure both of the Twins share the same HP.
//                if (NPC.life > Main.npc[AstraGlobalNPC.CipherSpazmatism].life)
//                    NPC.life = Main.npc[AstraGlobalNPC.CipherSpazmatism].life;

//                spazmatismAlive = true;
//            }
//        }

//        // Delay attacking while Cipher Spazmatism is already attacking to prevent cheap hits on the player.
//        if (spazmatismAlive)
//        {

//        }
//    }
//}