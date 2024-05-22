using Dalamud.Interface.Colors;
using GlamMaster.Services;
using GlamMaster.Socket;
using ImGuiNET;
using System.Collections.Generic;
using System;
using GlamMaster.Structs;
using System.Numerics;
using GlamMaster.Helpers;

namespace GlamMaster.UI
{
    public static class SettingsUI
    {
        private static SocketServer? SelectedServer;
        private static string CurrentServerSelectorSearch = "";
        private static string ViewModeServerSelector = "default";
        private static int CurrentDraggedServerIndex = -1;

        public static void DrawSettings()
        {
            ImGui.BeginChild("Settings_UI##ServerSelector");

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Server List");

            string? SelectedServerId = SelectedServer?.unique_id;
            List<SocketServer> SocketServers = Service.Configuration.SocketServers;

            string state = SocketManager.IsConnecting ? "Connecting" : (SocketManager.IsSocketConnected ? "Connected" : "Disconnected");

            if (ImGui.BeginChild("ServerSelector", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                ImGui.InputText("Search", ref CurrentServerSelectorSearch, 200U);
                ImGui.Spacing();

                for (int index = 0; index < SocketServers.Count; index++)
                {
                    SocketServer? SocketServer = SocketServers[index];

                    if (SocketServer == null)
                        continue;

                    bool isProcessingServer = SocketManager.CurrentProcessingSocketServer?.Equals(SocketServer) ?? false;

                    bool flag_name_match = GlobalHelper.RegExpMatch(SocketServer.name, CurrentServerSelectorSearch);
                    bool flag_url_match = GlobalHelper.RegExpMatch(SocketServer.serverURL, CurrentServerSelectorSearch);
                    bool flag_id_match = GlobalHelper.RegExpMatch(SocketServer.unique_id, CurrentServerSelectorSearch);

                    if (flag_name_match || flag_url_match || (SocketServer.name == string.Empty && flag_id_match))
                    {
                        string name = SocketServer.name.Trim() == string.Empty ? SocketServer.unique_id : SocketServer.name.Trim();
                        string id = SocketServer.unique_id;

                        if (ImGui.Selectable((isProcessingServer ? $"[{state}] " : "") + name, id == SelectedServerId))
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
                    ImGui.TextWrapped("Press \"Add\" at the bottom of this window to add an entry to the whitelist.");
                }
                else if (ViewModeServerSelector == "edit")
                {
                    if (SelectedServer != null)
                    {
                        bool isProcessingServer = SocketManager.CurrentProcessingSocketServer?.Equals(SelectedServer) ?? false;

                        ImGui.Text("Editing entry N°" + SelectedServer.unique_id);
                        ImGui.Spacing();

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
                                    if (ImGui.Button("Connect to the Server"))
                                    {
                                        if (Service.ClientState.IsLoggedIn)
                                            _ = SocketManager.InitializeSocket(SelectedServer);
                                    }

                                    if (!Service.ClientState.IsLoggedIn)
                                    {
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

            if (ImGui.Button("Delete"))
            {
                if (SelectedServer != null)
                {
                    Service.Configuration.RemoveSocketServer((SocketServer)SelectedServer);
                    Service.Configuration.Save();
                }

                SelectedServer = null;
                ViewModeServerSelector = "default";
            }

            ImGui.EndChild();
        }
    }
}
