using ECommons.EzIpcManager;
using GlamMaster.Helpers;
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

    private readonly ApiVersion penumbraApiVersion = new(Plugin.PluginInterface);
    public (int Breaking, int Features) ApiVersion()
    {
        return penumbraApiVersion.Invoke();
    }

    private readonly GetCollection penumbraGetCollection = new(Plugin.PluginInterface);
    public (Guid id, string Name)? GetCollection(ApiCollectionType type)
    {
        return penumbraGetCollection.Invoke(type);
    }

    private readonly GetCollectionForObject penumbraGetCollectionForObject = new(Plugin.PluginInterface);
    public (bool ObjectValid, bool IndividualSet, (Guid Id, string Name) EffectiveCollection) GetCollectionForObject(int gameObjectId)
    {
        return penumbraGetCollectionForObject.Invoke(gameObjectId);
    }

    private readonly GetModList penumbraGetModList = new(Plugin.PluginInterface);
    public Dictionary<string, string> GetModList() => penumbraGetModList.Invoke();

    private readonly GetModPath penumbraGetModPath = new(Plugin.PluginInterface);
    public (PenumbraApiEc, string FullPath, bool FullDefault, bool NameDefault) GetModPath(string modDirectory, string modName = "") => penumbraGetModPath.Invoke(modDirectory, modName);

    public void RedrawPlayer(int playerGameObjectIndex)
    {
        try
        {
            new RedrawObject(Plugin.PluginInterface).Invoke(playerGameObjectIndex);
        }
        catch (Exception e)
        {
            e.LogError();
        }
    }
}
