using System.Text.Json.Serialization;

namespace GlamMaster.Structs;

/*
 * JSON structure for the ReceiveServerMessage event of the socket.
 * Allows deserialization of the players in a full payload
 * 
 * Used in FullPayloadJSON.cs
 */

public class PlayerJSON
{
    [JsonPropertyName("playerName")]
    public required string playerName { get; set; }

    [JsonPropertyName("homeWorld")]
    public required string homeWorld { get; set; }
}
