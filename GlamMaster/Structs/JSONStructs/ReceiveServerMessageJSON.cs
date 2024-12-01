using System.Text.Json.Serialization;

namespace GlamMaster.Structs;

/*
 * JSON structure for the ReceiveServerMessage event of the socket.
 * Allows deserialization of the server's response
 * 
 * Used in ReceiveServerMessageHandler.cs
 */

public class ReceiveServerMessageJSON
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
