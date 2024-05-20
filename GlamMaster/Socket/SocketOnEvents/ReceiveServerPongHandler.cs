using GlamMaster.Helpers;
using GlamMaster.Structs;
using SocketIOClient;
using System;

namespace GlamMaster.Socket.SocketOnEvents
{
    public static class ReceiveServerPongHandler
    {
        public static void Handle(SocketIOResponse response)
        {
            try
            {
                var data = response.GetValue<ReceiveServerPongJSON>();
                GlamLogger.Print(data.Message);
            }
            catch (Exception ex)
            {
                GlamLogger.Print("Could not parse response from the serverPong socket Event: " + ex.Message);
            }
        }
    }
}
