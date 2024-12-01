using System;

namespace GlamMaster.Structs;

/*
 * A class representing a set of events to register or unregister, with two Actions.
 * 
 * Used in GlobalEventsList.cs 
 */

public class GlobalEvent
{
    public Action Register { get; }
    public Action Unregister { get; }

    public GlobalEvent(Action register, Action unregister)
    {
        Register = register;
        Unregister = unregister;
    }
}
