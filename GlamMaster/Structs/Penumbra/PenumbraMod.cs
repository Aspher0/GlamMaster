using Penumbra.Api.Enums;
using System.Collections.Generic;

namespace GlamMaster.Structs.Penumbra;

public class PenumbraMod
{
    public string DirectoryName;
    public string ModName;
    public string FullPath;
    public bool IsFullPathDefault;
    public bool IsSortOrderNameDefault;
    public Dictionary<string, object?> OverallChangedItems;
    public IReadOnlyDictionary<string, (string[], GroupType)>? AvailableModSettings;
    public CurrentModSettings CurrentModSettings;

    public PenumbraMod(
            string directoryName,
            string modName,
            string fullPath,
            bool isFullPathDefault,
            bool isSortOrderNameDefault,
            Dictionary<string, object?> overallChangedItems,
            IReadOnlyDictionary<string, (string[], GroupType)>? availableModSettings,
            CurrentModSettings currentModSettings
        )
    {
        DirectoryName = directoryName;
        ModName = modName;
        FullPath = fullPath;
        IsFullPathDefault = isFullPathDefault;
        IsSortOrderNameDefault = isSortOrderNameDefault;
        OverallChangedItems = overallChangedItems;
        AvailableModSettings = availableModSettings;
        CurrentModSettings = currentModSettings;
    }
}
