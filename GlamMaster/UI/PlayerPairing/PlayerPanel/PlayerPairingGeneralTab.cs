using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.PlayerPairing
{
    internal class PlayerPairingGeneralTab
    {
        public static void DrawGeneralTab(PairedPlayer SelectedPlayer)
        {
            if (ImGui.Checkbox("Automatically request their permissions", ref SelectedPlayer.requestTheirPermissionsAutomatically))
            {
                Service.Configuration!.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("If this box is checked, the plugin will automatically request their permissions when you want to control them.");
                ImGui.EndTooltip();
            }
        }
    }
}
