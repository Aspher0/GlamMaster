using GlamMaster.Helpers;
using GlamMaster.Structs;
using GlamMaster.Structs.Payloads;
using GlamMaster.Structs.WhitelistedPlayers;
using SocketIOClient;
using System;

namespace GlamMaster.Socket.SocketOnEvents
{
    public static class ReceivePayloadFromPlayerHandler
    {
        public static void Handle(SocketIOResponse response)
        {
            try
            {
                var data = response.GetValue<FullPayloadJSON>();

                GlamLogger.Print($"Received payload from {data.FromPlayer.playerName}@{data.FromPlayer.homeWorld}");

                PairedPlayer? pairedPlayer = GlobalHelper.TryGetExistingPairedPlayerInConfig(data.FromPlayer.playerName, data.FromPlayer.homeWorld);

                if (pairedPlayer == null)
                {
                    GlamLogger.Debug("Received a payload from an unauthorized player: " + data.FromPlayer.playerName);
                    GlamLogger.PrintErrorChannel("Received a payload from an unauthorized player: " + data.FromPlayer.playerName);
                    return;
                }

                Payload? payload = PayloadHelper.DecryptPayload(data.Payload, pairedPlayer.theirSecretEncryptionKey);

                if (payload == null)
                {
                    GlamLogger.Error($"Payload could not be decrypted. This could be because {data.FromPlayer.playerName}'s encryption key has changed, or is not valid.");
                    GlamLogger.PrintErrorChannel($"Payload could not be decrypted. This could be because {data.FromPlayer.playerName}'s encryption key has changed, or is not valid.");
                    return;
                }

                GlamLogger.Print($"Payload type: {payload.payloadType.ToString()}");
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Could not parse response from the transferPayloadFromPlayer socket Event: " + ex.Message);
            }
        }
    }
}
