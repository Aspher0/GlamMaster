using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.PlayerPairing;

public class PlayerPairingGlamourerTab
{
    public static void DrawGlamourerPermissionsTab(PairedPlayer SelectedPlayer)
    {
        ImGui.Spacing();

        var GlamourerPermissions = SelectedPlayer.permissionsList.glamourerControlPermissions;

        // Enable Glamourer Control Module Checkbox

        bool canControlGlamourer = GlamourerPermissions.canControlGlamourer;

        if (ImGui.Checkbox("Enable the Glamourer Control module", ref canControlGlamourer))
        {
            GlamourerPermissions.canControlGlamourer = canControlGlamourer;
            Service.Configuration!.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("This will enable the Glamourer Control module, allowing that player to control your glamourer.");
            ImGui.EndTooltip();
        }

        if (GlamourerPermissions.canControlGlamourer)
        {

        }
    }
}
