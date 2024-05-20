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
            { nameof(SocketIOClient.SocketIO.OnConnected), client => client.OnConnected += (s, e) => HandleConnectionEvent("Connected to the server.", true) },
            { nameof(SocketIOClient.SocketIO.OnDisconnected), client => client.OnDisconnected += (s, e) => HandleConnectionEvent("Disconnected from the server.", false) },
            { nameof(SocketIOClient.SocketIO.OnError), client => client.OnError += (s, e) => HandleConnectionEvent("Error while connecting to the server.", false) },
            { nameof(SocketIOClient.SocketIO.OnReconnectAttempt), client => client.OnReconnectAttempt += (s, e) => HandleConnectionEvent("Attempting to reconnect to the server.", null) },
            { nameof(SocketIOClient.SocketIO.OnReconnected), client => client.OnReconnected += (s, e) => HandleConnectionEvent("Successfully reconnected to the server.", true) },
            { nameof(SocketIOClient.SocketIO.OnReconnectError), client => client.OnReconnectError += (s, e) => HandleConnectionEvent("Server reconnection attempt failed.", false) },
            { nameof(SocketIOClient.SocketIO.OnReconnectFailed), client => client.OnReconnectFailed += (s, e) => HandleConnectionEvent("Failed to reconnect to the server.", false) },
        };

        public static void RegisterAllEvents(SocketIOClient.SocketIO client)
        {
            GlamLogger.Information("Registering all Socket Events.");
            GlamLogger.Print("Registering all Socket Events.");

            RegisterConnectionEvents(client);

            foreach (var SocketEvent in SocketEventsList.OnEventsHandlers)
            {
                client.On(SocketEvent.Name, SocketEvent.Handler);
            }
        }

        public static void UnregisterAllEvents(SocketIOClient.SocketIO client)
        {
            GlamLogger.Information("Unregistering all Socket Events.");
            GlamLogger.Print("Unregistering all Socket Events.");

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
                        client.OnConnected -= (s, e) => HandleConnectionEvent("Connected to the server.", true);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnDisconnected):
                        client.OnDisconnected -= (s, e) => HandleConnectionEvent("Disconnected from the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnError):
                        client.OnError -= (s, e) => HandleConnectionEvent("Error while connecting to the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectAttempt):
                        client.OnReconnectAttempt -= (s, e) => HandleConnectionEvent("Attempting to reconnect to the server.", null);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnected):
                        client.OnReconnected -= (s, e) => HandleConnectionEvent("Successfully reconnected to the server.", true);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectError):
                        client.OnReconnectError -= (s, e) => HandleConnectionEvent("Error trying to reconnect to the server.", false);
                        break;
                    case nameof(SocketIOClient.SocketIO.OnReconnectFailed):
                        client.OnReconnectFailed -= (s, e) => HandleConnectionEvent("Failed to reconnect to the server.", false);
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

        private static void HandleConnectionEvent(string message, bool? isConnected)
        {
            if (SocketManager.IsSocketConnected)
                return;

            GlamLogger.Print(message);

            if (isConnected.HasValue)
                SocketManager.setSocketConnected(isConnected.Value);
        }
    }
}
