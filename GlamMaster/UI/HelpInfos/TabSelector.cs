using GlamMaster.Structs;
using ImGuiNET;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace GlamMaster.UI.HelpInfos
{
    internal class TabSelector
    {
        public static HelpUITab SelectedTab = HelpUITabsList.SelectableTabs.ElementAt(0);

        public static void DrawTabSelector(bool displayDisabledText = true)
        {
            List<HelpUITab> selectableTabs = HelpUITabsList.SelectableTabs;

            if (ImGui.BeginChild("Help_UI##HelpTabSelector", new Vector2(225f, -ImGui.GetFrameHeightWithSpacing()), true))
            {
                foreach (var tab in selectableTabs)
                {
                    if (ImGui.Selectable(tab.TabName, tab == SelectedTab))
                    {
                        SelectedTab = tab;
                    }
                }

                ImGui.EndChild();
            }
        }
    }
}
