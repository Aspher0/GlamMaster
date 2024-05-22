using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using ImGuiNET;

namespace GlamMaster.Events
{
    internal class CharacterLogEvents
    {
        public static void OnCharacterLogin()
        {
            GlamLogger.Information("Character logged in.");

            if (Service.Configuration.AutoConnectSocketServer != null)
            {
                _ = SocketManager.InitializeSocket(Service.Configuration.AutoConnectSocketServer);
            }
        }

        public static void OnCharacterLogout()
        {
            GlamLogger.Information("Character logged out.");

            if (SocketManager.IsConnecting)
            {
                SocketManager.AbortSocketConnection(SocketManager.GetClient);
            } else if (SocketManager.IsSocketConnected)
            {
                _ = SocketManager.DisconnectSocket(SocketManager.GetClient, true);
            }
        }
    }
}
