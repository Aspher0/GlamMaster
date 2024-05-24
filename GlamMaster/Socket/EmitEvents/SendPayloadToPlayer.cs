using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs.Payloads;
using GlamMaster.Structs.WhitelistedPlayers;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GlamMaster.Socket.EmitEvents
{
    public static class SendPayloadToPlayerExtension
    {
        public static async Task SendPayloadToPlayer(this SocketIOClient.SocketIO client, PairedPlayer pairedPlayerToSendTo, Payload payloadToEncrypt)
        {
            if (!SocketManager.IsClientValidAndConnected(client, true) || Service.ConnectedPlayer == null)
                return;

            FullPayloadToPlayer payload = PayloadHelper.BuildFullPayload(Service.ConnectedPlayer, pairedPlayerToSendTo, payloadToEncrypt);

            try
            {
                string serializedPayloadToSend = JsonConvert.SerializeObject(payload);
                await client.EmitAsync("sendPayloadToPlayer", serializedPayloadToSend);
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Failed to send payload to player: " + ex.Message);
            }
        }
    }
}
