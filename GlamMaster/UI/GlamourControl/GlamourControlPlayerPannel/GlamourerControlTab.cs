using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourerControlTab
    {
        public static void DrawGlamourerControlTab(PairedPlayer SelectedPlayer)
        {
            float availableWidth = ImGui.GetContentRegionAvail().X;

            /*if (ImGui.Button("Get their permissions from configuration", new Vector2(availableWidth, 0)) && SocketManager.GetClient != null)
            {
                GlamLogger.Print($"Null ? {(SelectedPlayer.theirPermissionsListToUser == null).ToString()}");
            }*/
        }
    }
}
