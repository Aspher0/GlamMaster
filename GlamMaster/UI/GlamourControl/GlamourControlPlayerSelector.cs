using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;

namespace GlamMaster.UI.GlamourControl;

public class GlamourControlPlayerSelector
{
    private static Timer? AutomaticPermissionRequestTimer;
    private static int TimerInSeconds = 10;

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
        /* Check if selected player != null && automatically request permissions = true
         * 
         * If yes, start a loop where every "TimerInSeconds" seconds, it will send a request permissions infos payload
         * Also, request selected user's infos directly when tab is clicked
         * 
         * Do the same process on click on a user in the player pannel list
         */

        StopTimer();

        if (SelectedPlayer != null && SelectedPlayer.requestTheirPermissionsAutomatically)
        {
            // Start 5 seconds loop
            SelectedPlayer.RequestTheirPermissions();

            AutomaticPermissionRequestTimer = new Timer(TimerInSeconds * 1000);
            AutomaticPermissionRequestTimer.Elapsed += (sender, e) => OnPermissionTimerElapsed(SelectedPlayer);
            AutomaticPermissionRequestTimer.AutoReset = true;
            AutomaticPermissionRequestTimer.Start();
        }
    }

    private static void OnPermissionTimerElapsed(PairedPlayer? SelectedPlayer)
    {
        if (SelectedPlayer == null || !SelectedPlayer.requestTheirPermissionsAutomatically)
        {
            StopTimer();
            return;
        }

        // Envoyer une demande de permissions
        SelectedPlayer.RequestTheirPermissions();
    }

    public static void StopTimer()
    {
        if (AutomaticPermissionRequestTimer != null)
        {
            AutomaticPermissionRequestTimer.Stop();
            AutomaticPermissionRequestTimer.Dispose();
            AutomaticPermissionRequestTimer = null;
        }
    }
}
