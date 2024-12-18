using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using ECommons;
using GlamMaster.Events;
using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.UI;
using System;
using System.Collections.Generic;

namespace GlamMaster;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    private readonly List<Tuple<string, string>> commandNames = new()
    {
        new Tuple<string, string>("/glamourmaster", "Opens Glamour Master."),
        new Tuple<string, string>("/glammaster", "Alias of /glamourmaster."),
        new Tuple<string, string>("/gmaster", "Alias of /glamourmaster."),
    };

    public readonly WindowSystem WindowSystem = new("GlamMaster");
    private UIBuilder MainWindow { get; init; }

    public Plugin()
    {
        PluginInterface.Create<Service>(Array.Empty<object>());

        ECommonsMain.Init(PluginInterface, this);
        SetupAPIs();

        Service.InitializeService();

        MainWindow = new UIBuilder(this);
        WindowSystem.AddWindow(MainWindow);

        SetupUI();
        SetupCommands();
        EventsManager.RegisterAllEvents();

        if (Service.Configuration!.AutoConnectSocketServer != null && Service.ClientState.IsLoggedIn)
        {
            _ = SocketManager.InitializeSocket(Service.Configuration.AutoConnectSocketServer);
        }

        if (Service.Configuration.OpenPluginOnLoad) MainWindow.IsOpen = true;


    }

    private void SetupAPIs()
    {
        if (IPCHelper.IsPenumbraAPIAvailable())
        {
            PenumbraEvents.OnPenumbraInitialized();
            Service.PenumbraIPC_Caller.isPenumbraAvailable = true;
        }
    }

    private void SetupUI()
    {
        PluginInterface.UiBuilder.Draw += () => WindowSystem.Draw();
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        PluginInterface.UiBuilder.OpenConfigUi += OpenSettings;
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
        string[] splitArgs = args.Split(' ');

        if (splitArgs.Length > 0)
        {
            // For a possible future, not yet planned
            if (splitArgs[0] == "config" || splitArgs[0] == "settings")
            {
                OpenSettings();
                return;
            }
        }

        ToggleMainUI();
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

        ECommonsMain.Dispose();
    }
}
