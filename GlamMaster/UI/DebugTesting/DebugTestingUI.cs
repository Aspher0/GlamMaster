using Dalamud.Interface.Colors;
using GlamMaster.Helpers;
using GlamMaster.Services;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;

namespace GlamMaster.UI.DebugTesting;

public class DebugTestingUI
{
    public static void DrawDebugTestingUI()
    {
        if (ImGui.BeginChild("Debug_Testing_UI##MainUI", new Vector2(0, 0), true))
        {
            if (ImGui.BeginChild("Debug_Testing_UI##MainUI##PenumbraSection", new Vector2(0, Service.PenumbraIPC_Caller.isPenumbraAvailable ? 175f : 60f), true))
            {
                ImGui.TextColored(ImGuiColors.DalamudViolet, "Penumbra");

                if (Service.PenumbraIPC_Caller.isPenumbraAvailable)
                {
                    if (Service.ClientState!.LocalPlayer != null)
                    {
                        if (ImGui.Button("Redraw Local Player"))
                        {
                            Service.PenumbraIPC_Caller.RedrawObject(0);
                        }
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

                    if (ImGui.Button("Get Penumbra API Version"))
                    {
                        var ApiVersion = Service.PenumbraIPC_Caller.ApiVersion();
                        GlamLogger.Debug($"Breaking {ApiVersion.Breaking}, Features {ApiVersion.Features}");
                    }

                    if (Service.ClientState!.LocalPlayer != null)
                    {
                        if (ImGui.Button("Get currently used collection on character"))
                        {
                            var collection = Service.PenumbraIPC_Caller.GetCollectionForObject((int)Service.ClientState.LocalPlayer.DataId);

                            GlamLogger.Debug($"Valid: {collection.ObjectValid}, IndividualSet: {collection.IndividualSet}, Collection: GUID {collection.EffectiveCollection.Id} Name {collection.EffectiveCollection.Name}, NO COLLECTION USED: {collection.EffectiveCollection.Id == Guid.Empty}");
                        }

                        if (ImGui.Button("Try initialize penumbra mods"))
                        {
                            Service.InitializePenumbraModList();
                        }
                    }
                }
                else
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, "Penumbra unavailable, please enable or install it.");
                }

                ImGui.EndChild();
            }

            ImGui.EndChild();
        }
    }
}
