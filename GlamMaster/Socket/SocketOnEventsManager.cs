using GlamMaster.Structs;
using System.Collections.Generic;
using System;
using GlamMaster.Helpers;
using GlamMaster.Socket.EmitEvents;
using GlamMaster.Services;

namespace GlamMaster.Socket
{
    /*
     * A class that manages all events related to the socket client.
     * This class is used to register, unregister and handle the socket connections
     * 
     * Mostly used in SocketManager.cs
     */

    public static class SocketOnEventsManager
    {
        // This is a Dictionnary of socket events, linked to a handler
        private static readonly Dictionary<string, Action<SocketIOClient.SocketIO>> ConnectionEventHandlers = new Dictionary<string, Action<SocketIOClient.SocketIO>>()
        {
            { nameof(SocketIOClient.SocketIO.OnConnected), client => client.OnConnected += (s, e) => HandleConnectionEvent(client, "OnConnected", true, "Connected to the server.", true) },
            { nameof(SocketIOClient.SocketIO.OnDisconnected), client => client.OnDisconnected += (s, e) => HandleConnectionEvent(client, "OnDisconnected", false, "Disconnected from the server.", false) },
            { nameof(SocketIOClient.SocketIO.OnError), client => client.OnError += (s, e) => HandleConnectionEvent(client, "OnError", false, "Error while connecting to the server.", false) },
            { nameof(SocketIOClient.SocketIO.OnReconnectAttempt), client => client.OnReconnectAttempt += (s, e) => HandleConnectionEvent(client, "OnReconnectAttempt", false, "Attempting to reconnect to the server.", null) },
            { nameof(SocketIOClient.SocketIO.OnReconnected), client => client.OnReconnected += (s, e) => HandleConnectionEvent(client, "OnReconnected", true, "Successfully reconnected to the server.", true) },
            { nameof(SocketIOClient.SocketIO.OnReconnectError), client => client.OnReconnectError += (s, e) => HandleConnectionEvent(client, "OnReconnectError", false, "Server reconnection attempt failed.", false) },
            { nameof(SocketIOClient.SocketIO.OnReconnectFailed), client => client.OnReconnectFailed += (s, e) => HandleConnectionEvent(client, "OnReconnectFailed", false, "Failed to reconnect to the server.", false) },
        };

        public static void RegisterAllEvents(SocketIOClient.SocketIO client)
        {
            RegisterConnectionEvents(client);

            foreach (var SocketEvent in SocketEventsList.OnEventsHandlers)
            {
                client.On(SocketEvent.eventName, SocketEvent.Handler);
            }
        }

        public static void UnregisterAllEvents(SocketIOClient.SocketIO client)
        {
            UnregisterConnectionEvents(client);

            foreach (var SocketEvent in SocketEventsList.OnEventsHandlers)
            {
                UnregisterEvent(client, SocketEvent);
            }
        }

        private static void RegisterConnectionEvents(SocketIOClient.SocketIO client)
        {
            foreach (var handler in ConnectionEventHandlers)
            {
                handler.Value(client);
            }
        }

        private static void UnregisterConnectionEvents(SocketIOClient.SocketIO client)
        {
            foreach (var handler in ConnectionEventHandlers)
            {
                var eventName = handler.Key;

                switch (eventName)
                {
                    case nameof(SocketIOClient.SocketIO.OnConnected):
                        client.OnConnected -= (s, e) => HandleConnectionEvent(client, "OnConnected", true, "Connected to the server.", true);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnDisconnected):
                        client.OnDisconnected -= (s, e) => HandleConnectionEvent(client, "OnDisconnected", false, "Disconnected from the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnError):
                        client.OnError -= (s, e) => HandleConnectionEvent(client, "OnError", false, "Error while connecting to the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectAttempt):
                        client.OnReconnectAttempt -= (s, e) => HandleConnectionEvent(client, "OnReconnectAttempt", true, "Attempting to reconnect to the server.", null);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnected):
                        client.OnReconnected -= (s, e) => HandleConnectionEvent(client, "OnReconnected", true, "Successfully reconnected to the server.", true);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectError):
                        client.OnReconnectError -= (s, e) => HandleConnectionEvent(client, "OnReconnectError", false, "Error trying to reconnect to the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectFailed):
                        client.OnReconnectFailed -= (s, e) => HandleConnectionEvent(client, "OnReconnectFailed", false, "Failed to reconnect to the server.", false);
                        break;
                }
            }
        }

        public static void RegisterEvent(SocketIOClient.SocketIO client, SocketEvent socketEvent)
        {
            client.On(socketEvent.eventName, socketEvent.Handler);
        }

        public static void UnregisterEvent(SocketIOClient.SocketIO client, SocketEvent socketEvent)
        {
            client.Off(socketEvent.eventName);
        }


        /*
         * Because of missing features in the SocketIOClient library, multiple issues occured and this function below tries to resolve one of them.
         * The main issue solved by this function is the fact that we can not abort a socket's connection in a clean way, the only way to do so being disposing it.
         * But disposing a client does not really aborts the socket's connection, it only partially works and thus, events are being fired even after the client has been disposed.
         * 
         * The function below makes it so that only one client can be connected to a server at a time.
         * If a socket is already connected to the server, clients will automatically be disposed and disconnected from the server.
         */
        private static void HandleConnectionEvent(SocketIOClient.SocketIO client, string socketEvent, bool isConnectEvent, string message, bool? isConnected)
        {
            if (isConnectEvent)
            {
                if (SocketManager.GetClient != client)
                {
                    if (isConnectEvent)
                        GlamLogger.Information("Attempting to connect to the server but the client does not correspond to the last connected client. Disposing this client now.");

                    _ = SocketManager.DisposeSocket(client, false);
                    return;
                }

                if (SocketManager.GetClient != null && SocketManager.IsSocketConnected)
                {
                    if (isConnectEvent)
                        GlamLogger.Information("GlamMaster already has an active server connection. Disposing this client now.");

                    _ = SocketManager.DisposeSocket(client, false);
                    return;
                }
            }

            GlamLogger.Information(message);

            if (SocketManager.GetClient == client && isConnected.HasValue)
                SocketManager.IsSocketConnected = isConnected.Value;

            if ((socketEvent == "OnConnected" || socketEvent == "OnReconnected") && Service.ClientState.IsLoggedIn && Service.ConnectedPlayer != null)
            {
                // Emit connection message to the server, containing the currently logged player name and homeworld
                
                _ = client.SendClientInfos();
            }

            if (socketEvent == "OnDisconnected")
            {
                _ = SocketManager.DisposeSocket(client, SocketManager.GetClient == client);
            }
        }
    }
}
