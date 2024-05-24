using Dalamud.Interface.Colors;
using GlamMaster.Socket;
using GlamMaster.UI.PlayerPairing;
using ImGuiNET;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourControlUI
    {
        public static void DrawGlamourControlUI()
        {
            ImGui.BeginChild("Glamour_Control_UI##GlamMaster");

            if (!SocketManager.IsSocketConnected)
            {
                ImGui.TextWrapped("Please, connect to a server by going in the settings tab.");
            }
            else
            {
                ImGui.TextColored(ImGuiColors.DalamudViolet, "Paired Players");

                PlayerSelector.DrawPlayerSelector(false);
            }

            ImGui.EndChild();
        }
    }
}
