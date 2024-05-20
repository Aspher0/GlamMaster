using GlamMaster.Helpers;
using GlamMaster.Services;
using SocketIOClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GlamMaster.Socket
{
    public class SocketManager
    {
        private static bool SocketConnected;
        public static bool IsSocketConnected => SocketConnected;
        public static bool setSocketConnected(bool connected) => SocketConnected = connected;

        private static bool Connecting = false;
        public static bool IsConnecting => Connecting;

        private static SemaphoreSlim ConnectionSemaphore = new SemaphoreSlim(1);

        private static SocketIOClient.SocketIO? Client;
        public static SocketIOClient.SocketIO? GetClient => Client;

        public static async Task InitializeSocket()
        {
            if (SocketConnected || Connecting)
            {
                if (SocketConnected)
                    GlamLogger.Print("You are already connected to the server.");

                if (Connecting)
                    GlamLogger.Print("The socket is already trying to connect.");

                return;
            }

            Connecting = true;

            try
            {
                await ConnectionSemaphore.WaitAsync();

                string serverURL = Service.Configuration.SocketServerURL;

                if (string.IsNullOrEmpty(serverURL))
                {
                    GlamLogger.Print("You have not selected a server in the configuration. Please go to the settings and input a Server URL.");
                    return;
                }

                var options = new SocketIOOptions
                {
                    Reconnection = true,
                    ConnectionTimeout = TimeSpan.FromSeconds(5),
                    ReconnectionAttempts = 0,
                    ReconnectionDelay = 5,
                    ReconnectionDelayMax = 5,
                };

                Client = new SocketIOClient.SocketIO(serverURL, options);

                SocketOnEventsManager.RegisterAllEvents(Client);

                GlamLogger.Print("Attempting to connect to the server " + serverURL);

                try
                {
                    await Client.ConnectAsync();
                }
                catch (Exception ex)
                {
                    GlamLogger.Print("Failed to connect to the server: " + ex.Message);
                }
                finally
                {
                    Connecting = false;
                }
            }
            finally
            {
                ConnectionSemaphore.Release();
            }
        }

        public static async Task DisposeSocket(SocketIOClient.SocketIO? client, bool printMessages = false)
        {
            Client = null;
            SocketConnected = false;

            if (printMessages)
                GlamLogger.Print("Aborting the connection");

            if (client != null)
            {
                SocketOnEventsManager.UnregisterAllEvents(client);

                if (client.Connected)
                {
                    await client.DisconnectAsync();
                }
            }
            else
            {
                if (printMessages)
                    GlamLogger.Print("The socket you are trying to dispose is null");
            }

            client?.Dispose();
            client = null;
        }

        public static async Task DisconnectSocket(SocketIOClient.SocketIO? client, bool printMessages = false)
        {
            if (client != null)
                SocketOnEventsManager.UnregisterAllEvents(client);

            if (client == null || !client.Connected)
            {
                if (printMessages)
                    GlamLogger.Print("The socket is not connected to the server.");

                if (client == null)
                    GlamLogger.Print("The socket is null !");
            }
            else if (client != null && client.Connected)
            {
                if (printMessages)
                    GlamLogger.Print("Disconnecting from the server.");

                GlamLogger.Print("Global client id: " + Client?.Id ?? "null");
                GlamLogger.Print("client id: " + client.Id);

                await client.DisconnectAsync();
                SocketConnected = false;
            }

            client?.Dispose();
            client = null;

            Client = null;
        }

        public static bool IsClientValidAndConnected(SocketIOClient.SocketIO client, bool printMessage = false)
        {
            if (client == null || !client.Connected)
            {
                if (printMessage)
                    GlamLogger.Print("Not connected to the server. Please, connect to the server by going to the settings and clicking the connect to server button.");

                return false;
            }

            return true;
        }
    }
}
