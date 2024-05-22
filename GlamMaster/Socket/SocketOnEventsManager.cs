using GlamMaster.Structs;
using System.Collections.Generic;
using System;
using GlamMaster.Helpers;

namespace GlamMaster.Socket
{
    public static class SocketOnEventsManager
    {
        private static readonly Dictionary<string, Action<SocketIOClient.SocketIO>> ConnectionEventHandlers = new Dictionary<string, Action<SocketIOClient.SocketIO>>()
        {
            { nameof(SocketIOClient.SocketIO.OnConnected), client => client.OnConnected += (s, e) => HandleConnectionEvent(client, true, "Connected to the server.", true) },
            { nameof(SocketIOClient.SocketIO.OnDisconnected), client => client.OnDisconnected += (s, e) => HandleConnectionEvent(client, false, "Disconnected from the server.", false) },
            { nameof(SocketIOClient.SocketIO.OnError), client => client.OnError += (s, e) => HandleConnectionEvent(client, false, "Error while connecting to the server.", false) },
            { nameof(SocketIOClient.SocketIO.OnReconnectAttempt), client => client.OnReconnectAttempt += (s, e) => HandleConnectionEvent(client, false, "Attempting to reconnect to the server.", null) },
            { nameof(SocketIOClient.SocketIO.OnReconnected), client => client.OnReconnected += (s, e) => HandleConnectionEvent(client, true, "Successfully reconnected to the server.", true) },
            { nameof(SocketIOClient.SocketIO.OnReconnectError), client => client.OnReconnectError += (s, e) => HandleConnectionEvent(client, false, "Server reconnection attempt failed.", false) },
            { nameof(SocketIOClient.SocketIO.OnReconnectFailed), client => client.OnReconnectFailed += (s, e) => HandleConnectionEvent(client, false, "Failed to reconnect to the server.", false) },
        };

        public static void RegisterAllEvents(SocketIOClient.SocketIO client)
        {
            RegisterConnectionEvents(client);

            foreach (var SocketEvent in SocketEventsList.OnEventsHandlers)
            {
                client.On(SocketEvent.Name, SocketEvent.Handler);
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
                        client.OnConnected -= (s, e) => HandleConnectionEvent(client, true, "Connected to the server.", true);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnDisconnected):
                        client.OnDisconnected -= (s, e) => HandleConnectionEvent(client, false, "Disconnected from the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnError):
                        client.OnError -= (s, e) => HandleConnectionEvent(client, false, "Error while connecting to the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectAttempt):
                        client.OnReconnectAttempt -= (s, e) => HandleConnectionEvent(client, true, "Attempting to reconnect to the server.", null);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnected):
                        client.OnReconnected -= (s, e) => HandleConnectionEvent(client, true, "Successfully reconnected to the server.", true);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectError):
                        client.OnReconnectError -= (s, e) => HandleConnectionEvent(client, false, "Error trying to reconnect to the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectFailed):
                        client.OnReconnectFailed -= (s, e) => HandleConnectionEvent(client, false, "Failed to reconnect to the server.", false);
                        break;
                }
            }
        }

        public static void RegisterEvent(SocketIOClient.SocketIO client, SocketEvent socketEvent)
        {
            client.On(socketEvent.Name, socketEvent.Handler);
        }

        public static void UnregisterEvent(SocketIOClient.SocketIO client, SocketEvent socketEvent)
        {
            client.Off(socketEvent.Name);
        }

        private static void HandleConnectionEvent(SocketIOClient.SocketIO client, bool isConnectEvent, string message, bool? isConnected)
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

                if (SocketManager.IsSocketConnected && SocketManager.GetClient != null)
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
        }
    }
}
