namespace GlamMaster.Structs.Permissions
{
    /*
     * A class that stores all of the permissions a player has.
     * Each permissions modules are separated for better structuring.
     * 
     * Used in PairedPlayer.cs
     */

    public class PermissionsBuilder
    {
        public bool enabled = true;

        public GlamourerControlPermissions glamourerControlPermissions = new GlamourerControlPermissions(); // GlamourerControlPermissions Module

        public PermissionsBuilder(bool? enabled = null, GlamourerControlPermissions? glamourerControlPermissions = null)
        {
            if (enabled != null)
                this.enabled = enabled.Value;

            if (glamourerControlPermissions != null)
                this.glamourerControlPermissions = glamourerControlPermissions;
        }
    }
}
