using Terraria.ModLoader;

namespace AstraBosses;

public class AstraBosses : Mod
{
    /// <summary>
    /// The instance of this mod.
    /// </summary>
    public static AstraBosses Instance
    {
        get;
        private set;
    }

    public override void Load() => Instance = this;

    public override void Unload() => Instance = null;
}