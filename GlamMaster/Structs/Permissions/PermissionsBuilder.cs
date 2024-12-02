namespace GlamMaster.Structs.Permissions;

/*
 * A class that stores all of the permissions a player has.
 * Each permissions modules are separated for better structuring.
 * 
 * Used in PairedPlayer.cs
 */

public class PermissionsBuilder
{
    public bool enabled = false;

    public GlamourerControlPermissions? glamourerControlPermissions = null;
    public PenumbraControlPermissions? penumbraControlPermissions = null;

    public PermissionsBuilder(GlamourerControlPermissions? glamourerControlPermissions = null, PenumbraControlPermissions? penumbraControlPermissions = null, bool enabled = false)
    {
        this.enabled = enabled;
        this.glamourerControlPermissions = glamourerControlPermissions;
        this.penumbraControlPermissions = penumbraControlPermissions;
    }
}
