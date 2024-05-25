using GlamMaster.Socket;
using GlamMaster.Socket.EmitEvents;
using GlamMaster.Structs.Payloads;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourerControlTab
    {
        public static void DrawGlamourerControlTab(PairedPlayer SelectedPlayer)
        {
            float availableWidth = ImGui.GetContentRegionAvail().X;

            if (ImGui.Button("Send Payload", new Vector2(availableWidth, 0)) && SocketManager.GetClient != null)
            {
                Payload payload = new Payload(PayloadType.PermissionsRequest);

                _ = SocketManager.GetClient.SendPayloadToPlayer(SelectedPlayer, payload);
            }
        }
    }
}
