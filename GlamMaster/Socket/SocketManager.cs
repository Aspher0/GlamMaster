using Dalamud.Game.Text;
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
                {
                    GlamLogger.Information("Client already connected.");
                    GlamLogger.Print("You are already connected to the server.");
                }

                if (Connecting)
                {
                    GlamLogger.Information("Client already trying to connect to the server.");
                    GlamLogger.Print("The socket is already trying to connect.");
                }

                return;
            }

            string serverURL = Service.Configuration.SocketServerURL;

            if (string.IsNullOrEmpty(serverURL))
            {
                GlamLogger.Information("No Server URL specified.");
                GlamLogger.PrintError("You have not selected a server in the configuration. Please go to the settings and input a Server URL.");
                return;
            }

            Connecting = true;

            try
            {
                await ConnectionSemaphore.WaitAsync();

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

                GlamLogger.Information("Attempting to connect to the server " + serverURL);

                try
                {
                    await Client.ConnectAsync();
                }
                catch (Exception ex)
                {
                    GlamLogger.Error("Failed to connect to the server: " + ex.Message);
                    GlamLogger.PrintError("Failed to connect to the server.");
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

        public static void AbortSocketConnection(SocketIOClient.SocketIO? client)
        {
            GlamLogger.Information("Aborting the connection to the server.");

            _ = DisposeSocket(client, true, true);
        }

        public static async Task DisposeSocket(SocketIOClient.SocketIO? client, bool resetVariables, bool printMessages = false)
        {
            if (resetVariables)
            {
                Client = null;
                SocketConnected = false;
            }

            if (client != null)
            {
                SocketOnEventsManager.UnregisterAllEvents(client);

                if (client.Connected)
                {
                    await client.DisconnectAsync();
                }
            }

            client?.Dispose();
            client = null;
        }

        public static async Task DisconnectSocket(SocketIOClient.SocketIO? client, bool printMessages = false)
        {
            if (client == null || !client.Connected)
            {
                if (printMessages)
                {
                    GlamLogger.Information("Attempting to disconnect a client that isn't connected.");
                    GlamLogger.Print("The socket is not connected to the server.");
                }
            }
            else if (client != null)
            {
                SocketOnEventsManager.UnregisterAllEvents(client);

                if (client.Connected)
                {
                    await client.DisconnectAsync();
                    SocketConnected = false;
                }
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
                {
                    GlamLogger.Information("Trying to send a message to the server but the client is not connected to the server.");
                    GlamLogger.PrintError("Not connected to the server. Please, connect to the server by going to the settings and clicking the connect to server button.");
                }

                return false;
            }

            return true;
        }
    }
}
