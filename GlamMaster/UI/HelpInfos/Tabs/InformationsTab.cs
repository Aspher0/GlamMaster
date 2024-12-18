using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.HelpInfos;

public class InformationsTab
{
    public static void DrawInformations()
    {
        float availableWidth = ImGui.GetContentRegionAvail().X;
        float windowHeight = ImGui.GetContentRegionAvail().Y;

        string text = "Made with ♥ by Aspher0";
        UIHelper.CenterNextText(text);
        ImGui.TextColored(ImGuiColors.ParsedPink, text);

        text = "Thank you very much for using my plugin, I hope you will enjoy it and have lots of fun !";
        UIHelper.CenterNextText(text);
        ImGui.TextWrapped(text);

        float buttonHeight = ImGui.GetFrameHeightWithSpacing() * 2;
        float spacing = 0.0f;

        ImGui.SetCursorPosY(windowHeight - buttonHeight - spacing);

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.7f, 0f, 0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1.0f, 0f, 0f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.5f, 0f, 0f, 1.0f));

        if (ImGui.Button("Report a bug", new Vector2(availableWidth, 0)))
        {
            CommonHelper.OpenUrl("https://github.com/Aspher0/GlamMaster/issues");
        }

        ImGui.PopStyleColor(3);

        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1.0f, 0.4f, 0.7f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(1.0f, 0.5f, 0.8f, 1.0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(1.0f, 0.3f, 0.6f, 1.0f));

        if (ImGui.Button("Support me ♥", new Vector2(availableWidth, 0)))
        {
            CommonHelper.OpenUrl("https://github.com/Aspher0/GlamMaster");
        }

        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Thank you so much !");
            ImGui.EndTooltip();
        }

        ImGui.PopStyleColor(3);
    }
}
