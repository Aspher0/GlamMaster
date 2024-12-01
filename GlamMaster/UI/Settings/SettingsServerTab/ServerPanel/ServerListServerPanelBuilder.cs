using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.Settings;

public class ServerListServerPanelBuilder
{
    public static void DrawServerPanel()
    {
        if (ImGui.BeginChild("Settings_UI##ServerListTab##ServerPanel", new Vector2(0.0f, -ImGui.GetFrameHeightWithSpacing()), true))
        {
            if (ServerSelector.ViewModeServerSelector == "default")
            {
                ImGui.TextWrapped("Select a server or press the button in the bottom left corner to add one.");
            }
            else if (ServerSelector.ViewModeServerSelector == "edit" && ServerSelector.SelectedServer != null)
            {
                if (ImGui.BeginTabBar("ServerPanelSettingsTabs"))
                {
                    if (ImGui.BeginTabItem("General"))
                    {
                        ServerListGeneralTab.DrawServerGeneralTab();
                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }
            }

            ImGui.EndChild();
        }
    }
}
