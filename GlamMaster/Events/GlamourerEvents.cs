using GlamMaster.Helpers;
using GlamMaster.Services;
using Glamourer.Api.Helpers;

namespace GlamMaster.Events;

public class GlamourerEvents
{
    public static EventSubscriber? _glamourerInitialized;
    public static EventSubscriber? _glamourerDisposed;

    public static void OnGlamourerInitialized()
    {
        if (!Service.GlamourerIPC_Caller.isGlamourerAvailable)
        {
            GlamLogger.Debug($"Glamourer API Initialized.");

            Service.GlamourerIPC_Caller.isGlamourerAvailable = true;
        }
    }

    public static void OnGlamourerDisposed()
    {
        if (Service.GlamourerIPC_Caller.isGlamourerAvailable)
        {
            GlamLogger.Debug($"Glamourer API Disposed.");

            Service.GlamourerIPC_Caller.isGlamourerAvailable = false;
        }
    }
}
