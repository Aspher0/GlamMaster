using GlamMaster.Helpers;
using GlamMaster.Services;
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

                PairedPlayer? pairedPlayer = GlobalHelper.TryGetExistingPairedPlayerInConfig(data.FromPlayer.playerName, data.FromPlayer.homeWorld);

                if (pairedPlayer == null)
                {
                    GlamLogger.Debug("Received a payload from an unauthorized player: " + data.FromPlayer.playerName);
                    return;
                }

                Payload? payload = PayloadHelper.DecryptPayload(data.Payload, pairedPlayer.theirSecretEncryptionKey);

                if (payload == null)
                {
                    GlamLogger.Error($"Received payload from {data.FromPlayer.playerName}@{data.FromPlayer.homeWorld} but it could not be decrypted. This could be because {data.FromPlayer.playerName}'s encryption key has changed, or is not valid.");
                    GlamLogger.Print($"Received payload from {data.FromPlayer.playerName}@{data.FromPlayer.homeWorld} but it could not be decrypted. This could be because {data.FromPlayer.playerName}'s encryption key has changed, or is not valid.");
                    return;
                }

                if (payload.PayloadType == PayloadType.PermissionsRequest)
                {
                    // Send user's permissions to them
                    pairedPlayer.SendYourPermissions();
                }
                else if (payload.PayloadType == PayloadType.SendPermissions)
                {
                    // Receive their permissions to user
                    if (payload.Permissions == null)
                    {
                        GlamLogger.Error($"The payload does not contain {data.FromPlayer.playerName}'s permissions.");
                        return;
                    }

                    GlamLogger.Print($"Permissions of {data.FromPlayer.playerName} : Enabled: {payload.Permissions.enabled.ToString()}, Glamourer control: {payload.Permissions.glamourerControlPermissions.canControlGlamourer.ToString()}");

                    pairedPlayer.theirPermissionsListToUser = payload.Permissions;
                    Service.Configuration!.Save();
                }
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Could not parse response from the transferPayloadFromPlayer socket Event: " + ex.Message);
            }
        }
    }
}
