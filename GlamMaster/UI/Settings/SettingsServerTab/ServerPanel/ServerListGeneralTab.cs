using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using ImGuiNET;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace GlamMaster.UI.Settings;

public class ServerListGeneralTab
{
    private static readonly AsyncTaskExecutionHelper TaskManager = new();

    public static void DrawServerGeneralTab()
    {
        if (ImGui.BeginChild("Settings_UI##ServerListTab##ServerPanel##GeneralTab", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing())))
        {
            if (ServerSelector.ViewModeServerSelector == "default")
            {
                ImGui.TextWrapped("Select a server or press the button in the bottom left corner to add one.");
            }
            else if (ServerSelector.ViewModeServerSelector == "edit")
            {
                if (ServerSelector.SelectedServer != null)
                {
                    bool isProcessingServer = SocketManager.CurrentProcessingSocketServer?.Equals(ServerSelector.SelectedServer) ?? false;

                    ImGui.Text("This server is");
                    ImGui.SameLine();

                    if (isProcessingServer)
                    {
                        if (SocketManager.IsConnecting)
                            ImGui.TextColored(ImGuiColors.DalamudOrange, "connecting");
                        else
                            ImGui.TextColored((SocketManager.IsSocketConnected ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed), (SocketManager.IsSocketConnected ? "connected" : "disconnected"));
                    }
                    else
                    {
                        ImGui.TextColored(ImGuiColors.DalamudRed, "disconnected");
                    }

                    ImGui.Spacing();

                    if (ImGui.InputText("Socket Server Name", ref ServerSelector.SelectedServer.name, 200))
                    {
                        Service.Configuration!.Save();
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("A friendly name for the server. Could be anything.");
                        ImGui.EndTooltip();
                    }

                    ImGui.Spacing();

                    if (ImGui.InputText("Socket Server URL", ref ServerSelector.SelectedServer.serverURL, 200))
                    {
                        Service.Configuration!.Save();
                    }

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("The URL of the server. For example, if you're hosting your version of GlamMaster locally, it might be http://localhost:3000.");
                        ImGui.EndTooltip();
                    }

                    ImGui.Spacing();

                    if (isProcessingServer && SocketManager.IsConnecting)
                    {
                        if (ImGui.Button("Abort Connection"))
                        {
                            _ = SocketManager.AbortSocketConnection(SocketManager.GetClient);
                        }
                    }
                    else if (isProcessingServer && SocketManager.IsSocketConnected)
                    {
                        if (ImGui.Button("Disconnect from Server"))
                        {
                            _ = SocketManager.DisconnectSocket(SocketManager.GetClient, true);
                        }
                    }
                    else
                    {
                        if (Service.ClientState.IsLoggedIn)
                        {
                            if (ImGui.Button(SocketManager.IsConnecting ? "Abort other Server connection" : "Connect to the Server"))
                            {
                                if (SocketManager.IsConnecting)
                                {
                                    _ = SocketManager.AbortSocketConnection(SocketManager.GetClient);
                                }
                                else if (SocketManager.IsSocketConnected)
                                {
                                    TaskManager.StartTask(
                                        SocketManager.DisconnectSocket(SocketManager.GetClient, true),
                                        () => _ = SocketManager.InitializeSocket(ServerSelector.SelectedServer)
                                    );
                                }
                                else
                                {
                                    _ = SocketManager.InitializeSocket(ServerSelector.SelectedServer);
                                }
                            }
                        }
                        else
                        {
                            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
                            ImGui.Button("Connect to the Server");
                            ImGui.PopStyleVar();

                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.Text("Please connect to a character to connect to the server.");
                                ImGui.EndTooltip();
                            }
                        }
                    }

                    bool AutoConnectToSocketServer = Service.Configuration!.AutoConnectSocketServer?.Equals(ServerSelector.SelectedServer) ?? false;

                    if (ImGui.Checkbox("Connect to that server on login", ref AutoConnectToSocketServer))
                    {
                        Service.Configuration.AutoConnectSocketServer = AutoConnectToSocketServer ? ServerSelector.SelectedServer : null;
                        Service.Configuration.Save();
                    }

                    ImGui.Spacing();
                }
            }

            ImGui.EndChild();
        }
    }
}
