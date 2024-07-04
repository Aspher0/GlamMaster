using Dalamud.Configuration;
using GlamMaster.Services;
using GlamMaster.Structs;
using GlamMaster.Structs.WhitelistedPlayers;
using System;
using System.Collections.Generic;

namespace GlamMaster;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public SocketServer? AutoConnectSocketServer { get; set; } = null; // The socket server to auto connect to on startup or on player login

    public List<SocketServer> SocketServers { get; set; } = new List<SocketServer>(); // The configured servers as seen in the settings tab
    public void AddSocketServer(SocketServer socketServer) => SocketServers.Add(socketServer);
    public void RemoveSocketServer(SocketServer socketServer) => SocketServers.Remove(socketServer);

    public List<PairedPlayer> PairedPlayers { get; set; } = new List<PairedPlayer>(); // The paired players as seen in the player pairing tab
    public void AddPairedPlayer(PairedPlayer pairedPlayer) => PairedPlayers.Add(pairedPlayer);
    public void RemovePairedPlayer(PairedPlayer pairedPlayer) => PairedPlayers.Remove(pairedPlayer);

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
