using Dalamud.Interface.Colors;
using ImGuiNET;

namespace GlamMaster.UI.HelpInfos
{
    internal class HelpInfosUI
    {
        public static void DrawHelpUI()
        {
            ImGui.BeginChild("Help_UI##MainUI");

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Help & Infos");

            TabSelector.DrawTabSelector();
            ImGui.SameLine();
            TabPannel.DrawTabPannel();

            ImGui.EndChild();
        }
    }
}
