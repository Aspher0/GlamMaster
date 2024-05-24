using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs;
using SocketIOClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GlamMaster.Socket
{
    public class SocketManager
    {
        public static SocketServer? CurrentProcessingSocketServer = null;

        public static bool IsSocketConnected;

        private static bool Connecting = false;
        public static bool IsConnecting => Connecting;

        private static SocketIOClient.SocketIO? Client;
        public static SocketIOClient.SocketIO? GetClient => Client;

        private static SemaphoreSlim ConnectionSemaphore = new SemaphoreSlim(1);

        public static async Task InitializeSocket(SocketServer? socketServer)
        {
            if (!Service.ClientState.IsLoggedIn)
            {
                GlamLogger.Error("Cannot connect to the server, you are not logged in.");
                return;
            }

            if (socketServer == null)
            {
                GlamLogger.Error("No server selected.");
                return;
            }

            if (IsSocketConnected || Connecting)
            {
                if (IsSocketConnected)
                {
                    GlamLogger.Information("Client already connected.");
                    GlamLogger.Print("You are already connected to a server.");
                }

                if (Connecting)
                {
                    GlamLogger.Information("Client already trying to connect to a server.");
                    GlamLogger.Print("The socket is already trying to connect to a server.");
                }

                return;
            }

            string serverURL = socketServer.serverURL;

            if (!GlobalHelper.IsValidServerUrl(serverURL))
            {
                GlamLogger.Information("Invalid Server URL.");
                GlamLogger.PrintError("The server you have selected does not have a valid URL.");
                return;
            }

            CurrentProcessingSocketServer = socketServer;

            Connecting = true;

            try
            {
                await ConnectionSemaphore.WaitAsync();

                var options = new SocketIOOptions
                {
                    Reconnection = true,
                    ConnectionTimeout = TimeSpan.FromSeconds(30),
                    ReconnectionAttempts = 0,
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
                    _ = DisposeSocket(Client, true, true);
                }
            }
            finally
            {
                Connecting = false;
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
                IsSocketConnected = false;
                CurrentProcessingSocketServer = null;
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
                    GlamLogger.Print("The socket is not connected to a server.");
                }
            }
            else if (client != null)
            {
                SocketOnEventsManager.UnregisterAllEvents(client);

                if (client.Connected)
                {
                    await client.DisconnectAsync();
                    IsSocketConnected = false;
                }
            }

            client?.Dispose();
            client = null;

            Client = null;
            CurrentProcessingSocketServer = null;
        }

        public static bool IsClientValidAndConnected(SocketIOClient.SocketIO client, bool printMessage = false)
        {
            if (client == null || !client.Connected)
            {
                if (printMessage)
                {
                    GlamLogger.Information("Trying to send a message but the client is not connected to any server.");
                    GlamLogger.PrintError("Not connected to a server. Please, connect to a server in the settings tab.");
                }

                return false;
            }

            return true;
        }
    }
}
