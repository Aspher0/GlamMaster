using GlamMaster.Helpers;
using GlamMaster.Structs.Permissions;
using System;

namespace GlamMaster.Structs.WhitelistedPlayers
{
    /*
     * A class that represents a player which the user paired with on their side.
     * It stores the paired player name, its home world and a new empty list of permissions upon creation.
     * It also generates a secret encryption key that is meant to be sent to the other player for encryption and decryption, for security purposes
     */

    public class PairedPlayer
    {
        public string uniqueID; // A unique identifier

        public string playerName; // The paired player first and last name (Ex. : "Some Player")
        public string homeWorld; // The paired player world

        public PermissionsBuilder permissionsList = new PermissionsBuilder(); // A list of permissions this paired player has on the user

        public string theirSecretEncryptionKey = string.Empty; // The secret encryption key generated and sent by the paired player to the user
        public string mySecretEncryptionKey; // An auto generated secret encryption for encrypting data before being sent to the server, for security purposes

        public PairedPlayer(string playerName, string playerWorld, string? uniqueID = null, PermissionsBuilder? permissionsList = null, string? mySecretEncryptionKey = null, string? theirSecretEncryptionKey = null)
        {
            if (uniqueID != null)
                this.uniqueID = uniqueID;
            else
                this.uniqueID = Guid.NewGuid().ToString();

            this.playerName = playerName;
            this.homeWorld = playerWorld;

            if (permissionsList != null)
                this.permissionsList = permissionsList;

            if (mySecretEncryptionKey != null)
                this.mySecretEncryptionKey = mySecretEncryptionKey;
            else
                this.mySecretEncryptionKey = GlobalHelper.GenerateRandomString(250, true);

            if (theirSecretEncryptionKey != null)
                this.theirSecretEncryptionKey = theirSecretEncryptionKey;
        }

        public void GenerateNewEncryptionKey()
        {
            mySecretEncryptionKey = GlobalHelper.GenerateRandomString(100, true);
        }
    }
}
