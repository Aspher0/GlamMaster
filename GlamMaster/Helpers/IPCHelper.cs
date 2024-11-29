using GlamMaster.Services;

namespace GlamMaster.Helpers;

public static class IPCHelper
{
    public static bool IsPenumbraAPIAvailable()
    {
        try
        {
            var ApiVersion = Service.PenumbraIPC_Caller.ApiVersion();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
