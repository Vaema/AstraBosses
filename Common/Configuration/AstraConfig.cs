using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AstraBosses.Common.Configuration;

public class AstraConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(true)]
    public static bool BossReworksEnabled { get; set; }
}