using GlamMaster.Events;
using GlamMaster.Services;
using GlamourerIPCEvents = Glamourer.Api.IpcSubscribers;
using PenumbraIPCEvents = Penumbra.Api.IpcSubscribers;
using System.Collections.Generic;

namespace GlamMaster.Structs;

/*
 * A list that represents events that the plugin will register and unregister based on in game events such as player login and logout, penumbra events etc
 * Elements are of type "GlobalEvent" with invokable Actions for easy (un)registering
 * 
 * Used in EventsManager.cs
 */

internal class GlobalEventsList
{
    public static List<GlobalEvent> MandatoryEvents = new List<GlobalEvent> // The list of events
    {
        // Login and Logout game events
        new GlobalEvent(
            () => Service.ClientState.Login += CharacterLogEvents.OnCharacterLogin,
            () => Service.ClientState.Login -= CharacterLogEvents.OnCharacterLogin
        ),
        new GlobalEvent(
            () => Service.ClientState.Logout += CharacterLogEvents.OnCharacterLogout,
            () => Service.ClientState.Logout -= CharacterLogEvents.OnCharacterLogout
        ),

        // Penumbra Initialization/Dispose Events via IPC
        new GlobalEvent(
            () => PenumbraEvents._penumbraInitialized = PenumbraIPCEvents.Initialized.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnPenumbraInitialized),
            () => PenumbraEvents._penumbraInitialized?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._penumbraDisposed = PenumbraIPCEvents.Disposed.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnPenumbraDisposed),
            () => PenumbraEvents._penumbraDisposed?.Dispose()
        ),

        // Penumbra Events via IPC once Initialized
        new GlobalEvent(
            () => PenumbraEvents._modSettingChangedSubscriber = PenumbraIPCEvents.ModSettingChanged.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModSettingChanged),
            () => PenumbraEvents._modSettingChangedSubscriber?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._modDeletedSubscriber = PenumbraIPCEvents.ModDeleted.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModDeleted),
            () => PenumbraEvents._modDeletedSubscriber?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._modAddedSubscriber = PenumbraIPCEvents.ModAdded.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModAdded),
            () => PenumbraEvents._modAddedSubscriber?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._modMovedSubscriber = PenumbraIPCEvents.ModMoved.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModMoved),
            () => PenumbraEvents._modMovedSubscriber?.Dispose()
        ),

        // Glamourer Initialization/Dispose Events via IPC
        new GlobalEvent(
            () => GlamourerEvents._glamourerInitialized = GlamourerIPCEvents.Initialized.Subscriber(Plugin.PluginInterface, GlamourerEvents.OnGlamourerInitialized),
            () => GlamourerEvents._glamourerInitialized?.Dispose()
        ),
        new GlobalEvent(
            () => GlamourerEvents._glamourerDisposed = GlamourerIPCEvents.Disposed.Subscriber(Plugin.PluginInterface, GlamourerEvents.OnGlamourerDisposed),
            () => GlamourerEvents._glamourerDisposed?.Dispose()
        ),
    };
}

