using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using GlamMaster.UI.PlayerPairing;
using GlamMaster.UI.Settings;
using GlamMaster.UI.GlamourControl;
using GlamMaster.UI.HelpInfos;

namespace GlamMaster.UI
{
    public class UIBuilder : Window, IDisposable
    {
        public string openTab = string.Empty;

        public UIBuilder(Plugin plugin)
            : base("Glamour Master##GlamMasterMain", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(800, 680),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };
        }

        public void Dispose() { }

        public override void Draw()
        {
            if (ImGui.BeginTabBar("SettingsTabs"))
            {
                if (ImGui.BeginTabItem("Glamour Control"))
                {
                    // Todo : On click on the tab or on click on a player, try to ping the player to check if they are connected on the server
                    // If not, do not display control ui and end there
                    // If yes, display it
                    // Then retrieve their permissions to you maybe ? Or make a button
                    // Other stuff ?

                    GlamourControlUI.DrawGlamourControlUI();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Player Pairing"))
                {
                    PlayerPairingUI.DrawPlayerPairingUI();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Settings"))
                {
                    SettingsUI.DrawSettings();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Help & Infos"))
                {
                    HelpInfosUI.DrawHelpUI();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }
    }
}
