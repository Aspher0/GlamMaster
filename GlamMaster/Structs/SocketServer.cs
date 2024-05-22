using System;

namespace GlamMaster.Structs
{
    public class SocketServer
    {
        public readonly string unique_id = string.Empty; // A unique identifier
        public string serverURL = string.Empty; // The URL of the server
        public string name = string.Empty; // A user-specified name

        public SocketServer(string unique_id, string serverURL = "", string name = "")
        {
            this.unique_id = unique_id;
            this.serverURL = serverURL;
            this.name = name;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (SocketServer)obj;
            return unique_id == other.unique_id && serverURL == other.serverURL && name == other.name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(unique_id, serverURL, name);
        }
    }
}
