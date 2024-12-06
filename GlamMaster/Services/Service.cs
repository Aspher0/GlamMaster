using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using GlamMaster.Helpers;
using GlamMaster.IPC.Glamourer;
using GlamMaster.IPC.Penumbra;
using GlamMaster.Structs;
using GlamMaster.Structs.Penumbra;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace GlamMaster.Services;

public class Service
{
    [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static IPluginLog Logger { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
    [PluginService] public static ITargetManager TargetManager { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;
    [PluginService] public static IObjectTable Objects { get; private set; } = null!;
    [PluginService] public static ICondition Condition { get; private set; } = null!;
    [PluginService] public static IGameConfig GameConfig { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static IAddonLifecycle AddonLifecycle { get; private set; } = null!;

    public static Configuration? Configuration { get; set; }
    public static Player? ConnectedPlayer { get; set; }
    public static PenumbraIPC_Caller PenumbraIPC_Caller = new();
    public static GlamourerIPC_Caller GlamourerIPC_Caller = new();
    public static List<PenumbraMod> PenumbraModList = new List<PenumbraMod>();
    public static bool IsInitializingPenumbraModList { get; private set; } = false;

    public static void InitializeService()
    {
        InitializeConfig();
        GetConnectedPlayer();
        InitializePenumbraModList();
    }

    public static void InitializeConfig()
    {
        Configuration = Plugin.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Save();
    }

    public static void GetConnectedPlayer()
    {
        IPlayerCharacter? playerCharacter = ClientState!.LocalPlayer;

        if (playerCharacter != null)
        {
            string playerName = playerCharacter.Name.ToString();
            World? playerHomeworld = DataManager.GetExcelSheet<World>()?.GetRow(playerCharacter.HomeWorld.RowId);

            if (playerHomeworld != null)
            {
                ConnectedPlayer = new Player(playerName, playerHomeworld.Value.Name.ToString());
            }
        }
    }

    public static void ClearConnectedPlayer()
    {
        ConnectedPlayer = null;
    }

    public static void InitializePenumbraModList()
    {
        if (!PenumbraIPC_Caller.isPenumbraAvailable)
        {
            GlamLogger.Error("Penumbra is not available, unable to Initialize Penumbra Mod List.");
            return;
        }

        if (IsInitializingPenumbraModList)
        {
            GlamLogger.Debug("Penumbra Mod List initialization is already in progress.");
            return;
        }

        IsInitializingPenumbraModList = true;

        Task.Run(() =>
        {
            try
            {
                ClearPenumbraModList();
                PenumbraModList = IPCHelper.GetAllInstalledPenumbraMods();
                GlamLogger.Debug($"Loaded {PenumbraModList.Count} Penumbra mods successfully.");
            }
            catch (Exception ex)
            {
                GlamLogger.Error(ex.Message);
            }
            finally
            {
                IsInitializingPenumbraModList = false;
            }
        });
    }

    public static void ClearPenumbraModList()
    {
        PenumbraModList.Clear();
    }

    public static void Dispose()
    {
        ClearConnectedPlayer();
    }
}
