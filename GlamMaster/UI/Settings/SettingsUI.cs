using ImGuiNET;

namespace GlamMaster.UI.Settings
{
    internal class SettingsUI
    {
        public static void DrawSettingsUI()
        {
            ImGui.BeginChild("Settings_UI##MainUI");

            if (ImGui.BeginTabBar("SettingsUITabs"))
            {
                if (ImGui.BeginTabItem("General Settings"))
                {
                    
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
}
