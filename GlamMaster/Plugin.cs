using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using GlamMaster.Events;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.UI;
using System;
using System.Collections.Generic;

namespace GlamMaster;

public sealed class Plugin : IDalamudPlugin
{
    private readonly List<Tuple<string, string>> commandNames = new()
    {
        new Tuple<string, string>("/gmaster", "Opens Glamour Master.")
    };

    public readonly WindowSystem WindowSystem = new("GlamMaster");
    private UIBuilder MainWindow { get; init; }

    public Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>(Array.Empty<object>());
        Service.InitializeService(this);

        MainWindow = new UIBuilder(this);
        WindowSystem.AddWindow(MainWindow);

        SetupUI();
        SetupCommands();
        EventsManager.RegisterAllEvents();

        Service.GetConnectedPlayer();

        if (Service.Configuration!.AutoConnectSocketServer != null && Service.ClientState.IsLoggedIn)
        {
            _ = SocketManager.InitializeSocket(Service.Configuration.AutoConnectSocketServer);
        }
    }
    private void SetupUI()
    {
        Service.PluginInterface.UiBuilder.Draw += () => WindowSystem.Draw();
        Service.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += OpenSettings;
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

    private void OnCommand(string command, string args)
    {
        if (command == "/gmaster")
        {
            ToggleMainUI();
        }
    }
    public void ToggleMainUI() => MainWindow.Toggle();
    public void OpenSettings() => MainWindow.Toggle();


    public async void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();
        await SocketManager.DisposeSocket(SocketManager.GetClient, true);
        EventsManager.UnregisterAllEvents();

        Service.Dispose();

        foreach (var CommandName in commandNames)
        {
            Service.CommandManager.RemoveHandler(CommandName.Item1);
        }
    }
}
