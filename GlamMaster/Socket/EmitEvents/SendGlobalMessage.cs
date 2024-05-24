using GlamMaster.Helpers;
using System;
using System.Threading.Tasks;

namespace GlamMaster.Socket.EmitEvents
{
    public static class SendGlobalMessageExtension
    {
        public static async Task SendGlobalMessage(this SocketIOClient.SocketIO client, string message)
        {
            if (!SocketManager.IsClientValidAndConnected(client, true))
                return;

            try
            {
                await client.EmitAsync("globalMessage", message);
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Failed to send global message: " + ex.Message);
            }
        }
    }
}
