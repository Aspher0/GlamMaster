using GlamMaster.Helpers;
using GlamMaster.Structs;
using SocketIOClient;
using System;

namespace GlamMaster.Socket.SocketOnEvents
{
    public static class ReceiveGlobalMessageHandler
    {
        public static void Handle(SocketIOResponse response)
        {
            try
            {
                var data = response.GetValue<ReceiveServerPongJSON>();
                GlamLogger.Print(data.Message, false);
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Could not parse response from the serverPong socket Event: " + ex.Message);
            }
        }
    }
}
