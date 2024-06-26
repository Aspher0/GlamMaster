using Dalamud.Interface.Colors;
using GlamMaster.Socket;
using ImGuiNET;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourControlUI
    {
        public static string testMessage = string.Empty;

        public static void DrawGlamourControlUI()
        {
            ImGui.BeginChild("Glamour_Control_UI##MainUI");

            if (!SocketManager.IsSocketConnected)
            {
                ImGui.TextWrapped("Please, connect to a server by going in the settings tab.");
            }
            else
            {
                ImGui.TextColored(ImGuiColors.DalamudViolet, "Paired Players");

                GlamourControlPlayerSelector.DrawGlamourControlPlayerSelector();
                ImGui.SameLine();
                GlamourControlPlayerPannelBuilder.DrawGlamourControlPanel();
            }

            ImGui.EndChild();
        }
    }
}
