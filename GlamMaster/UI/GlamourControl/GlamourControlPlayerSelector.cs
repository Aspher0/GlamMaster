using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourControlPlayerSelector
    {
        public static PairedPlayer? SelectedPlayer;
        public static string CurrentPlayerSelectorSearch = string.Empty;
        public static string ViewModePlayerSelector = "default";

        public static void DrawGlamourControlPlayerSelector()
        {
            string? selectedPlayerId = SelectedPlayer?.uniqueID;
            List<PairedPlayer> pairedPlayers = Service.Configuration!.PairedPlayers;

            if (ImGui.BeginChild("Glamour_Control_UI##PlayerSelectorList", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                float availableWidth = ImGui.GetContentRegionAvail().X;

                ImGui.SetNextItemWidth(availableWidth);
                ImGui.InputTextWithHint("##playerFilter", "Search", ref CurrentPlayerSelectorSearch, 300);

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Filter by name or world.");
                    ImGui.EndTooltip();
                }

                ImGui.Spacing();

                foreach (var player in pairedPlayers)
                {
                    bool emptyPairedPlayerEncKey = player.theirSecretEncryptionKey == string.Empty;

                    string displayName = $"{player.pairedPlayer.playerName}@{player.pairedPlayer.homeWorld}";
                    string displayText = (emptyPairedPlayerEncKey ? "[Warning] " : "") + displayName;
                    string id = player.uniqueID;

                    bool nameMatch = GlobalHelper.RegExpMatch(displayText, CurrentPlayerSelectorSearch);

                    if (nameMatch)
                    {
                        if (ImGui.Selectable(displayText, id == selectedPlayerId))
                        {
                            SelectedPlayer = player;
                            ViewModePlayerSelector = "edit";

                            CheckAutoRequestPermissions(player);
                        }
                    }
                }

                ImGui.EndChild();
            }
        }

        public static void CheckAutoRequestPermissions(PairedPlayer? SelectedPlayer)
        {
            /* Check if selected player != null
             * If ok, check if automatically request permissions = true
             * If not, do nothing
             * If yes, start a loop where every 5 seconds, it will send a request permissions infos
             * Also, request selected user's infos directly when tab is clicked
             * 
             * Do the same process on click on a user in the player pannel list
             */

            if (SelectedPlayer != null && SelectedPlayer.requestTheirPermissionsAutomatically)
            {
                // Start 5 seconds loop

                SelectedPlayer.RequestTheirPermissions();
            }
        }
    }
}
