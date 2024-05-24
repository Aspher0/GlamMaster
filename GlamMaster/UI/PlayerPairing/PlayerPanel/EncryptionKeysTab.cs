using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.Socket.EmitEvents;
using GlamMaster.Structs.Payloads;
using GlamMaster.Structs.WhitelistedPlayers;
using ImGuiNET;
using System.Numerics;

namespace GlamMaster.UI.PlayerPairing
{
    internal class EncryptionKeysTab
    {
        public static void DrawEncryptionKeysTab(PairedPlayer SelectedPlayer)
        {
            ImGui.Text("Their encryption key has");
            ImGui.SameLine();

            bool emptyPairedPlayerEncKey = SelectedPlayer.theirSecretEncryptionKey == string.Empty;

            ImGui.TextColored(emptyPairedPlayerEncKey ? ImGuiColors.DalamudRed : ImGuiColors.HealerGreen, emptyPairedPlayerEncKey ? "not been entered" : "been entered");

            float availableWidth = ImGui.GetContentRegionAvail().X;

            if (emptyPairedPlayerEncKey)
            {
                if (ImGui.Button($"Paste {SelectedPlayer.pairedPlayer.playerName}'s encryption key", new Vector2(availableWidth, 0)))
                {
                    string fromClipboard = ImGui.GetClipboardText();

                    if (!string.IsNullOrEmpty(fromClipboard) && fromClipboard.Length == 79 && fromClipboard.Substring(0, 20) == "GLAM_MASTER_ENC_KEY-")
                    {
                        SelectedPlayer.theirSecretEncryptionKey = fromClipboard;
                        Service.Configuration!.Save();
                    } else
                    {
                        GlamLogger.PrintError("The pasted key is not valid or the clipboard is empty.");
                    }
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text($"Please, ask {SelectedPlayer.pairedPlayer.playerName} to send you their secret encryption key and paste it here.");
                    ImGui.EndTooltip();
                }
            }
            else
            {
                string text = $"Reset {SelectedPlayer.pairedPlayer.playerName}'s encryption key";
                bool ctrlPressed = ImGui.GetIO().KeyCtrl;

                if (ctrlPressed)
                {
                    if (ImGui.Button(text, new Vector2(availableWidth, 0)) && ctrlPressed)
                    {
                        SelectedPlayer.theirSecretEncryptionKey = string.Empty;
                        Service.Configuration!.Save();
                    }
                }
                else
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.Alpha, ImGui.GetStyle().Alpha * 0.5f);
                    ImGui.Button(text, new Vector2(availableWidth, 0));
                    ImGui.PopStyleVar();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Hold CTRL to reset.");
                    ImGui.Text($"Clicking this button will reset {SelectedPlayer.pairedPlayer.playerName}'s encryption key on your side.");
                    ImGui.Text("Useful if the other player has regenerated their key.");
                    ImGui.EndTooltip();
                }
            }

            if (ImGui.Button("Copy your secret encryption key", new Vector2(availableWidth, 0)))
            {
                string myEncryptionKey = SelectedPlayer.mySecretEncryptionKey;
                ImGui.SetClipboardText(myEncryptionKey);
            }

            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text($"Click this button to copy your encryption key and send it to {SelectedPlayer.pairedPlayer.playerName}.");
                ImGui.EndTooltip();
            }

            if (ImGui.Button("Send Payload", new Vector2(availableWidth, 0)) && SocketManager.GetClient != null)
            {
                Payload payload = new Payload(PayloadType.PermissionsRequest);

                _ = SocketManager.GetClient.SendPayloadToPlayer(SelectedPlayer, payload);
            }
        }
    }
}
