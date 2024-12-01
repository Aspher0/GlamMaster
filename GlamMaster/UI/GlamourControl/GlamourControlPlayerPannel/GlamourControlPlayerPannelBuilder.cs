using Dalamud.Interface.Colors;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.GlamourControl;

public class GlamourControlPlayerPannelBuilder
{
    public static void DrawGlamourControlPanel()
    {
        if (ImGui.BeginChild("Glamour_Control_UI##PlayerPanel", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()), true))
        {
            if (GlamourControlPlayerSelector.ViewModePlayerSelector == "default")
            {
                ImGui.TextWrapped("Select a player on the left.");
            }
            else if (GlamourControlPlayerSelector.ViewModePlayerSelector == "edit" && GlamourControlPlayerSelector.SelectedPlayer != null)
            {
                DrawPlayerInfos(GlamourControlPlayerSelector.SelectedPlayer);

                if (GlamourControlPlayerSelector.SelectedPlayer.theirPermissionsListToUser == null)
                {
                    ImGui.TextColored(ImGuiColors.DalamudOrange, $"The permissions for {GlamourControlPlayerSelector.SelectedPlayer.pairedPlayer.playerName}@{GlamourControlPlayerSelector.SelectedPlayer.pairedPlayer.homeWorld} have not been retrieved yet.");
                    ImGui.TextColored(ImGuiColors.DalamudOrange, $"Please, request their permissions to continue.");
                } else
                {
                    if (ImGui.BeginTabBar("PlayerPanel_Tabs"))
                    {
                        // Might Remove General tab

                        /* if (ImGui.BeginTabItem("General"))
                        {
                            GlamourControlGeneralTab.DrawGeneralTab(GlamourControlPlayerSelector.SelectedPlayer);
                            ImGui.EndTabItem();
                        } */

                        if (ImGui.BeginTabItem("Glamourer"))
                        {
                            GlamourControlGlamourerControlTab.DrawGlamourerControlTab(GlamourControlPlayerSelector.SelectedPlayer);
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Penumbra"))
                        {
                            GlamourControlPenumbraControlTab.DrawPenumbraControlTab(GlamourControlPlayerSelector.SelectedPlayer);
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                }
            }

            ImGui.EndChild();
        }
    }

    public static void DrawPlayerInfos(PairedPlayer SelectedPlayer)
    {
        if (ImGui.Button("Request Permissions") && SocketManager.GetClient != null)
        {
            SelectedPlayer.RequestTheirPermissions();
        }

        ImGui.SameLine();

        if (ImGui.Checkbox("Automatically request their permissions", ref SelectedPlayer.requestTheirPermissionsAutomatically))
        {
            Service.Configuration!.Save();

            if (SelectedPlayer.requestTheirPermissionsAutomatically)
                GlamourControlPlayerSelector.CheckAutoRequestPermissions(SelectedPlayer);
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("If this box is checked, the plugin will automatically request their permissions when you want to control them.");
            ImGui.EndTooltip();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        bool emptyPairedPlayerEncKey = SelectedPlayer.theirSecretEncryptionKey == string.Empty;

        if (emptyPairedPlayerEncKey)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, $"Please, go to the \"Player Pairing\" tab and specify {SelectedPlayer.pairedPlayer.playerName}'s encryption key.");
            ImGui.TextColored(ImGuiColors.DalamudRed, $"If the key is not specified, most of the features won't work.");
            ImGui.Spacing();
        }
    }
}
