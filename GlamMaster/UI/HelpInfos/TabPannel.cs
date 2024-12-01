using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.HelpInfos;

public class TabPannel
{
    public static void DrawTabPannel()
    {
        if (ImGui.BeginChild("Help_UI##TabPannel", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), true))
        {
            TabSelector.SelectedTab.CallBack();

            ImGui.EndChild();
        }
    }
}
