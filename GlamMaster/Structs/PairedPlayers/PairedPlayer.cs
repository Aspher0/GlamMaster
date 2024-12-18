using GlamMaster.Helpers;
using GlamMaster.Services;
using GlamMaster.Socket;
using GlamMaster.Socket.EmitEvents;
using GlamMaster.Structs.Payloads;
using GlamMaster.Structs.Permissions;
using System;

namespace GlamMaster.Structs.WhitelistedPlayers;

/*
 * A class that represents a player which the user paired with on their side.
 * It stores the paired player and a new empty list of permissions upon creation.
 * It also generates a secret encryption key that is meant to be sent to the other player for encryption and decryption, for security purposes
 */

public class PairedPlayer
{
    public string uniqueID; // A unique identifier

    public Player pairedPlayer; // The player paired with

    public PermissionsBuilder permissionsList = new PermissionsBuilder(new(), new()); // A list of permissions this paired player has on the user
    public PermissionsBuilder? theirPermissionsListToUser = null; // A list of permissions the user has on the paired player, populated when the user requests the paired player permissions

    public string theirSecretEncryptionKey = string.Empty; // The secret encryption key generated and sent by the paired player to the user
    public string mySecretEncryptionKey; // An auto generated secret encryption for encrypting data before being sent to the server, for security purposes

    public bool requestTheirPermissionsAutomatically = true;

    public PairedPlayer(string playerName, string playerWorld, string? uniqueID = null, PermissionsBuilder? permissionsList = null, PermissionsBuilder? theirPermissionsListToUser = null, string? mySecretEncryptionKey = null, string? theirSecretEncryptionKey = null, bool requestTheirPermissionsAutomatically = true)
    {
        if (uniqueID != null)
            this.uniqueID = uniqueID;
        else
            this.uniqueID = Guid.NewGuid().ToString();

        pairedPlayer = new Player(playerName, playerWorld);

        if (permissionsList != null)
            this.permissionsList = permissionsList;

        if (theirPermissionsListToUser != null)
            this.theirPermissionsListToUser = theirPermissionsListToUser;

        this.mySecretEncryptionKey = (mySecretEncryptionKey != null) ? mySecretEncryptionKey : GenerateEncryptionKey();

        if (theirSecretEncryptionKey != null)
            this.theirSecretEncryptionKey = theirSecretEncryptionKey;

        this.requestTheirPermissionsAutomatically = requestTheirPermissionsAutomatically;
    }

    public static string GenerateEncryptionKey() => "GLAM_MASTER_ENC_KEY-" + CommonHelper.GenerateRandomString(50, true);

    public void GenerateNewEncryptionKey()
    {
        mySecretEncryptionKey = GenerateEncryptionKey();
    }

    public void RequestTheirPermissions()
    {
        if (SocketManager.GetClient == null)
            return;

        Payload payload = new Payload(PayloadType.PermissionsRequest);

        _ = SocketManager.GetClient.SendPayloadToPlayer(this, payload);
    }

    public void SendYourPermissions()
    {
        if (SocketManager.GetClient == null)
            return;

        PermissionsBuilder cleanedPermissionsList = GenerateCleanedPermissionsList();

        Payload payload = new Payload(PayloadType.SendPermissions, cleanedPermissionsList);

        _ = SocketManager.GetClient.SendPayloadToPlayer(this, payload);
    }

    /*
     * Will generate a clean PermissionsBuilder and return it
     * 
     * The cleaned version will be exempt of Penumbra/Glamourer (etc) permissions if they haven't been enabled or if their respective IPCs are not available.
     */
    public PermissionsBuilder GenerateCleanedPermissionsList()
    {
        if (!permissionsList.enabled)
            return new PermissionsBuilder(null, null, permissionsList.enabled);

        var currentGlamourerPermissions = permissionsList.glamourerControlPermissions;
        var currentPenumbraPermissions = permissionsList.penumbraControlPermissions;

        GlamourerControlPermissions? glamourerControlPermissions = null;
        PenumbraControlPermissions? penumbraControlPermissions = null;

        if (Service.GlamourerIPC_Caller.isGlamourerAvailable && currentGlamourerPermissions!.canControlGlamourer)
            glamourerControlPermissions = currentGlamourerPermissions;

        if (Service.PenumbraIPC_Caller.isPenumbraAvailable && currentPenumbraPermissions!.CanControlPenumbra)
            penumbraControlPermissions = currentPenumbraPermissions;

        return new PermissionsBuilder(glamourerControlPermissions, penumbraControlPermissions, permissionsList.enabled);
    }
}
