using Dalamud.Interface.Colors;
using ImGuiNET;

namespace GlamMaster.UI.PlayerPairing
{
    internal class PlayerPairingUI
    {
        public static void DrawPlayerPairingUI()
        {
            ImGui.BeginChild("Player_Pairing_UI##MainUI");

            ImGui.TextWrapped("This pannel is used to pair with people. Once paired with a player, send your encryption keys to each others.");
            ImGui.TextColored(ImGuiColors.DalamudOrange, "The permissions on this screen are permissions your want the other player to have on you.");
            ImGui.TextColored(ImGuiColors.ParsedPurple, "If you only want to control that player, you only have to exchange your encryption keys.");

            PlayerSelector.DrawPlayerSelector();
            ImGui.SameLine();
            PlayerPairingPlayerPanelBuilder.DrawPlayerPanel();
            PlayerActionBar.DrawPlayerActionBar();

            ImGui.EndChild();
        }
    }
}
