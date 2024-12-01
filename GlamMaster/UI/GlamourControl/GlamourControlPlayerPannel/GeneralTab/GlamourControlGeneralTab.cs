using GlamMaster.Socket;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.GlamourControl;

public class GlamourControlGeneralTab
{
    public static void DrawGeneralTab(PairedPlayer SelectedPlayer)
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;

        if (ImGui.Button("Request Permissions", new Vector2(availableWidth, 0)) && SocketManager.GetClient != null)
        {
            SelectedPlayer.RequestTheirPermissions();
        }
    }
}
