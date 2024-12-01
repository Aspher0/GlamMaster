using System.Collections.Generic;

namespace GlamMaster.Structs.Permissions;

/*
 * A class that represents the Penumbra controlling permissions module.
 * 
 * Used in PermissionsBuilder.cs
 */

public class PenumbraControlPermissions
{
    public bool CanControlPenumbra = false;

    public List<PenumbraModPermissions> ModList = new List<PenumbraModPermissions>(); // Contains all penumbra mods with their permissions

    public bool CanInstallMods = false;
    public bool CanDeleteMods = false;
    public bool CanViewFullModPaths = true; // Will show all the folder structure where mods are in just like in penumbra

    public PenumbraControlPermissions(
            bool canControlPenumbraMods = false,
            List<PenumbraModPermissions>? modList = null,
            bool canInstallMods = false,
            bool canDeleteMods = false,
            bool canViewFullModPaths = true
        )
    {
        CanControlPenumbra = canControlPenumbraMods;

        if (modList != null)
            ModList = modList;

        CanInstallMods = canInstallMods;
        CanDeleteMods = canDeleteMods;
        CanViewFullModPaths = canViewFullModPaths;
    }
}
