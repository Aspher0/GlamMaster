using ECommons.DalamudServices;
using GlamMaster.Structs;
using System;
using System.Linq;

namespace GlamMaster.Helpers;

public static class PluginInterfaceHelper
{
    public static PluginAvailability IsPluginAvailable(string name, string minVersion = "0.0.0.0")
    {
        var plugin = Svc.PluginInterface.InstalledPlugins.FirstOrDefault(x => x.InternalName == name && x.IsLoaded);

        if (plugin == null)
        {
            return PluginAvailability.NotInstalled;
        }
        else
        {
            if (plugin.Version < Version.Parse(minVersion))
            {
                return PluginAvailability.UnsupportedVersion;
            }
            else
            {
                return PluginAvailability.Available;
            }
        }
    }
}
