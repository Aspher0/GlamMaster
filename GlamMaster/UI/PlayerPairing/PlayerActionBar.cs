using Dalamud.Game.ClientState.Objects.SubKinds;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace GlamMaster.UI.PlayerPairing
{
    internal class PlayerActionBar
    {
        public static void DrawPlayerActionBar()
        {
            string text = "Target a player to add";
            bool isLogged = Service.ClientState.IsLoggedIn;

            if (isLogged)
            {
                bool isTargeting = Service.TargetManager.Target != null;
                bool isTargetingPlayer = false;
                PairedPlayer? alreadyFoundPlayer = null;
                IPlayerCharacter? playerCharacter = null;
                World? targetedPlayerHomeWorld = null;

                if (isTargeting)
                {
                    var target = Service.TargetManager.Target;

                    if (target is IPlayerCharacter pc)
                    {
                        if (pc.Address != Service.ClientState.LocalPlayer?.Address)
                        {
                            isTargetingPlayer = true;
                            playerCharacter = pc;
                            targetedPlayerHomeWorld = Service.DataManager.GetExcelSheet<World>()?.GetRow(playerCharacter.HomeWorld.RowId);

                            if (targetedPlayerHomeWorld != null)
                            {
                                // alreadyFoundPlayer = GlobalHelper.TryGetExistingPairedPlayerInConfig(playerCharacter.Name.TextValue, targetedPlayerHomeWorld.Name.RawString);
                                alreadyFoundPlayer = GlobalHelper.TryGetExistingPairedPlayerInConfig(playerCharacter.Name.TextValue, targetedPlayerHomeWorld.Value.Name.ToString());

                                // text = (alreadyFoundPlayer != null ? "Edit " : "Add ") + $"{playerCharacter.Name.TextValue}@{targetedPlayerHomeWorld.Name.RawString}";
                                text = (alreadyFoundPlayer != null ? "Edit " : "Add ") + $"{playerCharacter.Name.TextValue}@{targetedPlayerHomeWorld.Value.Name.ToString()}";
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
                        PlayerSelector.SelectedPlayer = alreadyFoundPlayer;
                        PlayerSelector.ViewModePlayerSelector = "edit";
                    }
                    else if (isTargetingPlayer && playerCharacter != null && targetedPlayerHomeWorld != null)
                    {
                        // var newPlayer = new PairedPlayer(playerCharacter.Name.TextValue, targetedPlayerHomeWorld.Name.RawString);
                        var newPlayer = new PairedPlayer(playerCharacter.Name.TextValue, targetedPlayerHomeWorld.Value.Name.ToString());
                        Service.Configuration!.AddPairedPlayer(newPlayer);
                        Service.Configuration.Save();
                        PlayerSelector.SelectedPlayer = newPlayer;
                        PlayerSelector.ViewModePlayerSelector = "edit";
                    }
                }

                ImGui.SameLine();
            }

            bool ctrlPressed = ImGui.GetIO().KeyCtrl;

            if (ctrlPressed && PlayerSelector.SelectedPlayer != null)
            {
                if (ImGui.Button("Delete"))
                {
                    Service.Configuration!.RemovePairedPlayer(PlayerSelector.SelectedPlayer);
                    Service.Configuration.Save();
                    PlayerSelector.SelectedPlayer = null;
                    PlayerSelector.ViewModePlayerSelector = "default";
                }
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
                ImGui.Button("Delete");
                ImGui.PopStyleVar();
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text("Hold CTRL to delete.");
                ImGui.EndTooltip();
            }
        }
    }
}
