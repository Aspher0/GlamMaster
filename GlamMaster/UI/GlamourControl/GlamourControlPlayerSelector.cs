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

            if (ImGui.BeginChild("PlayerSelector##GlamourControl", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
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
                    bool nameMatch = GlobalHelper.RegExpMatch($"{player.pairedPlayer.playerName}@{player.pairedPlayer.homeWorld}", CurrentPlayerSelectorSearch);

                    if (nameMatch)
                    {
                        bool emptyPairedPlayerEncKey = player.theirSecretEncryptionKey == string.Empty;

                        string displayName = $"{player.pairedPlayer.playerName}@{player.pairedPlayer.homeWorld}";
                        string id = player.uniqueID;
                        string displayText = (emptyPairedPlayerEncKey ? "[Warning] " : "") + displayName;

                        if (ImGui.Selectable(displayText, id == selectedPlayerId))
                        {
                            SelectedPlayer = player;
                            ViewModePlayerSelector = "edit";
                        }
                    }
                }

                ImGui.EndChild();
            }
        }
    }
}
