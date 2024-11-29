using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using System;

namespace GlamMaster.Events;

internal class PenumbraEvents
{
    public static EventSubscriber<ModSettingChange, Guid, string, bool>? _modSettingChangedSubscriber;
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
        GlamLogger.Debug($"Mod Setting Changed, values: ModSettingChange: {change.ToString()}, ModId: {modId}, Setting: {settingName}, Value: {value}");
    }
}
