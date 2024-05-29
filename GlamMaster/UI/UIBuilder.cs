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
        private bool glamourControlTabOpened = false;

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
            if (ImGui.BeginTabBar("MainWindowTabs"))
            {
                bool glamourControlTabSelected = ImGui.BeginTabItem("Glamour Control");

                if (glamourControlTabSelected && !glamourControlTabOpened)
                {
                    GlamourControlPlayerSelector.CheckAutoRequestPermissions(GlamourControlPlayerSelector.SelectedPlayer);
                    glamourControlTabOpened = true;
                }
                else if (!glamourControlTabSelected && glamourControlTabOpened)
                {
                    glamourControlTabOpened = false;
                }

                if (glamourControlTabSelected)
                {
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
                    SettingsUI.DrawSettingsUI();
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
