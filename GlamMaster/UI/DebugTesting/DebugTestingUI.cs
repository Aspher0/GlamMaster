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
                    if (Service.ConnectedPlayerObject != null)
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

                    if (ImGui.Button("Get Penumbra Mod install path"))
                    {
                        var PenumbraModsPath = Service.PenumbraIPC_Caller.GetModDirectory();
                        GlamLogger.Debug($"Penumbra Mods Path: {PenumbraModsPath}");
                    }

                    if (ImGui.Button("Get Penumbra Configuration"))
                    {
                        var PenumbraConfig = Service.PenumbraIPC_Caller.GetConfiguration();
                        GlamLogger.Debug($"Penumbra Configuration: {PenumbraConfig}");
                    }

                    if (Service.ConnectedPlayerObject != null)
                    {
                        if (ImGui.Button("Get currently used collection on character"))
                        {
                            var collection = Service.PenumbraIPC_Caller.GetCollectionForObject((int)Service.ConnectedPlayerObject.DataId);

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

            ImGui.TextColored(ImGuiColors.DalamudViolet, "Mods");

            if (ImGui.BeginChild("Debug_Testing_UI##MainUI##ModsSection", new Vector2(0, Service.PenumbraIPC_Caller.isPenumbraAvailable ? -1 : 60f), true))
            {
                if (Service.PenumbraIPC_Caller.isPenumbraAvailable)
                {
                    var sortedModList = Service.PenumbraModList.OrderBy(penumbraMod => penumbraMod.ModName).ToList();
                    foreach (var penumbraMod in sortedModList)
                    {
                        if (ImGui.Button(penumbraMod.ModName))
                        {
                            GlamLogger.Debug($"Enabled: {penumbraMod.CurrentModSettings.Enabled}, Priority: {penumbraMod.CurrentModSettings.Priority}, Inherited: {penumbraMod.CurrentModSettings.Inherited}");

                            GlamLogger.Debug($"{penumbraMod.CurrentModSettings.CurrentOptions.Count}");

                            foreach (var option in penumbraMod.CurrentModSettings.CurrentOptions)
                            {
                                GlamLogger.Debug($"================ Key: {option.Key} ================");

                                foreach (var value in option.Value)
                                {
                                    GlamLogger.Debug($"Value: {value}");
                                }
                            }
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
