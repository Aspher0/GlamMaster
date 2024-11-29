using Dalamud.Interface.Colors;
using Dalamud.Plugin.Services;
using ECommons;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.UI.GlamourControl;
using ImGuiNET;
using Penumbra.Api.Enums;
using System.Linq;

namespace GlamMaster.UI.DebugTesting;

internal class DebugTestingUI
{
    public static void DrawDebugTestingUI()
    {
        ImGui.BeginChild("Debug_Testing_UI##MainUI");

        if (ImGui.Button("Redraw Local Player Test"))
        {
            Service.PenumbraIPC_Caller.RedrawLocalPlayer();
        }

        if (ImGui.Button("Get Mod List"))
        {
            var modList = Service.PenumbraIPC_Caller.GetModList();

            GlamLogger.Debug(modList.ToDebugString());
        }

        if (ImGui.Button("Get First Mod Penumbra Path"))
        {
            var modList = Service.PenumbraIPC_Caller.GetModList();
            var modPath = Service.PenumbraIPC_Caller.GetModPath(modList.Keys.First());

            GlamLogger.Debug(modPath.FullPath);
        }

        if (Service.ClientState!.LocalPlayer != null)
        {
            if (ImGui.Button("Get currently used collection on character"))
            {
                var collection = Service.PenumbraIPC_Caller.GetCollectionForObject((int)Service.ClientState.LocalPlayer.DataId);

                GlamLogger.Debug($"Valid: {collection.ObjectValid}, IndividualSet: {collection.IndividualSet}, Collection: GUID {collection.EffectiveCollection.Id} Name {collection.EffectiveCollection.Name}");
            }
        }

        ImGui.EndChild();
    }
}
