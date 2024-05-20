using GlamMaster.Helpers;
using GlamMaster.Structs;

namespace GlamMaster.Events
{
    internal class EventsManager
    {
        public static void RegisterAllEvents()
        {
            GlamLogger.Information("Registering Global Events.");

            foreach (var subscription in GlobalEventsList.Events)
            {
                subscription.Item1.Invoke();
            }
        }

        public static void UnregisterAllEvents()
        {
            GlamLogger.Information("Unregistering Global Events.");

            foreach (var subscription in GlobalEventsList.Events)
            {
                subscription.Item2.Invoke();
            }
        }
    }
}
