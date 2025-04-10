using ECommons.EzIpcManager;
using Glamourer.Api.IpcSubscribers;

namespace GlamMaster.IPC.Glamourer;

public class GlamourerIPC_Caller
{
    public bool isGlamourerAvailable { get; set; } = false;

    public GlamourerIPC_Caller()
    {
         EzIPC.Init(this, "Glamourer");
    }

    private readonly UnlockAll glamourerUnlockAll = new(Plugin.PluginInterface);

    public int UnlockAll(uint key) => glamourerUnlockAll.Invoke(key);
}
