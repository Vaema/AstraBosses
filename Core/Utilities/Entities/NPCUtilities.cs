using Microsoft.Xna.Framework;
using Terraria;

namespace AstraBosses;

public static partial class Utilities
{
    /// <summary>
    ///     Defines a given <see cref="NPC"/>'s HP based on the current difficulty mode.
    /// </summary>
    /// <param name="npc">The NPC to set the HP for.</param>
    /// <param name="normalModeHP">HP value for Normal Mode.</param>
    /// <param name="expertModeHP">HP value for Expert Mode.</param>
    /// <param name="masterModeHP">HP value for Master Mode.</param>
    public static void SetLifeMaxByMode(this NPC npc, int normalModeHP, int expertModeHP, int masterModeHP)
    {
        npc.lifeMax = normalModeHP;
        if (Main.expertMode)
            npc.lifeMax = expertModeHP;
        if (Main.masterMode)
            npc.lifeMax = masterModeHP;
    }

    public static void SmoothFlyNear(this Entity entity, Vector2 destination, float movementSharpnessInterpolant, float movementSmoothnessInterpolant)
    {
        // Calculate the ideal velocity. The closer movementSharpnessInterpolant is to 1, the more closely the entity will hover exactly at the destination.
        // Lower, greater than zero values result in a greater tendency to hover in the general vicinity of the destination, rather than zipping straight towards it.
        Vector2 idealVelocity = (destination - entity.Center) * Clamp(movementSharpnessInterpolant, 0.0001f, 1f);

        // Interpolate towards the ideal velocity. The closer movementSmoothnessInterpolant is to 1, the more opportunities the entity has for overshooting and
        // more "curvy" motion.
        entity.velocity = Vector2.Lerp(entity.velocity, idealVelocity, Clamp(1f - movementSmoothnessInterpolant, 0.0001f, 1f));
    }
}