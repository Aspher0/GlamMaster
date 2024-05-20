using System.Text.Json.Serialization;

namespace GlamMaster.Structs
{
    internal class ReceiveServerPongJSON
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("message")]
        public required string Message { get; set; }
    }
}
