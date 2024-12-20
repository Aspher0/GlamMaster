using ImGuiNET;

namespace GlamMaster.UI.Settings;

public class SettingsUI
{
    public static void DrawSettingsUI()
    {
        ImGui.BeginChild("Settings_UI##MainUI");

        if (ImGui.BeginTabBar("SettingsUITabs"))
        {
            if (ImGui.BeginTabItem("General Settings"))
            {
                SettingsGeneralTabUI.DrawGeneralTabUI();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Server List"))
            {
                SettingsServerTabUI.DrawServerTabUI();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

        ImGui.EndChild();
    }
}
