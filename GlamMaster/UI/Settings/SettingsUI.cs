using Dalamud.Interface.Colors;
using GlamMaster.Services;
using GlamMaster.Socket;
using ImGuiNET;
using System.Collections.Generic;
using System;
using GlamMaster.Structs;
using System.Numerics;
using GlamMaster.Helpers;

namespace GlamMaster.UI.Settings
{
    internal class SettingsUI
    {
        private static SocketServer? SelectedServer;
        private static string CurrentServerSelectorSearch = "";
        private static string ViewModeServerSelector = "default";
        private static int CurrentDraggedServerIndex = -1;

        public static void DrawSettings()
        {
            ImGui.BeginChild("Settings_UI##ServerSelector");

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Server List");

            string? SelectedPlayerId = SelectedServer?.uniqueID;
            List<SocketServer> SocketServers = Service.Configuration!.SocketServers;

            string state = SocketManager.IsConnecting ? "Connecting" : (SocketManager.IsSocketConnected ? "Connected" : "Disconnected");

            if (ImGui.BeginChild("ServerSelector", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                float availableWidth = ImGui.GetContentRegionAvail().X;

                ImGui.SetNextItemWidth(availableWidth);
                ImGui.InputTextWithHint("##server_search", "Search", ref CurrentServerSelectorSearch, 200U);

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Filter by name or server url.");
                    ImGui.EndTooltip();
                }

                ImGui.Spacing();

                for (int index = 0; index < SocketServers.Count; index++)
                {
                    SocketServer? SocketServer = SocketServers[index];

                    if (SocketServer == null)
                        continue;

                    bool isProcessingServer = SocketManager.CurrentProcessingSocketServer?.Equals(SocketServer) ?? false;

                    bool flag_id_match = GlobalHelper.RegExpMatch(SocketServer.uniqueID, CurrentServerSelectorSearch);
                    bool flag_name_match = GlobalHelper.RegExpMatch(SocketServer.name, CurrentServerSelectorSearch);
                    bool flag_url_match = GlobalHelper.RegExpMatch(SocketServer.serverURL, CurrentServerSelectorSearch);

                    if (flag_name_match || flag_url_match || (SocketServer.name == string.Empty && flag_id_match))
                    {
                        string name = SocketServer.name.Trim() == string.Empty ? SocketServer.uniqueID : SocketServer.name.Trim();
                        string id = SocketServer.uniqueID;

                        if (ImGui.Selectable((isProcessingServer ? $"[{state}] " : "") + name, id == SelectedPlayerId))
                        {
                            SelectedServer = SocketServer;
                            ViewModeServerSelector = "edit";
                        }

                        if (ImGui.BeginDragDropSource())
                        {
                            CurrentDraggedServerIndex = index;
                            ImGui.Text("Dragging: " + name);

                            unsafe
                            {
                                int* draggedIndex = &index;
                                ImGui.SetDragDropPayload("DRAG_SERVER", new IntPtr(draggedIndex), sizeof(int));
                            }

                            ImGui.EndDragDropSource();
                        }

                        if (ImGui.BeginDragDropTarget())
                        {
                            ImGuiPayloadPtr acceptPayload = ImGui.AcceptDragDropPayload("DRAG_SERVER");
                            bool isDropping = false;

                            unsafe
                            {
                                isDropping = acceptPayload.NativePtr != null;
                            }

                            if (isDropping)
                            {
                                var temp = SocketServers[CurrentDraggedServerIndex];
                                SocketServers.RemoveAt(CurrentDraggedServerIndex);
                                SocketServers.Insert(index, temp);
                                Service.Configuration.Save();
                                CurrentDraggedServerIndex = -1;
                            }

                            ImGui.EndDragDropTarget();
                        }
                    }
                }

                ImGui.EndChild();
            }
            ImGui.SameLine();

            if (ImGui.BeginChild("ServerPannel", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                if (ViewModeServerSelector == "default")
                {
                    ImGui.TextWrapped("Select a server or press the button in the bottom left corner to add one.");
                }
                else if (ViewModeServerSelector == "edit")
                {
                    if (SelectedServer != null)
                    {
                        bool isProcessingServer = SocketManager.CurrentProcessingSocketServer?.Equals(SelectedServer) ?? false;

                        ImGui.Text("This server is");
                        ImGui.SameLine();

                        if (isProcessingServer)
                        {
                            if (SocketManager.IsConnecting)
                                ImGui.TextColored(ImGuiColors.DalamudOrange, "connecting");
                            else
                                ImGui.TextColored((SocketManager.IsSocketConnected ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed), (SocketManager.IsSocketConnected ? "connected" : "disconnected"));
                        } else
                        {
                            ImGui.TextColored(ImGuiColors.DalamudRed, "disconnected");
                        }

                        ImGui.Spacing();

                        if (ImGui.InputText("Socket Server Name", ref SelectedServer.name, 200))
                        {
                            Service.Configuration.Save();
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("A friendly name for the server. Could be anything.");
                            ImGui.EndTooltip();
                        }

                        ImGui.Spacing();

                        if (ImGui.InputText("Socket Server URL", ref SelectedServer.serverURL, 200))
                        {
                            Service.Configuration.Save();
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("The URL of the server. For example, if you're hosting your version of GlamMaster locally, it might be http://localhost:3000.");
                            ImGui.EndTooltip();
                        }

                        ImGui.Spacing();

                        if (isProcessingServer || SocketManager.CurrentProcessingSocketServer == null)
                        {
                            if (isProcessingServer && SocketManager.IsConnecting)
                            {
                                if (ImGui.Button("Abort Connection"))
                                {
                                    SocketManager.AbortSocketConnection(SocketManager.GetClient);
                                }
                            }
                            else
                            {
                                if (isProcessingServer && SocketManager.IsSocketConnected)
                                {
                                    if (ImGui.Button("Disconnect from Server"))
                                    {
                                        _ = SocketManager.DisconnectSocket(SocketManager.GetClient, true);
                                    }
                                }
                                else
                                {
                                    bool isLoggedIn = Service.ClientState.IsLoggedIn;

                                    if (isLoggedIn)
                                    {
                                        if (ImGui.Button("Connect to the Server") && isLoggedIn)
                                        {
                                            _ = SocketManager.InitializeSocket(SelectedServer);
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
                            }
                        }

                        bool AutoConnectToSocketServer = Service.Configuration.AutoConnectSocketServer?.Equals(SelectedServer) ?? false;

                        if (ImGui.Checkbox("Connect to that server on login", ref AutoConnectToSocketServer))
                        {
                            Service.Configuration.AutoConnectSocketServer = AutoConnectToSocketServer ? SelectedServer : null;
                            Service.Configuration.Save();
                        }

                        ImGui.Spacing();
                    }
                }
            }

            ImGui.EndChild();

            if (ImGui.Button("Add"))
            {
                SocketServer NewSocketServer = new(Guid.NewGuid().ToString());
                Service.Configuration.AddSocketServer(NewSocketServer);
                Service.Configuration.Save();

                SelectedServer = NewSocketServer;
                ViewModeServerSelector = "edit";
            }

            ImGui.SameLine();

            bool ctrlPressed = ImGui.GetIO().KeyCtrl;

            if (ctrlPressed && SelectedServer != null)
            {
                if (ImGui.Button("Delete"))
                {
                    if (Service.Configuration.AutoConnectSocketServer?.Equals(SelectedServer) ?? false)
                        Service.Configuration.AutoConnectSocketServer = null;

                    Service.Configuration.RemoveSocketServer(SelectedServer);
                    Service.Configuration.Save();

                    SelectedServer = null;
                    ViewModeServerSelector = "default";
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

            ImGui.EndChild();
        }
    }
}
