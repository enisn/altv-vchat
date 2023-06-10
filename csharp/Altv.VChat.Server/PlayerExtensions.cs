using AltV.Net;
using AltV.Net.Elements.Entities;

namespace Altv.VChat.Server;
public static class PlayerExtensions 
{
    private static Action<IPlayer, string> sendChatMessage;
    static PlayerExtensions()
    {
        Alt.Import(VChat.ModuleName, "send", out sendChatMessage);
    }

    public static void SendVChatMessage(this IPlayer player, string message)
    {
        sendChatMessage(player, message);
    }
}
