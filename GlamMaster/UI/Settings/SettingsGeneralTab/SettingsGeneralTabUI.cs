using GlamMaster.Services;
using ImGuiNET;

namespace GlamMaster.UI.Settings;

public class SettingsGeneralTabUI
{
    public static void DrawGeneralTabUI()
    {
        ImGui.BeginChild("Settings_UI##GeneralSettingsTab");

        bool openPluginOnLoad = Service.Configuration!.OpenPluginOnLoad;
        if (ImGui.Checkbox("Open the plugin on load", ref openPluginOnLoad))
        {
            Service.Configuration.OpenPluginOnLoad = openPluginOnLoad;
            Service.Configuration.Save();
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Determines if the plugin window should open automatically when it is loaded.");
            ImGui.EndTooltip();
        }

        ImGui.EndChild();
    }
}
