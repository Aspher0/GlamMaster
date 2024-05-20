using System;
using System.Collections.Generic;
using GlamMaster.Events;
using GlamMaster.Services;

namespace GlamMaster.Structs
{
    internal class GlobalEventsList
    {
        public static List<Tuple<Action, Action>> Events = new List<Tuple<Action, Action>>
        {
            Tuple.Create<Action, Action>(() => Service.ClientState.Login += CharacterLogEvents.OnCharacterLogin, () => Service.ClientState.Login -= CharacterLogEvents.OnCharacterLogin),
            Tuple.Create<Action, Action>(() => Service.ClientState.Logout += CharacterLogEvents.OnCharacterLogout, () => Service.ClientState.Logout -= CharacterLogEvents.OnCharacterLogout)
        };
    }
}
