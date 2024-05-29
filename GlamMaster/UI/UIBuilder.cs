using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using GlamMaster.UI.PlayerPairing;
using GlamMaster.UI.Settings;
using GlamMaster.UI.GlamourControl;
using GlamMaster.UI.HelpInfos;
using GlamMaster.Structs.WhitelistedPlayers;

namespace GlamMaster.UI
{
    public class UIBuilder : Window, IDisposable
    {
        private bool GlamourControlTabOpened = false;

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
            if (ImGui.BeginTabBar("MainTabs"))
            {
                bool glamourControlTabSelected = ImGui.BeginTabItem("Glamour Control");

                if (glamourControlTabSelected && !GlamourControlTabOpened)
                {
                    CheckAutoRequestPermissions(GlamourControlPlayerSelector.SelectedPlayer);
                    GlamourControlTabOpened = true;
                }
                else if (!glamourControlTabSelected && GlamourControlTabOpened)
                {
                    GlamourControlTabOpened = false;
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

        public static void CheckAutoRequestPermissions(PairedPlayer? SelectedPlayer)
        {
            /* Check if selected player != null
             * If ok, check if automatically request permissions = true
             * If not, do nothing
             * If yes, start a loop where every 5 seconds, it will send a request permissions infos
             * Also, request selected user's infos directly when tab is clicked
             * 
             * Do the same process on click on a user in the player pannel list
             */

            if (SelectedPlayer != null && SelectedPlayer.requestTheirPermissionsAutomatically)
            {
                // Start 5 seconds loop

                SelectedPlayer.RequestTheirPermissions();
            }
        }
    }
}
