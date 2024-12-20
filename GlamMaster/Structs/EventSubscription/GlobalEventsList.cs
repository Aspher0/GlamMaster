using GlamMaster.Events;
using GlamMaster.Services;
using Penumbra.Api.IpcSubscribers;
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
            () => PenumbraEvents._penumbraInitialized = Initialized.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnPenumbraInitialized),
            () => PenumbraEvents._penumbraInitialized?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._penumbraDisposed = Disposed.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnPenumbraDisposed),
            () => PenumbraEvents._penumbraDisposed?.Dispose()
        ),

        // Penumbra Events via IPC once Initialized
        new GlobalEvent(
            () => PenumbraEvents._modSettingChangedSubscriber = ModSettingChanged.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModSettingChanged),
            () => PenumbraEvents._modSettingChangedSubscriber?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._modDeletedSubscriber = ModDeleted.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModDeleted),
            () => PenumbraEvents._modDeletedSubscriber?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._modAddedSubscriber = ModAdded.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModAdded),
            () => PenumbraEvents._modAddedSubscriber?.Dispose()
        ),
        new GlobalEvent(
            () => PenumbraEvents._modMovedSubscriber = ModMoved.Subscriber(Plugin.PluginInterface, PenumbraEvents.OnModMoved),
            () => PenumbraEvents._modMovedSubscriber?.Dispose()
        ),
    };
}

