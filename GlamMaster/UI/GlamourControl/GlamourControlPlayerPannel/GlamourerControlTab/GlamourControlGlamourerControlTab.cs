using Dalamud.Interface.Colors;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.GlamourControl;

public class GlamourControlGlamourerControlTab
{
    public static void DrawGlamourerControlTab(PairedPlayer SelectedPlayer)
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;

        var GlamourerPermissions = SelectedPlayer.theirPermissionsListToUser!.glamourerControlPermissions;

        if (!GlamourerPermissions.canControlGlamourer)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, $"You do not have the permission to use the Glamourer Control Module.");
        }
        else
        {

        }
    }
}
