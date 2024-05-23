using System.Text.Json.Serialization;

namespace GlamMaster.Structs
{
    /*
     * Temporary JSON structure for the ReceiveServerPong event of the socket.
     * Allows deserialization of the server's response
     * 
     * Used in ReceiveServerPongHandler.cs
     */

    internal class ReceiveServerPongJSON
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }
}
