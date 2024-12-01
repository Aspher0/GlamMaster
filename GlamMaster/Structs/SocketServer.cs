using System;

namespace GlamMaster.Structs;

/*
 * A class that represents a socket server entry in the settings UI
 * Has a unique ID, a server url and a name
 */

public class SocketServer
{
    public readonly string uniqueID; // A unique identifier

    public string serverURL = string.Empty; // The URL of the server
    public string name = string.Empty; // A user-specified name

    public SocketServer(string? uniqueID = null, string? serverURL = null, string? name = null)
    {
        if (uniqueID != null)
            this.uniqueID = uniqueID;
        else
            this.uniqueID = Guid.NewGuid().ToString();

        if (serverURL != null)
            this.serverURL = serverURL;

        if (name != null)
            this.name = name;
    }

    public override bool Equals(object? obj) // Override the Equals method to compare properties instead of instances of the class
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (SocketServer)obj;
        return uniqueID == other.uniqueID && serverURL == other.serverURL && name == other.name;
    }

    public override int GetHashCode() // Unnecessary for the moment
    {
        return HashCode.Combine(uniqueID, serverURL, name);
    }
}
