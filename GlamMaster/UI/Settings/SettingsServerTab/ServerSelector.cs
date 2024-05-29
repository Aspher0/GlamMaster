using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.Structs;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GlamMaster.UI.Settings
{
    internal class ServerSelector
    {
        public static SocketServer? SelectedServer;
        public static string CurrentServerSelectorSearch = "";
        public static string ViewModeServerSelector = "default";
        public static int CurrentDraggedServerIndex = -1;

        public static void DrawServerSelector()
        {
            ImGui.TextColored(ImGuiColors.DalamudViolet, "Server List");

            string? SelectedPlayerId = SelectedServer?.uniqueID;
            List<SocketServer> SocketServers = Service.Configuration!.SocketServers;

            string state = SocketManager.IsConnecting ? "Connecting" : (SocketManager.IsSocketConnected ? "Connected" : "Disconnected");

            if (ImGui.BeginChild("Settings_UI##ServerListTab##ServerSelector", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
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

                    string name = SocketServer.name.Trim() == string.Empty ? SocketServer.uniqueID : SocketServer.name.Trim();
                    string nameWithWarnings = (isProcessingServer ? $"[{state}] " : "") + name;
                    string id = SocketServer.uniqueID;

                    bool flag_id_match = GlobalHelper.RegExpMatch(id, CurrentServerSelectorSearch);
                    bool flag_name_match = GlobalHelper.RegExpMatch(nameWithWarnings, CurrentServerSelectorSearch);
                    bool flag_url_match = GlobalHelper.RegExpMatch(SocketServer.serverURL, CurrentServerSelectorSearch);

                    if (flag_name_match || flag_url_match || (SocketServer.name == string.Empty && flag_id_match))
                    {
                        if (ImGui.Selectable(nameWithWarnings, id == SelectedPlayerId))
                        {
                            SelectedServer = SocketServer;
                            ViewModeServerSelector = "edit";
                        }

                        HandleDragDrop(name, SocketServers.IndexOf(SocketServer));
                    }
                }

                ImGui.EndChild();
            }
        }

        public static void HandleDragDrop(string DisplayName, int index)
        {
            if (ImGui.BeginDragDropSource())
            {
                CurrentDraggedServerIndex = index;
                ImGui.Text("Dragging: " + DisplayName);

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
                    var temp = Service.Configuration!.SocketServers[CurrentDraggedServerIndex];
                    Service.Configuration!.SocketServers.RemoveAt(CurrentDraggedServerIndex);
                    Service.Configuration!.SocketServers.Insert(index, temp);
                    Service.Configuration.Save();
                    CurrentDraggedServerIndex = -1;
                }

                ImGui.EndDragDropTarget();
            }
        }
    }
}
