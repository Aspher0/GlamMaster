using ImGuiNET;

namespace GlamMaster.Helpers;

public static class UIHelper
{
    public static void CenterNextText(string text)
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;
        float textWidth = ImGui.CalcTextSize(text).X;
        float textPosX = (availableWidth - textWidth) / 2.0f;
        ImGui.SetCursorPosX(textPosX);
    }
}
