using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;

namespace GlamMaster.UI.PlayerPairing;

public class PlayerPairingPenumbraTab
{
    public static void DrawPenumbraPermissionsTab(PairedPlayer SelectedPlayer)
    {
        ImGui.Spacing();

        var PenumbraPermissions = SelectedPlayer.permissionsList.penumbraControlPermissions;

        // Enable the module Checkbox

        bool canControlPenumbra = PenumbraPermissions.CanControlPenumbra;

        if (ImGui.Checkbox("Enable the Penumbra Control module", ref canControlPenumbra))
        {
            PenumbraPermissions.CanControlPenumbra = canControlPenumbra;
            Service.Configuration!.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("This will enable the Penumbra Control module, allowing that player to control your penumbra.");
            ImGui.EndTooltip();
        }

        if (PenumbraPermissions.CanControlPenumbra)
        {
            // Allow them to view your mod folders

            bool canViewFullModPaths = PenumbraPermissions.CanViewFullModPaths;

            if (ImGui.Checkbox("Allow them to view your mod folders", ref canViewFullModPaths))
            {
                PenumbraPermissions.CanViewFullModPaths = canViewFullModPaths;
                Service.Configuration!.Save();
            }

            // Allow Mods install

            bool canInstallMods = PenumbraPermissions.CanInstallMods;

            if (ImGui.Checkbox("Allow them to install mods", ref canInstallMods))
            {
                PenumbraPermissions.CanInstallMods = canInstallMods;
                Service.Configuration!.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text($"This will allow {SelectedPlayer.pairedPlayer.playerName}@{SelectedPlayer.pairedPlayer.homeWorld} to install mods in your penumbra installation.");
                ImGui.EndTooltip();
            }

            // Allow Mods deletion

            bool canDeleteMods = PenumbraPermissions.CanDeleteMods;

            if (ImGui.Checkbox("Allow them to delete mods", ref canDeleteMods))
            {
                PenumbraPermissions.CanDeleteMods = canDeleteMods;
                Service.Configuration!.Save();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text($"This will allow {SelectedPlayer.pairedPlayer.playerName}@{SelectedPlayer.pairedPlayer.homeWorld} to delete mods from your penumbra installation.");
                ImGui.EndTooltip();
            }
        }
    }
}
