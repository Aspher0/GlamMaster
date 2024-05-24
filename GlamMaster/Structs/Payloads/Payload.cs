using System;

namespace GlamMaster.Structs.Payloads
{
    public class Payload
    {
        public PayloadType payloadType;
        public string timestamp;

        public Payload(PayloadType payloadType)
        {
            this.payloadType = payloadType;
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }
    }

    public enum PayloadType
    {
        PermissionsRequest
    }
}
