//using Luminance.Common.Utilities;

//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace AstraBosses.Content.NPCs.Bosses.Cipher.Eyes;

//public class CipherSpazmatismBoss : ModNPC
//{
//    public enum SpazmatismState
//    {
//        Idle,
//        Charge,
//        Lasers,
//        Deathray,
//        PhaseTransition
//    }

//    public SpazmatismState CurrentState
//    {
//        get => (SpazmatismState)(int)NPC.ai[0];
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
//}