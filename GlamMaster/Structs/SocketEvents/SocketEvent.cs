using SocketIOClient;
using System;

namespace GlamMaster.Structs;

/*
 * A class that represents a socket event
 * Has an event name which corresponds to an "endpoint" the server can send messages to
 * Also has a handler which corresponds to the function the plugin will call whenever the socket receives data from the server on the eventName endpoint
 * 
 * Used in SocketEventsList.cs
 */

public class SocketEvent
{
    public string eventName { get; init; } // The "endpoint" the server can send messages to
    public Action<SocketIOResponse> Handler { get; init; } // The function the plugin will call whenever the socket receives data from the server on the eventName endpoint

    public SocketEvent(string name, Action<SocketIOResponse> handler)
    {
        eventName = name;
        Handler = handler;
    }
}
