using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using static System.Net.WebRequestMethods;

namespace GlamMaster;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool AutomaticallyConnectToSocketServer { get; set; } = false;
    public string SocketServerURL { get; set; } = "http://localhost:3000";

    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        PluginInterface = pluginInterface;
    }

    public void Save()
    {
        PluginInterface!.SavePluginConfig(this);
    }
}
