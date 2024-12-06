using GlamMaster.IPC.Penumbra;
using GlamMaster.Services;
using GlamMaster.Structs.Penumbra;
using ImGuizmoNET;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace GlamMaster.Helpers;

public static class IPCHelper
{
    public static bool IsPenumbraAPIAvailable()
    {
        try
        {
            var ApiVersion = Service.PenumbraIPC_Caller.ApiVersion();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static PenumbraMod MakePenumbraModFromDirectoryAndName(Guid collectionId, KeyValuePair<string, string> mod)
    {
        if (!Service.PenumbraIPC_Caller.isPenumbraAvailable)
            throw new Exception("Penumbra API is not available while retrieving all installed penumbra mods.");

        var directoryName = mod.Key;
        var modName = mod.Value;

        var modFullPathObject = Service.PenumbraIPC_Caller.GetModPath(directoryName, modName);

        if (modFullPathObject.Item1 == PenumbraApiEc.ModMissing)
            throw new Exception($"Mod with directory {directoryName} does not exist while trying to create a PenumbraMod.");

        var fullPath = modFullPathObject.FullPath;
        var isFullPathDefault = modFullPathObject.FullDefault;
        var isSortOrderNameDefault = modFullPathObject.FullDefault;

        var overallChangedItems = Service.PenumbraIPC_Caller.GetChangedItems(directoryName, modName);
        var availableModSettings = Service.PenumbraIPC_Caller.GetAvailableModSettings(directoryName, modName);

        CurrentModSettings currentModSettings;

        currentModSettings = Service.PenumbraIPC_Caller.GetCurrentModSettings(collectionId, directoryName, modName, true);

        return new PenumbraMod(directoryName, modName, fullPath, isFullPathDefault, isSortOrderNameDefault, overallChangedItems, availableModSettings, currentModSettings);
    }

    public static List<PenumbraMod> GetAllInstalledPenumbraMods()
    {
        if (!Service.PenumbraIPC_Caller.isPenumbraAvailable)
            throw new Exception("Penumbra API is not available while retrieving all installed penumbra mods.");

        if (Service.ClientState!.LocalPlayer == null)
            throw new Exception("The player is not logged in while retrieving all installed penumbra mods.");

        var currentlyAppliedCollection = Service.PenumbraIPC_Caller.GetCollectionForObject(Service.ClientState!.LocalPlayer.ObjectIndex);

        if (!currentlyAppliedCollection.ObjectValid)
            throw new Exception("Game object was not valid when retrieving currently used Penumbra collection.");

        List<PenumbraMod> penumbraModList = new List<PenumbraMod>();

        if (currentlyAppliedCollection.EffectiveCollection.Id == Guid.Empty)
            return penumbraModList;

        Dictionary<string, string> modList = Service.PenumbraIPC_Caller.GetModList();

        foreach (var mod in modList)
        {
            try
            {
                PenumbraMod penumbraMod = MakePenumbraModFromDirectoryAndName(currentlyAppliedCollection.EffectiveCollection.Id, mod);
                penumbraModList.Add(penumbraMod);
            }
            catch (Exception ex)
            {
                GlamLogger.Error(ex.Message);
                continue;
            }
        }

        return penumbraModList;
    }
}
