using Dalamud.Interface.Colors;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.GlamourControl;

public class GlamourControlPenumbraControlTab
{
    public static void DrawPenumbraControlTab(PairedPlayer SelectedPlayer)
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;

        var PenumbraPermissions = SelectedPlayer.theirPermissionsListToUser!.penumbraControlPermissions;

        if (PenumbraPermissions == null || !PenumbraPermissions.CanControlPenumbra)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, $"You do not have the permission to use the Penumbra Control Module.");
        }
        else
        {

        }
    }
}
