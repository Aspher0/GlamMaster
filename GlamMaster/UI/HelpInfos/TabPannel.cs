using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.HelpInfos
{
    internal class TabPannel
    {
        public static void DrawTabPannel()
        {
            if (ImGui.BeginChild("TabPannel", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                TabSelector.SelectedTab.CallBack();

                ImGui.EndChild();
            }
        }
    }
}
