using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.Settings;

/// <summary>
/// Represents the general tab for the server list in the settings panel when a server is selected (in edit mode).
/// </summary>
public class ServerListGeneralTab
{
    private static readonly AsyncTaskExecutionHelper TaskManager = new();

    /// <summary>
    /// Event handler for when the connection status changes.
    /// Used to abort an existing server connection and then to connect to another server.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="isConnecting">Indicates whether the connection is in progress.</param>
    private static void OnIsConnectingChanged(object? sender, bool isConnecting)
    {
        if (!isConnecting)
        {
            SocketManager.IsConnectingChanged -= OnIsConnectingChanged;
            _ = SocketManager.InitializeSocket(ServerSelector.SelectedServer);
        }
    }

    /// <summary>
    /// Draws the general tab for the server list in the settings panel.
    /// </summary>
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
                    // Determines whether the selected server in the pannel is the one currently being processed in SocketManager.
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

                    if (isProcessingServer)
                    {
                        // If the currently selected server is being processed (connecting or connected), the user can abort the connection or disconnect from the server.

                        if (SocketManager.IsConnecting)
                        {
                            if (ImGui.Button("Abort Connection"))
                            {
                                _ = SocketManager.AbortSocketConnection(SocketManager.GetClient);
                            }
                        }
                        else if (SocketManager.IsSocketConnected)
                        {
                            if (ImGui.Button("Disconnect from Server"))
                            {
                                _ = SocketManager.DisconnectSocket(SocketManager.GetClient, true);
                            }
                        }
                    }
                    else
                    {
                        // If the currently selected server is not being processed, the user can connect to the server.

                        if (Service.ClientState.IsLoggedIn)
                        {
                            string buttonText = SocketManager.IsConnecting ? "Abort connection and connect" : "Connect to the Server";
                            if (ImGui.Button(buttonText))
                            {
                                if (SocketManager.IsConnecting)
                                {
                                    // If the socket is currently connecting, abort the connection and then connect to the server.
                                    TaskManager.StartTask(
                                        SocketManager.AbortSocketConnection(SocketManager.GetClient),
                                        () => SocketManager.IsConnectingChanged += OnIsConnectingChanged
                                    );
                                }
                                else if (SocketManager.IsSocketConnected)
                                {
                                    // If the socket is currently connected, disconnect from the server and then connect to the server.
                                    TaskManager.StartTask(
                                        SocketManager.DisconnectSocket(SocketManager.GetClient, true),
                                        () => _ = SocketManager.InitializeSocket(ServerSelector.SelectedServer)
                                    );
                                }
                                else
                                {
                                    // If the socket is currently disconnected, connect to the server.
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
