using Dalamud.Game;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using GlamMaster.Structs;
using Lumina.Excel.GeneratedSheets;

#nullable enable
namespace GlamMaster.Services
{
    public class Service
    {
        public static Configuration? Configuration { get; set; }
        public static Player? ConnectedPlayer { get; set; }

        public static void Dispose()
        {
            ClearConnectedPlayer();
        }

        public static void ClearConnectedPlayer()
        {
            ConnectedPlayer = null;
        }

        public static void GetConnectedPlayer()
        {
            IPlayerCharacter? playerCharacter = ClientState!.LocalPlayer;

            if (playerCharacter != null)
            {
                string playerName = playerCharacter.Name.ToString();
                World? playerHomeworld = DataManager.GetExcelSheet<World>()?.GetRow(playerCharacter.HomeWorld.Id);

                if (playerHomeworld != null)
                {
                    ConnectedPlayer = new Player(playerName, playerHomeworld.Name.RawString);
                }
            }
        }

        public static void InitializeService()
        {
            InitializeConfig();
        }

        public static void InitializeConfig()
        {
            Configuration = Plugin.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Save();
        }

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
    }
}
