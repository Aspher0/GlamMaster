using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using GlamMaster.Services;
using ImGuiNET;
using System.Collections.Generic;
using System;
using System.Numerics;
using GlamMaster.Structs.WhitelistedPlayers;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.GeneratedSheets;
using OtterGui.Classes;
using Dalamud.Interface.Utility;

namespace GlamMaster.UI
{
    internal class PlayerPairingUI
    {
        private static PairedPlayer? SelectedPlayer;
        private static LowerString CurrentPlayerSelectorSearch = LowerString.Empty;
        private static string ViewModePlayerSelector = "default";
        private static int CurrentDraggedPlayerIndex = -1;

        public static void DrawPlayerPairingUI()
        {
            ImGui.BeginChild("Player_Pairing_UI##PlayerSelector");

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Player List");

            string? selectedPlayerId = SelectedPlayer?.uniqueID;
            List<PairedPlayer> pairedPlayers = Service.Configuration!.PairedPlayers;

            if (ImGui.BeginChild("PlayerSelector", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                ImGui.SetNextItemWidth(200 * ImGuiHelpers.GlobalScale);
                LowerString.InputWithHint("##playerFilter", "Filter...", ref CurrentPlayerSelectorSearch, 300);

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Filter by name or world.");
                    ImGui.EndTooltip();
                }

                ImGui.Spacing();

                foreach (var player in pairedPlayers)
                {
                    if (player == null) continue;

                    bool isEnabled = player.permissionsList.enabled;
                    bool nameMatch = GlobalHelper.RegExpMatch($"{player.playerName}@{player.homeWorld}", CurrentPlayerSelectorSearch);

                    if (nameMatch)
                    {
                        string displayName = $"{player.playerName}@{player.homeWorld}";
                        string id = player.uniqueID;
                        string displayText = (!isEnabled ? "[Disabled] " : "") + displayName;

                        if (ImGui.Selectable(displayText, id == selectedPlayerId))
                        {
                            SelectedPlayer = player;
                            ViewModePlayerSelector = "edit";
                        }

                        HandleDragDrop(player, displayName, pairedPlayers.IndexOf(player));
                    }
                }

                ImGui.EndChild();
            }

            ImGui.SameLine();
            DrawPlayerPanel();

            HandleAddOrEditButton();

            ImGui.EndChild();
        }

