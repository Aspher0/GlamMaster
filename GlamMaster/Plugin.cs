using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using GlamMaster.UI;
using GlamMaster.Services;
using System.Collections.Generic;
using System;
using GlamMaster.Socket;

namespace GlamMaster;

public sealed class Plugin : IDalamudPlugin
{
    private readonly List<Tuple<string, string>> commandNames = new()
    {
        new Tuple<string, string>("/gmaster", "Opens Glamour Master.")
    };

    public readonly WindowSystem WindowSystem = new("GlamMaster");
    private MainWindow MainWindow { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>(Array.Empty<object>());
        Service.InitializeService(this);

        MainWindow = new MainWindow(this);
        WindowSystem.AddWindow(MainWindow);

        SetupUI();
        SetupCommands();
        SetupSocketConnection();
    }
    private void SetupUI()
    {
        Service.PluginInterface.UiBuilder.Draw += () => WindowSystem.Draw();
        Service.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    private void SetupCommands()
    {
        foreach (var CommandName in commandNames)
        {
            Service.CommandManager.AddHandler(CommandName.Item1, new CommandInfo(OnCommand)
            {
                HelpMessage = CommandName.Item2
            });
        }
    }

    private void SetupSocketConnection()
    {
        if (Service.Configuration.AutomaticallyConnectToSocketServer)
        {
            SocketManager.InitializeSocket();
        }
    }

    private void OnCommand(string command, string args)
    {
        if (command == "/gmaster")
        {
            ToggleMainUI();
        }
    }
    public void ToggleMainUI() => MainWindow.Toggle();


    public async void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();
        await SocketManager.DisposeSocket(SocketManager.GetClient, true);

        foreach (var CommandName in commandNames)
        {
            Service.CommandManager.RemoveHandler(CommandName.Item1);
        }
    }
}
