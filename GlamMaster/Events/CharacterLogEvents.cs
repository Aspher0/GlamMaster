using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;

namespace GlamMaster.Events;

public class CharacterLogEvents
{
    public static void OnCharacterLogin()
    {
        GlamLogger.Information("Character logged in.");

        Service.GetConnectedPlayer();

        if (Service.Configuration!.AutoConnectSocketServer != null)
        {
            _ = SocketManager.InitializeSocket(Service.Configuration.AutoConnectSocketServer);
        }
    }

    public static void OnCharacterLogout(int type, int code)
    {
        GlamLogger.Information("Character logged out.");

        Service.ConnectedPlayer = null;

        if (SocketManager.IsConnecting)
        {
            _ = SocketManager.AbortSocketConnection(SocketManager.GetClient);
        }
        else if (SocketManager.IsSocketConnected)
        {
            _ = SocketManager.DisconnectSocket(SocketManager.GetClient, true);
        }
    }
}
