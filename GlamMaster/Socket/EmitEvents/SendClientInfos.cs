using GlamMaster.Helpers;
using GlamMaster.Structs;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GlamMaster.Socket.EmitEvents
{
    public static class SendClientInfosExtension
    {
        public static async Task SendClientInfos(this SocketIOClient.SocketIO client, SendClientInfos clientInfos)
        {
            if (!SocketManager.IsClientValidAndConnected(client, true))
                return;

            try
            {
                string jsonString = JsonConvert.SerializeObject(clientInfos);
                await client.EmitAsync("clientInfos", jsonString);
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Failed to send client infos: " + ex.Message);
                await SocketManager.DisconnectSocket(client);
            }
        }
    }
}
