using GlamMaster.Helpers;
using GlamMaster.Structs.Permissions;
using System;

namespace GlamMaster.Structs.WhitelistedPlayers
{
    /*
     * A class that represents a player which the user paired with on their side.
     * It stores the paired player and a new empty list of permissions upon creation.
     * It also generates a secret encryption key that is meant to be sent to the other player for encryption and decryption, for security purposes
     */

    public class PairedPlayer
    {
        public string uniqueID; // A unique identifier

        public Player pairedPlayer; // The player paired with

        public PermissionsBuilder permissionsList = new PermissionsBuilder(); // A list of permissions this paired player has on the user

        public string theirSecretEncryptionKey = string.Empty; // The secret encryption key generated and sent by the paired player to the user
        public string mySecretEncryptionKey; // An auto generated secret encryption for encrypting data before being sent to the server, for security purposes

        public PairedPlayer(string playerName, string playerWorld, string? uniqueID = null, PermissionsBuilder? permissionsList = null, string? mySecretEncryptionKey = null, string? theirSecretEncryptionKey = null)
        {
            if (uniqueID != null)
                this.uniqueID = uniqueID;
            else
                this.uniqueID = Guid.NewGuid().ToString();

            pairedPlayer = new Player(playerName, playerWorld);

            if (permissionsList != null)
                this.permissionsList = permissionsList;

            if (mySecretEncryptionKey != null)
                this.mySecretEncryptionKey = mySecretEncryptionKey;
            else
                GenerateNewEncryptionKey();

            if (theirSecretEncryptionKey != null)
                this.theirSecretEncryptionKey = theirSecretEncryptionKey;
        }

        public void GenerateNewEncryptionKey()
        {
            mySecretEncryptionKey = "GLAM_MASTER_ENC_KEY-" + GlobalHelper.GenerateRandomString(50, true);
        }
    }
}
