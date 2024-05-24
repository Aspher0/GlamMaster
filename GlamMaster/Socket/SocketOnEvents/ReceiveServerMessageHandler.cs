using GlamMaster.Helpers;
using GlamMaster.Structs;
using SocketIOClient;
using System;

namespace GlamMaster.Socket.SocketOnEvents
{
    public static class ReceiveServerMessageHandler
    {
        public static void Handle(SocketIOResponse response)
        {
            try
            {
                var data = response.GetValue<ReceiveServerMessageJSON>();

                if (data.Type == "errorMessage")
                {
                    GlamLogger.PrintError("[Server] - " + data.Message, false);
                } else
                {
                    GlamLogger.Print("[Server] - " + data.Message, false);
                }
            }
            catch (Exception ex)
            {
                GlamLogger.Error("Could not parse response from the serverMessage socket Event: " + ex.Message);
            }
        }
    }
}
