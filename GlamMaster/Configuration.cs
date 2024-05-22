using Dalamud.Configuration;
using Dalamud.Plugin;
using GlamMaster.Structs;
using System;
using System.Collections.Generic;

namespace GlamMaster;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public SocketServer? AutoConnectSocketServer { get; set; } = null;

    public List<SocketServer> SocketServers { get; set; } = new List<SocketServer>();
    public void AddSocketServer(SocketServer socketServer) => SocketServers.Add(socketServer);
    public void RemoveSocketServer(SocketServer socketServer) => SocketServers.Remove(socketServer);

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
