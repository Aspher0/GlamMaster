using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GlamMaster.UI.PlayerPairing;

public class PlayerSelector
{
    public static PairedPlayer? SelectedPlayer;
    public static string CurrentPlayerSelectorSearch = string.Empty;
    public static string ViewModePlayerSelector = "default";
    public static int CurrentDraggedPlayerIndex = -1;

    public static void DrawPlayerSelector(bool displayDisabledText = true)
    {
        string? selectedPlayerId = SelectedPlayer?.uniqueID;
        List<PairedPlayer> pairedPlayers = Service.Configuration!.PairedPlayers;

        if (ImGui.BeginChild("Player_Pairing_UI##PlayerSelector", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
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
                bool isEnabled = player.permissionsList.enabled;
                bool emptyPairedPlayerEncKey = player.theirSecretEncryptionKey == string.Empty;

                string displayName = $"{player.pairedPlayer.playerName}@{player.pairedPlayer.homeWorld}";
                string displayText = (!isEnabled && displayDisabledText ? "[Disabled] " : (emptyPairedPlayerEncKey ? "[Warning] " : "")) + displayName;
                string id = player.uniqueID;

                bool nameMatch = CommonHelper.RegExpMatch(displayText, CurrentPlayerSelectorSearch);

                if (nameMatch)
                {
                    if (ImGui.Selectable(displayText, id == selectedPlayerId))
                    {
                        SelectedPlayer = player;
                        ViewModePlayerSelector = "edit";
                    }

                    HandleDragDrop(displayName, pairedPlayers.IndexOf(player));
                }
            }

            ImGui.EndChild();
        }
    }

    private static void HandleDragDrop(string DisplayName, int index)
    {
        if (ImGui.BeginDragDropSource())
        {
            CurrentDraggedPlayerIndex = index;
            ImGui.Text("Dragging: " + DisplayName);

            unsafe
            {
                int* draggedIndex = &index;
                ImGui.SetDragDropPayload("DRAG_PLAYER", new IntPtr(draggedIndex), sizeof(int));
            }

            ImGui.EndDragDropSource();
        }

        if (ImGui.BeginDragDropTarget())
        {
            var acceptPayload = ImGui.AcceptDragDropPayload("DRAG_PLAYER");
            bool isDropping = false;

            unsafe
            {
                isDropping = acceptPayload.NativePtr != null;
            }

            if (isDropping)
            {
                var temp = Service.Configuration!.PairedPlayers[CurrentDraggedPlayerIndex];
                Service.Configuration.PairedPlayers.RemoveAt(CurrentDraggedPlayerIndex);
                Service.Configuration.PairedPlayers.Insert(index, temp);
                Service.Configuration.Save();
                CurrentDraggedPlayerIndex = -1;
            }

            ImGui.EndDragDropTarget();
        }
    }
}
