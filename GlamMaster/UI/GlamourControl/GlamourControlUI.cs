using Dalamud.Interface.Colors;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.Socket.EmitEvents;
using GlamMaster.UI.PlayerPairing;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.GlamourControl
{
    internal class GlamourControlUI
    {
        public static string testMessage = string.Empty;

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
                ImGui.SameLine();

                if (ImGui.BeginChild("GlamourControlPannel", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), true))
                {
                    ImGui.InputText("", ref testMessage, 200);

                    if (ImGui.Button("Send global message TO EVERY OTHER PLAYER"))
                    {
                        if (Service.ClientState.LocalPlayer != null)
                        {
                            string message = $"{Service.ClientState.LocalPlayer!.Name.ToString()}: {testMessage}";

                            _ = SocketManager.GetClient!.SendGlobalMessage(message);
                            testMessage = string.Empty;
                        }
                    }

                    ImGui.EndChild();
                }
            }

            ImGui.EndChild();
        }
    }
}
