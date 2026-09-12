using AstraBosses.Core.Configuration;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AstraBosses.Core.BehaviorOverrides;

public class NPCOverrideGlobalManager : GlobalNPC
{

    public override bool InstancePerEntity => true;
}