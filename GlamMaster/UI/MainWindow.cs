using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using GlamMaster.Socket;
using GlamMaster.Services;
using Dalamud.Interface.Components;
using Dalamud.Interface.Colors;
using GlamMaster.Socket.EmitEvents;
using GlamMaster.Helpers;

namespace GlamMaster.UI
{
    public class MainWindow : Window, IDisposable
    {
        private string tempServerUrl;
        private bool switchToMainTab;

        public MainWindow(Plugin plugin)
            : base("Glamour Master##GlamMasterMain", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(375, 330),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };

            tempServerUrl = Service.Configuration.SocketServerURL;
            switchToMainTab = false;
        }

        public void Dispose() { }

        public override void OnOpen()
        {
            tempServerUrl = Service.Configuration.SocketServerURL;
            switchToMainTab = true;
        }

        public override void OnClose()
        {
            ResetTempSocketServerURL();
        }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("SettingsTabs"))
            {
                if (switchToMainTab)
                {
                    ImGui.SetTabItemClosed("Settings");
                    switchToMainTab = false;
                }

                if (ImGui.BeginTabItem("Glamour Master"))
                {
                    DrawMainUI();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Settings"))
                {
                    DrawSettings();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }

        async private void DrawMainUI()
        {
            ImGui.BeginChild("Main_UI##GlamMasterMainTab01");

            if (ImGui.Button("Send socket to server"))
            {
                await SocketManager.GetClient.SendPing();
            }

            ImGui.Spacing();

            ImGui.EndChild();
        }

        private void DrawSettings()
        {
            ImGui.BeginChild("Main_UI##GlamMasterMainTab02");

            ImGui.Spacing();

            ImGui.Text("The server is");
            ImGui.SameLine();

            bool ServerConnecting = SocketManager.IsConnecting;

            if (ServerConnecting)
                ImGui.TextColored(ImGuiColors.DalamudOrange, "connecting");
            else
                ImGui.TextColored((SocketManager.IsSocketConnected ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed), (SocketManager.IsSocketConnected ? "connected" : "disconnected"));

            ImGui.Spacing();

            ImGui.InputText("Socket Server URL", ref tempServerUrl, 200U);

            if (ImGui.Button("Save Server URL"))
            {
                SaveServerUrl();
            }

            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Click this button to save the server URL.");

            if (ServerConnecting)
            {
                if (ImGui.Button("Abort Connection"))
                {
                    _ = SocketManager.DisposeSocket(SocketManager.GetClient);
                }
            } else
            {
                if (SocketManager.IsSocketConnected)
                {
                    if (ImGui.Button("Disconnect from Server"))
                    {
                        _ = SocketManager.DisconnectSocket(SocketManager.GetClient, true);
                    }
                }
                else
                {
                    if (ImGui.Button("Connect to the Server"))
                    {
                        _ = SocketManager.InitializeSocket();
                    }
                }
            }

            bool AutoConnectToSocketServer = Service.Configuration.AutomaticallyConnectToSocketServer;

            if (ImGui.Checkbox("Connect to the server on startup", ref AutoConnectToSocketServer))
            {
                Service.Configuration.AutomaticallyConnectToSocketServer = AutoConnectToSocketServer;
                Service.Configuration.Save();
            }

            ImGui.EndChild();
        }

        private async void SaveServerUrl()
        {
            if (Service.Configuration.SocketServerURL != tempServerUrl)
            {
                Service.Configuration.SocketServerURL = tempServerUrl;
                Service.Configuration.Save();

                bool wasConnected = SocketManager.IsSocketConnected;

                await SocketManager.DisposeSocket(SocketManager.GetClient, true);

                if (wasConnected)
                {
                    _ = SocketManager.InitializeSocket();
                }
            }
        }

        public void ResetTempSocketServerURL()
        {
            tempServerUrl = Service.Configuration.SocketServerURL;
        }
    }
}