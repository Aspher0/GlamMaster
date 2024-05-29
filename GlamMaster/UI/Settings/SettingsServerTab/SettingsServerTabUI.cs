using ImGuiNET;

namespace GlamMaster.UI.Settings
{
    internal class SettingsServerTabUI
    {
        public static void DrawServerTabUI()
        {
            ImGui.BeginChild("Settings_UI##ServerListTab");

            ServerSelector.DrawServerSelector();
            ImGui.SameLine();
            ServerListServerPanelBuilder.DrawServerPanel();
            ServerActionBar.DrawServerActionBar();

            ImGui.EndChild();
        }
    }
}
