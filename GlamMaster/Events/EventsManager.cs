using GlamMaster.Helpers;
using GlamMaster.Structs;
using System;
using System.Collections.Generic;

namespace GlamMaster.Events
{
    internal class EventsManager
    {
        // For modularity in the future
        private static List<List<GlobalEvent>> EventsListsToProcess = new List<List<GlobalEvent>>
        {
            GlobalEventsList.MandatoryEvents
        };

        public static void RegisterAllEvents()
        {
            GlamLogger.Information("Registering Global Events.");

            foreach (var list in EventsListsToProcess)
            {
                RegisterEventsList(list);
            }
        }

        public static void UnregisterAllEvents()
        {
            GlamLogger.Information("Unregistering Global Events.");

            foreach (var list in EventsListsToProcess)
            {
                UnregisterEventsList(list);
            }
        }

        public static void RegisterEventsList(List<GlobalEvent> list)
        {
            foreach (var globalEvent in list)
            {
                try
                {
                    globalEvent.Register.Invoke();
                }
                catch (Exception ex)
                {
                    GlamLogger.Error($"Failed to register event: {ex.Message}");
                }
            }
        }

        public static void UnregisterEventsList(List<GlobalEvent> list)
        {
            foreach (var globalEvent in list)
            {
                try
                {
                    globalEvent.Unregister.Invoke();
                }
                catch (Exception ex)
                {
                    GlamLogger.Error($"Failed to unregister event: {ex.Message}");
                }
            }
        }
    }
}
