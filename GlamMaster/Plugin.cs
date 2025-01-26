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

/// <summary>
/// The main plugin class for GlamMaster.
/// </summary>
public sealed class Plugin : IDalamudPlugin
{
    /// <summary>
    /// Injects the plugin interface for Dalamud.
    /// </summary>
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    /// <summary>
    /// List of command names (aliases) and their descriptions.
    /// </summary>
    private readonly List<Tuple<string, string>> commandNames = new()
    {
        new Tuple<string, string>("/glamourmaster", "Opens Glamour Master."),
        new Tuple<string, string>("/glammaster", "Alias of /glamourmaster."),
        new Tuple<string, string>("/gmaster", "Alias of /glamourmaster."),
    };

    public readonly WindowSystem WindowSystem = new("GlamMaster");
    private UIBuilder MainWindow { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
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

    /// <summary>
    /// Sets up the APIs used by the plugin.
    /// </summary>
    private void SetupAPIs()
    {
        if (IPCHelper.IsPenumbraAPIAvailable())
        {
            PenumbraEvents.OnPenumbraInitialized();
            Service.PenumbraIPC_Caller.isPenumbraAvailable = true;
        }
    }

    /// <summary>
    /// Sets up the UI components of the plugin.
    /// </summary>
    private void SetupUI()
    {
        PluginInterface.UiBuilder.Draw += () => WindowSystem.Draw();
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        PluginInterface.UiBuilder.OpenConfigUi += OpenSettings;
    }

    /// <summary>
    /// Sets up the commands for the plugin.
    /// </summary>
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

    /// <summary>
    /// Handles the commands issued to the plugin.
    /// </summary>
    /// <param name="command">The command issued.</param>
    /// <param name="args">The arguments for the command.</param>
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

    /// <summary>
    /// Toggles the main UI of the plugin.
    /// </summary>
    public void ToggleMainUI() => MainWindow.Toggle();
    public void OpenSettings() => MainWindow.Toggle();

    /// <summary>
    /// Disposes the plugin and cleans up resources.
    /// </summary>
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
