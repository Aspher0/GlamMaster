using Dalamud.Interface.Colors;
using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourControlPlayerPannelBuilder
    {
        public static void DrawGlamourControlPanel()
        {
            if (ImGui.BeginChild("GlamourControlPlayerPanel", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                if (GlamourControlPlayerSelector.ViewModePlayerSelector == "default")
                {
                    ImGui.TextWrapped("Select a player on the left.");
                }
                else if (GlamourControlPlayerSelector.ViewModePlayerSelector == "edit" && GlamourControlPlayerSelector.SelectedPlayer != null)
                {
                    DrawPlayerInfos(GlamourControlPlayerSelector.SelectedPlayer);

                    if (ImGui.BeginTabBar("GlamourControlPlayerTabBar"))
                    {
                        if (ImGui.BeginTabItem("Glamourer"))
                        {
                            GlamourerControlTab.DrawGlamourerControlTab(GlamourControlPlayerSelector.SelectedPlayer);
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                }
            }

            ImGui.EndChild();
        }
        public static void DrawPlayerInfos(PairedPlayer SelectedPlayer)
        {
            ImGui.Text($"Player Name: {SelectedPlayer.pairedPlayer.playerName}");
            ImGui.Text($"Player Homeworld: {SelectedPlayer.pairedPlayer.homeWorld}");

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
}
