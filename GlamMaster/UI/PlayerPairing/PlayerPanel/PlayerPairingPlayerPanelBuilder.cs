using Dalamud.Interface.Colors;
using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.PlayerPairing;

public class PlayerPairingPlayerPanelBuilder
{
    public static void DrawPlayerPanel()
    {
        if (ImGui.BeginChild("Player_Pairing_UI##PlayerPanel", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()), true))
        {
            if (PlayerSelector.ViewModePlayerSelector == "default")
            {
                ImGui.TextWrapped("Select a player or press the button in the bottom left corner to add one.");
            }
            else if (PlayerSelector.ViewModePlayerSelector == "edit" && PlayerSelector.SelectedPlayer != null)
            {
                DrawPlayerInfos(PlayerSelector.SelectedPlayer);

                bool emptyPairedPlayerEncKey = PlayerSelector.SelectedPlayer.theirSecretEncryptionKey == string.Empty;

                if (ImGui.BeginTabBar("PlayerPairingSettingsTabs"))
                {
                    if (ImGui.BeginTabItem("General"))
                    {
                        PlayerPairingGeneralTab.DrawGeneralTab(PlayerSelector.SelectedPlayer);
                        ImGui.EndTabItem();
                    }

                    if (!emptyPairedPlayerEncKey && PlayerSelector.SelectedPlayer.permissionsList.enabled)
                    {
                        if (ImGui.BeginTabItem("Penumbra"))
                        {
                            PlayerPairingPenumbraTab.DrawPenumbraPermissionsTab(PlayerSelector.SelectedPlayer);
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Glamourer"))
                        {
                            PlayerPairingGlamourerTab.DrawGlamourerPermissionsTab(PlayerSelector.SelectedPlayer);
                            ImGui.EndTabItem();
                        }
                    }

                    ImGui.EndTabBar();
                }
            }

            ImGui.EndChild();
        }
    }

    public static void DrawPlayerInfos(PairedPlayer SelectedPlayer)
    {
        bool playerEnabled = SelectedPlayer.permissionsList.enabled;

        if (ImGui.Checkbox("Enabled", ref playerEnabled))
        {
            SelectedPlayer.permissionsList.enabled = playerEnabled;
            Service.Configuration!.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("If this box is unchecked, the player will not be able to control you.");
            ImGui.Text("Acts as an easy and fast way to toggle someone's control.");
            ImGui.EndTooltip();
        }

        if (!playerEnabled)
        {
            ImGui.TextColored(ImGuiColors.DalamudOrange, "This player is not enabled. They can not control you. Ignore if you only want to control them.");
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        bool emptyPairedPlayerEncKey = SelectedPlayer.theirSecretEncryptionKey == string.Empty;

        if (emptyPairedPlayerEncKey)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, $"Please, go to the \"General\" tab and paste {SelectedPlayer.pairedPlayer.playerName}'s encryption key.");
            ImGui.TextColored(ImGuiColors.DalamudRed, "If the key is not specified, most of the features won't work.");
            ImGui.Spacing();
        }
    }
}
