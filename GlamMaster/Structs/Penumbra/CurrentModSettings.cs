using System.Collections.Generic;

namespace GlamMaster.Structs.Penumbra;

public class CurrentModSettings
{
    public bool Enabled;
    public int Priority;
    public Dictionary<string, List<string>> CurrentOptions; // Dictionnary linking group name to enabled option(s)
    public bool Inherited;

    public CurrentModSettings(
            bool enabled,
            int priority,
            Dictionary<string, List<string>> currentOptions,
            bool inherited
        )
    {
        Enabled = enabled;
        Priority = priority;
        CurrentOptions = currentOptions;
        Inherited = inherited;
    }
}
