using GlamMaster.Events;
using GlamMaster.Services;
using System;
using System.Collections.Generic;

namespace GlamMaster.Structs
{
    /*
     * A list that represents events that the plugin will register and unregister based on in game events such as player login and logout
     * Elements are Tuples with invokable Actions for easy (un)registering
     * 
     * Used in EventsManager.cs
     */

    internal class GlobalEventsList
    {
        public static List<Tuple<Action, Action>> Events = new List<Tuple<Action, Action>> // The list of events
        {
            // Login and Logout game events
            Tuple.Create<Action, Action>(() => Service.ClientState.Login += CharacterLogEvents.OnCharacterLogin, () => Service.ClientState.Login -= CharacterLogEvents.OnCharacterLogin),
            Tuple.Create<Action, Action>(() => Service.ClientState.Logout += CharacterLogEvents.OnCharacterLogout, () => Service.ClientState.Logout -= CharacterLogEvents.OnCharacterLogout)
        };
    }
}
