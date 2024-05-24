using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs.Payloads;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GlamMaster.Socket.EmitEvents
{
    public static class SendClientInfosExtension
    {
        public static async Task SendClientInfos(this SocketIOClient.SocketIO client)
        {
            if (!SocketManager.IsClientValidAndConnected(client, true) || Service.ConnectedPlayer == null)
                return;

            string PlayerName = Service.ConnectedPlayer.playerName;
            string PlayerHomeworld = Service.ConnectedPlayer.homeWorld;

            SendClientInfos clientInfos = new SendClientInfos(PlayerName, PlayerHomeworld);

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
