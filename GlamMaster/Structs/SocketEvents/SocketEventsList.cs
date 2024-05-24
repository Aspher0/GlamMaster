using GlamMaster.Socket.SocketOnEvents;
using System.Collections.Generic;

namespace GlamMaster.Structs
{
    /*
     * A list that represents events that the socket can send, associated to their handle
     * Is used to register socket.On() events to the socket upon its creation, and unregister them on disposal with socket.Off()
     * 
     * Used in SocketOnEventsManager.cs
     */

    public static class SocketEventsList
    {
        public static List<SocketEvent> OnEventsHandlers = new List<SocketEvent> // The list of SocketEvent
        {
            new SocketEvent("serverPong", ReceiveServerPongHandler.Handle),
            new SocketEvent("receiveGlobalMessage", ReceiveGlobalMessageHandler.Handle)
        };
    }
}
