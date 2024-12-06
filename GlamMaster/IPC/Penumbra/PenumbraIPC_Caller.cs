using ECommons.EzIpcManager;
using GlamMaster.Helpers;
using GlamMaster.Structs.Penumbra;
using Penumbra.Api.Enums;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;

namespace GlamMaster.IPC.Penumbra;

public class PenumbraIPC_Caller
{
    public bool isPenumbraAvailable { get; set; } = false;

    public PenumbraIPC_Caller()
    {
        EzIPC.Init(this, "Penumbra");
    }

    #region General

    private readonly ApiVersion penumbraApiVersion = new(Plugin.PluginInterface);

    public (int Breaking, int Features) ApiVersion() => penumbraApiVersion.Invoke();

    public void RedrawObject(int gameObjectIndex)
    {
        try
        {
            new RedrawObject(Plugin.PluginInterface).Invoke(gameObjectIndex);
        }
        catch (Exception e)
        {
            e.LogError();
        }
    }

    #endregion

    #region Mods.cs & ModSettings.cs

    private readonly GetModList penumbraGetModList = new(Plugin.PluginInterface);
    private readonly GetModPath penumbraGetModPath = new(Plugin.PluginInterface);
    private readonly SetModPath penumbraSetModPath = new(Plugin.PluginInterface);

    private readonly InstallMod penumbraInstallMod = new(Plugin.PluginInterface);
    private readonly DeleteMod penumbraDeleteMod = new(Plugin.PluginInterface);

    private readonly AddMod penumbraAddMod = new(Plugin.PluginInterface);
    private readonly ReloadMod penumbraReloadMod = new(Plugin.PluginInterface);

    private readonly GetAvailableModSettings penumbraGetAvailableModSettings = new(Plugin.PluginInterface);
    private readonly GetCurrentModSettings penumbraGetCurrentModSettings = new(Plugin.PluginInterface);
    private readonly TrySetModSetting penumbraTrySetModSetting = new(Plugin.PluginInterface);
    private readonly TrySetModSettings penumbraTrySetModSettings = new(Plugin.PluginInterface);

    private readonly TrySetMod penumbraTrySetMod = new(Plugin.PluginInterface);
    private readonly TrySetModPriority penumbraTrySetModPriority = new(Plugin.PluginInterface);

    private readonly GetChangedItems penumbraGetChangedItems = new(Plugin.PluginInterface);

    public Dictionary<string, string> GetModList() => penumbraGetModList.Invoke();
    public (PenumbraApiEc, string FullPath, bool FullDefault, bool NameDefault) GetModPath(string modDirectory, string modName = "") => penumbraGetModPath.Invoke(modDirectory, modName);
    public PenumbraApiEc SetModPath(string modDirectory, string newPath, string modName = "") => penumbraSetModPath.Invoke(modDirectory, newPath, modName);

    public PenumbraApiEc InstallMod(string modFilePackagePath) => penumbraInstallMod.Invoke(modFilePackagePath);
    public PenumbraApiEc DeleteMod(string modDirectory, string modName = "") => penumbraDeleteMod.Invoke(modDirectory, modName);

    public PenumbraApiEc AddMod(string modDirectory) => penumbraAddMod.Invoke(modDirectory);
    public PenumbraApiEc ReloadMod(string modDirectory, string modName = "") => penumbraReloadMod.Invoke(modDirectory, modName);

    public IReadOnlyDictionary<string, (string[], GroupType)>? GetAvailableModSettings(string modDirectory, string modName = "") => penumbraGetAvailableModSettings.Invoke(modDirectory, modName);
    
    public CurrentModSettings GetCurrentModSettings(Guid collectionId, string modDirectory, string modName = "", bool ignoreInheritance = false)
    {
        var currentModSettings = penumbraGetCurrentModSettings.Invoke(collectionId, modDirectory, modName, ignoreInheritance);

        if (currentModSettings.Item1 == PenumbraApiEc.ModMissing || currentModSettings.Item1 == PenumbraApiEc.CollectionMissing || !currentModSettings.Item2.HasValue)
            throw new Exception($"Error during PenumbraIPC.GetCurrentModSettings, collection: {collectionId}, mod directory: {modDirectory}, mod name: {modName}. Status code: {currentModSettings.Item1}, Has value: {currentModSettings.Item2.HasValue}.");

        var modSettings = currentModSettings.Item2.Value;

        return new CurrentModSettings(modSettings.Item1, modSettings.Item2, modSettings.Item3, modSettings.Item4);
    }

    public PenumbraApiEc TrySetModSetting(Guid collectionId, string modDirectory, string optionGroupName, string optionName, string modName = "") => penumbraTrySetModSetting.Invoke(collectionId, modDirectory, optionGroupName, optionName, modName);
    public PenumbraApiEc TrySetModSettings(Guid collectionId, string modDirectory, string optionGroupName, IReadOnlyList<string> optionNames, string modName = "") => penumbraTrySetModSettings.Invoke(collectionId, modDirectory, optionGroupName, optionNames, modName);

    public PenumbraApiEc TrySetMod(Guid collectionId, string modDirectory, bool enabled, string modName = "") => penumbraTrySetMod.Invoke(collectionId, modDirectory, enabled, modName);
    public PenumbraApiEc TrySetModPriority(Guid collectionId, string modDirectory, int priority, string modName = "") => penumbraTrySetModPriority.Invoke(collectionId, modDirectory, priority, modName);

    public Dictionary<string, object?> GetChangedItems(string modDirectory, string modName = "") => penumbraGetChangedItems.Invoke(modDirectory, modName);

    #endregion

    #region Collection.cs

    private readonly GetCollections penumbraGetCollections = new(Plugin.PluginInterface);
    private readonly GetCollection penumbraGetCollection = new(Plugin.PluginInterface);
    private readonly SetCollection penumbraSetCollection = new(Plugin.PluginInterface);
    private readonly GetCollectionForObject penumbraGetCollectionForObject = new(Plugin.PluginInterface);
    private readonly SetCollectionForObject penumbraSetCollectionForObject = new(Plugin.PluginInterface);

    private readonly GetChangedItemsForCollection penumbraGetChangedItemsForCollection = new(Plugin.PluginInterface);

    public Dictionary<Guid, string> GetCollections() => penumbraGetCollections.Invoke();
    public (Guid id, string Name)? GetCollection(ApiCollectionType type) => penumbraGetCollection.Invoke(type);
    public (PenumbraApiEc, (Guid Id, string Name)? OldCollection) SetCollection(ApiCollectionType type, Guid? collectionId, bool allowCreateNew = true, bool allowDelete = true) => penumbraSetCollection.Invoke(type, collectionId, allowCreateNew, allowDelete);
    public (bool ObjectValid, bool IndividualSet, (Guid Id, string Name) EffectiveCollection) GetCollectionForObject(int gameObjectId) => penumbraGetCollectionForObject.Invoke(gameObjectId);
    public (PenumbraApiEc, (Guid Id, string Name)? OldCollection) SetCollectionForObject(int gameObjectIdx, Guid? collectionId, bool allowCreateNew = true, bool allowDelete = true) => penumbraSetCollectionForObject.Invoke(gameObjectIdx, collectionId, allowCreateNew, allowDelete);

    public Dictionary<string, object?> GetChangedItemsForCollection(Guid collectionId) => penumbraGetChangedItemsForCollection.Invoke(collectionId);

    #endregion
}
