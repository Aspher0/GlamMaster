using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using GlamMaster.Socket;

#nullable enable
namespace GlamMaster.Services
{
    public class Service
    {
        public static Plugin? Plugin { get; private set; }
        public static Configuration? Configuration { get; private set; }
        public static SocketManager? SocketManager { get; private set; }

        public static void InitializeService(Plugin plugin)
        {
            Plugin = plugin;
            InitializeConfig();
        }
        
        public static void InitializeConfig()
        {
            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);
            Configuration.Save();
        }

        [PluginService]
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;

        [PluginService]
        public static IChatGui ChatGui { get; private set; } = null!;

        [PluginService]
        public static ICommandManager CommandManager { get; private set; } = null!;

        [PluginService]
        public static IPluginLog Logger { get; private set; } = null!;
    }
}
