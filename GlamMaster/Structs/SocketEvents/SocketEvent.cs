using SocketIOClient;
using System;

namespace GlamMaster.Structs
{
    public class SocketEvent
    {
        public string Name { get; init; }
        public Action<SocketIOResponse> Handler { get; init; }

        public SocketEvent(string name, Action<SocketIOResponse> handler)
        {
            Name = name;
            Handler = handler;
        }
    }
}
