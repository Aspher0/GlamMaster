using FFXIVClientStructs.FFXIV.Client.System.Timer;
using GlamMaster.Helpers;
using System;
using System.Threading.Tasks;

/*
 * Base Emit event, only used to send a ping to the server and receive a pong !
 */

namespace GlamMaster.Socket.EmitEvents
{
    public static class SendPingExtension
    {
        // Usage : SocketManager.GetClient.SendPing();
        public static async Task SendPing(this SocketIOClient.SocketIO client)
        {
            if (!SocketManager.IsClientValidAndConnected(client, true))
                return;

            try
            {
                await client.EmitAsync("clientPing");
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Failed to send a ping: " + ex.Message);
            }
        }
    }
}
