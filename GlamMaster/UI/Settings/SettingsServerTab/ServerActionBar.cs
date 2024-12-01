using GlamMaster.Services;
using GlamMaster.Structs;
using ImGuiNET;
using System;

namespace GlamMaster.UI.Settings;

public class ServerActionBar
{
    public static void DrawServerActionBar()
    {
        if (ImGui.Button("Add"))
        {
            SocketServer NewSocketServer = new(Guid.NewGuid().ToString());
            Service.Configuration!.AddSocketServer(NewSocketServer);
            Service.Configuration.Save();

            ServerSelector.SelectedServer = NewSocketServer;
            ServerSelector.ViewModeServerSelector = "edit";
        }

        ImGui.SameLine();

        bool ctrlPressed = ImGui.GetIO().KeyCtrl;

        if (ctrlPressed && ServerSelector.SelectedServer != null)
        {
            if (ImGui.Button("Delete"))
            {
                if (Service.Configuration!.AutoConnectSocketServer?.Equals(ServerSelector.SelectedServer) ?? false)
                    Service.Configuration.AutoConnectSocketServer = null;

                Service.Configuration.RemoveSocketServer(ServerSelector.SelectedServer);
                Service.Configuration.Save();

                ServerSelector.SelectedServer = null;
                ServerSelector.ViewModeServerSelector = "default";
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
    }
}