        private static void HandleDragDrop(PairedPlayer player, string displayName, int index)
        {
            if (ImGui.BeginDragDropSource())
            {
                CurrentDraggedPlayerIndex = index;
                ImGui.Text("Dragging: " + displayName);

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

        private static void DrawPlayerPanel()
        {
            if (ImGui.BeginChild("PlayerPanel", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                if (ViewModePlayerSelector == "default")
                {
                    ImGui.TextWrapped("Press the button at the bottom of this window to add a player.");
                }
                else if (ViewModePlayerSelector == "edit" && SelectedPlayer != null)
                {
                    ImGui.Text("Editing entry N°" + SelectedPlayer.uniqueID);

                    ImGui.Spacing();

                    ImGui.Text("Player Name: " + SelectedPlayer.playerName);
                    ImGui.Text("Player Homeworld: " + SelectedPlayer.homeWorld);

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();

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

                    ImGui.Spacing();
                    ImGui.Separator();
                    ImGui.Spacing();

                    ImGui.Text("Their encryption key has");
                    ImGui.SameLine();

                    bool emptyPairedPlayerEncKey = SelectedPlayer.theirSecretEncryptionKey == string.Empty;

                    ImGui.TextColored(emptyPairedPlayerEncKey ? ImGuiColors.DalamudRed : ImGuiColors.HealerGreen, emptyPairedPlayerEncKey ? "not been entered" : "been entered");

                    float availableWidth = ImGui.GetContentRegionAvail().X;

                    if (emptyPairedPlayerEncKey)
                    {
                        if (ImGui.Button($"Paste {SelectedPlayer.playerName}'s encryption key", new Vector2(availableWidth, 0)))
                        {
                            string fromClipboard = ImGui.GetClipboardText();

                            if (!string.IsNullOrEmpty(fromClipboard))
                            {
                                SelectedPlayer.theirSecretEncryptionKey = fromClipboard;
                                Service.Configuration!.Save();
                            }
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"Please, ask {SelectedPlayer.playerName} to send you their secret encryption key and paste it here.");
                            ImGui.EndTooltip();
                        }
                    } else
                    {
                        string text = $"Reset {SelectedPlayer.playerName}'s encryption key";
                        bool shiftPressed = ImGui.GetIO().KeyCtrl;

                        if (shiftPressed)
                        {
                            if (ImGui.Button(text, new Vector2(availableWidth, 0)) && shiftPressed)
                            {
                                SelectedPlayer.theirSecretEncryptionKey = string.Empty;
                                Service.Configuration!.Save();
                            }
                        } else
                        {
                            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
                            ImGui.Button(text, new Vector2(availableWidth, 0));
                            ImGui.PopStyleVar();
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("Hold CTRL to reset.");
                            ImGui.Text($"Clicking this button will reset {SelectedPlayer.playerName}'s encryption key on your side.");
                            ImGui.Text("Useful if the other player has regenerated their key.");
                            ImGui.EndTooltip();
                        }
                    }

                    if (ImGui.Button("Copy your secret encryption key", new Vector2(availableWidth, 0)))
                    {
                        string myEncryptionKey = SelectedPlayer.mySecretEncryptionKey;
                        ImGui.SetClipboardText(myEncryptionKey);
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text($"Click this button to copy your encryption key and send it to {SelectedPlayer.playerName}.");
                        ImGui.EndTooltip();
                    }

                    ImGui.Spacing();
                }
            }
            ImGui.EndChild();
        }

        private static void HandleAddOrEditButton()
        {
            string text = "Target a player to add";
            bool isTargeting = Service.ClientState.IsLoggedIn && Service.TargetManager.Target != null;
            bool isTargetingPlayer = false;
            PairedPlayer? alreadyFoundPlayer = null;
            PlayerCharacter? playerCharacter = null;
            World? targetedPlayerHomeWorld = null;

            if (isTargeting)
            {
                var target = Service.TargetManager.Target;

                if (target is PlayerCharacter pc)
                {
                    if (pc != Service.ClientState.LocalPlayer)
                    {
                        isTargetingPlayer = true;
                        playerCharacter = pc;
                        targetedPlayerHomeWorld = Service.DataManager.GetExcelSheet<World>()?.GetRow(playerCharacter.HomeWorld.Id);

                        if (targetedPlayerHomeWorld != null)
                        {
                            alreadyFoundPlayer = GlobalHelper.TryGetExistingPlayerAlreadyInConfig(playerCharacter.Name.TextValue, targetedPlayerHomeWorld.Name.RawString);
                            text = alreadyFoundPlayer != null ? "Edit this player" : $"Add {playerCharacter.Name.TextValue}@{targetedPlayerHomeWorld.Name.RawString}";
                        }
                        else
                        {
                            text = "Error while retrieving target";
                            isTargetingPlayer = false;
                        }
                    }
                    else
                    {
                        text = "Cannot add yourself";
                    }
                }
            }

            if (ImGui.Button(text))
            {
                if (alreadyFoundPlayer != null)
                {
                    SelectedPlayer = alreadyFoundPlayer;
                    ViewModePlayerSelector = "edit";
                }
                else if (isTargetingPlayer && playerCharacter != null && targetedPlayerHomeWorld != null)
                {
                    var newPlayer = new PairedPlayer(playerCharacter.Name.TextValue, targetedPlayerHomeWorld.Name.RawString);
                    Service.Configuration!.AddPairedPlayer(newPlayer);
                    Service.Configuration.Save();
                    SelectedPlayer = newPlayer;
                    ViewModePlayerSelector = "edit";
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("Delete") && SelectedPlayer != null)
            {
                Service.Configuration!.RemovePairedPlayer(SelectedPlayer);
                Service.Configuration.Save();
                SelectedPlayer = null;
                ViewModePlayerSelector = "default";
            }
        }
    }
}
