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

    public GlamourerControlPermissions glamourerControlPermissions = new GlamourerControlPermissions();
    public PenumbraControlPermissions penumbraControlPermissions = new PenumbraControlPermissions();

    public PermissionsBuilder(bool enabled = false, GlamourerControlPermissions? glamourerControlPermissions = null, PenumbraControlPermissions? penumbraControlPermissions = null)
    {
        this.enabled = enabled;

        if (glamourerControlPermissions != null)
            this.glamourerControlPermissions = glamourerControlPermissions;

        if (penumbraControlPermissions != null)
            this.penumbraControlPermissions = penumbraControlPermissions;
    }
}
