using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using GlamMaster.Socket;
using GlamMaster.Socket.EmitEvents;

namespace GlamMaster.UI
{
    public class UIBuilder : Window, IDisposable
    {
        public UIBuilder(Plugin plugin)
            : base("Glamour Master##GlamMasterMain", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(375, 330),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
        }

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("SettingsTabs"))
            {
                if (ImGui.BeginTabItem("Glamour Master"))
                {
                    DrawMainUI();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Player Pairing"))
                {
                    PlayerPairingUI.DrawPlayerPairingUI();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Settings"))
                {
                    SettingsUI.DrawSettings();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }

        async private void DrawMainUI()
        {
            ImGui.BeginChild("Main_UI##GlamMasterMain");

            if (!SocketManager.IsSocketConnected)
            {
                ImGui.TextWrapped("Please, connect to a server by going in the settings tab.");
            } else
            {
                if (ImGui.Button("Send a ping to the server"))
                {
                    var client = SocketManager.GetClient;

                    if (client != null)
                        await client.SendPing();
                }

                ImGui.Spacing();
            }

            ImGui.EndChild();
        }
    }
}
