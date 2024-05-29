using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.PlayerPairing
{
    internal class PlayerPairingGlamourerControlPermissionsTab
    {
        public static void DrawGlamourerControlPermissionsTab(PairedPlayer SelectedPlayer)
        {
            bool canControlGlamourer = SelectedPlayer.permissionsList.glamourerControlPermissions.canControlGlamourer;

            if (ImGui.Checkbox("Enable the Glamourer Control module", ref canControlGlamourer))
            {
                SelectedPlayer.permissionsList.glamourerControlPermissions.canControlGlamourer = canControlGlamourer;
                Service.Configuration!.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("This will enable the Glamourer Control module, allowing that player to control your glamourer.");
                ImGui.EndTooltip();
            }
        }
    }
}
