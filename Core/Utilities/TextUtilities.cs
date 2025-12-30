using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace AstraBosses;

public static partial class Utilities
{
    /// <summary>
    ///     Displays the localized text received from the provided key in the chat, accounting for multiplayer.
    /// </summary>
    public static void BroadcastLocalizedText(string key, Color? textColor = null)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            Main.NewText(Language.GetTextValue(key), textColor ?? Color.White);
        else if (Main.netMode == NetmodeID.Server || Main.netMode == NetmodeID.MultiplayerClient)
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(key), textColor ?? Color.White);
    }
}