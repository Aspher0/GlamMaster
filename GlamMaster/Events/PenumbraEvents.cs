using GlamMaster.Helpers;
using GlamMaster.Services;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using System;

namespace GlamMaster.Events;

public class PenumbraEvents
{
    public static EventSubscriber<ModSettingChange, Guid, string, bool>? _modSettingChangedSubscriber;
    public static EventSubscriber<string> _modDeletedSubscriber;
    public static EventSubscriber<string> _modAddedSubscriber;
    public static EventSubscriber<string, string> _modMovedSubscriber;

    public static EventSubscriber? _penumbraInitialized;
    public static EventSubscriber? _penumbraDisposed;

    public static void OnPenumbraInitialized()
    {
        if (!Service.PenumbraIPC_Caller.isPenumbraAvailable)
        {
            GlamLogger.Debug($"Penumbra API Initialized.");

            Service.PenumbraIPC_Caller.isPenumbraAvailable = true;
        }
    }

    public static void OnPenumbraDisposed()
    {
        if (Service.PenumbraIPC_Caller.isPenumbraAvailable)
        {
            GlamLogger.Debug($"Penumbra API Disposed.");

            Service.PenumbraIPC_Caller.isPenumbraAvailable = false;
        }
    }

    public static void OnModSettingChanged(ModSettingChange change, Guid modId, string settingName, bool value)
    {
        if (change != ModSettingChange.TemporaryMod)
            GlamLogger.Debug($"Mod Setting Changed, values: ModSettingChange: {change.ToString()}, ModId: {modId}, Setting: {settingName}, Value: {value}");
    }

    public static void OnModDeleted(string deletedModBaseDirectoryName)
    {
        GlamLogger.Debug($"Mod deleted at {deletedModBaseDirectoryName}");
    }

    public static void OnModAdded(string newModBaseDirectoryName)
    {
        GlamLogger.Debug($"Mod added at {newModBaseDirectoryName}");
    }

    public static void OnModMoved(string previousModBaseDirectoryName, string newModBaseDirectoryName)
    {
        GlamLogger.Debug($"Mod moved from {previousModBaseDirectoryName} to {newModBaseDirectoryName}");
    }
}
