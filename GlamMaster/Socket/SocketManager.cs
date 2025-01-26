using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Structs;
using SocketIO.Serializer.SystemTextJson;
using SocketIOClient;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GlamMaster.Socket;

public static class SocketManager
{
    /// <summary>
    /// The current socket server being processed.
    /// It means it is either connecting or connected to this server.
    /// </summary>
    public static SocketServer? CurrentProcessingSocketServer = null;

    /// <summary>
    /// Indicates whether the socket is connected.
    /// </summary>
    public static bool IsSocketConnected;

    /// <summary>
    /// Event triggered when the connection state changes.
    /// </summary>
    public static event EventHandler<bool>? IsConnectingChanged;

    private static bool Connecting = false;

    /// <summary>
    /// Indicates whether the socket is in the process of connecting.
    /// </summary>
    public static bool IsConnecting
    {
        get => Connecting;
        private set
        {
            if (Connecting != value)
            {
                Connecting = value;
                IsConnectingChanged?.Invoke(null, Connecting);
            }
        }
    }

    private static SocketIOClient.SocketIO? Client;

    /// <summary>
    /// Gets the current socket client.
    /// </summary>
    public static SocketIOClient.SocketIO? GetClient => Client;

    private static SemaphoreSlim ConnectionSemaphore = new SemaphoreSlim(1);

    private static CancellationTokenSource? CancellationTokenSource;

    public static CancellationTokenSource? GetCancellationTokenSource => CancellationTokenSource;

    /// <summary>
    /// Initializes the socket connection to the specified server.
    /// </summary>
    /// <param name="socketServer">The socket server to connect to.</param>
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

            if (IsConnecting)
            {
                GlamLogger.Information("Client already trying to connect to a server.");
                GlamLogger.Print("The socket is already trying to connect to a server.");
            }

            return;
        }

        string serverURL = socketServer.serverURL;

        if (!CommonHelper.IsValidServerUrl(serverURL))
        {
            GlamLogger.Information("Invalid Server URL.");
            GlamLogger.PrintErrorChannel("The server you have selected does not have a valid URL.");
            return;
        }

        CurrentProcessingSocketServer = socketServer;
        IsConnecting = true;

        CancellationTokenSource?.Cancel();
        CancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = CancellationTokenSource.Token;

        try
        {
            await ConnectionSemaphore.WaitAsync(cancellationToken);

            var options = new SocketIOOptions
            {
                Reconnection = true,
                ConnectionTimeout = TimeSpan.FromSeconds(30),
                ReconnectionAttempts = 0,
            };

            Client = new SocketIOClient.SocketIO(serverURL, options);
            Client.Serializer = new SystemTextJsonSerializer(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            SocketOnEventsManager.RegisterAllEvents(Client);

            GlamLogger.Information("Attempting to connect to the server " + serverURL);

            try
            {
                await Client.ConnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    GlamLogger.Information("Connection attempt was canceled.");
                }
                else
                {
                    GlamLogger.Error("Failed to connect to the server: " + ex.Message);
                    GlamLogger.PrintErrorChannel("Failed to connect to the server.");
                }

                _ = DisposeSocket(Client, true, true);
            }
        }
        finally
        {
            IsConnecting = false;
            ConnectionSemaphore.Release();
        }
    }

    /// <summary>
    /// Aborts the socket connection.
    /// </summary>
    /// <param name="client">The socket client to abort the connection for.</param>
    public static async Task AbortSocketConnection(SocketIOClient.SocketIO? client)
    {
        GlamLogger.Information("Aborting the connection to the server.");
        CancellationTokenSource?.Cancel();
        await DisposeSocket(client, true, true);
    }

    /// <summary>
    /// Disposes the socket connection.
    /// </summary>
    /// <param name="client">The socket client to dispose.</param>
    /// <param name="resetVariables">Indicates whether to reset the SocketManager class variables.</param>
    /// <param name="printMessages">Indicates whether to print messages.</param>
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

    /// <summary>
    /// Disconnects the socket connection.
    /// </summary>
    /// <param name="client">The socket client to disconnect.</param>
    /// <param name="printMessages">Indicates whether to print messages.</param>
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

    /// <summary>
    /// Checks if the client is valid and connected.
    /// </summary>
    /// <param name="client">The socket client to check.</param>
    /// <param name="printMessage">Indicates whether to print messages.</param>
    /// <returns>True if the client is valid and connected, otherwise false.</returns>
    public static bool IsClientValidAndConnected(SocketIOClient.SocketIO client, bool printMessage = false)
    {
        if (client == null || !client.Connected)
        {
            if (printMessage)
            {
                GlamLogger.Information("Trying to send a message but the client is not connected to any server.");
                GlamLogger.PrintErrorChannel("Not connected to a server. Please, connect to a server in the settings tab.");
            }

            return false;
        }

        return true;
    }
}
