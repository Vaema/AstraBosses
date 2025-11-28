using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AstraBosses.Core.Configuration;

public class AstraServerConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(true)]
    public static bool BossReworksEnabled { get; set; }
}