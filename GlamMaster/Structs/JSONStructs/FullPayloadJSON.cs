using System.Text.Json.Serialization;

namespace GlamMaster.Structs;

/*
 * JSON structure for the ReceivePayloadFromPlayer event of the socket.
 * Allows deserialization of a payload
 * 
 * Used in ReceivePermissionsPayloadFromPlayerHandler.cs
 */

public class FullPayloadJSON
{
    [JsonPropertyName("FromPlayer")]
    public required PlayerJSON FromPlayer { get; set; }

    [JsonPropertyName("ToPlayer")]
    public required PlayerJSON ToPlayer { get; set; }

    [JsonPropertyName("Payload")]
    public required string Payload { get; set; }
}
