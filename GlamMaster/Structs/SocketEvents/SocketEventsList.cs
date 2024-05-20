using GlamMaster.Socket.SocketOnEvents;
using System.Collections.Generic;

namespace GlamMaster.Structs
{
    public static class SocketEventsList
    {
        public static List<SocketEvent> OnEventsHandlers = new List<SocketEvent>
        {
            new SocketEvent("serverPong", ReceiveServerPongHandler.Handle)
        };
    }
}
