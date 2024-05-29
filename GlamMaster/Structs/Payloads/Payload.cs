using GlamMaster.Structs.Permissions;
using System;

namespace GlamMaster.Structs.Payloads
{
    public class Payload
    {
        public PayloadType PayloadType;
        public string Timestamp;

        public PermissionsBuilder? Permissions;

        public Payload(PayloadType PayloadType, PermissionsBuilder? Permissions = null)
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            this.PayloadType = PayloadType;
            this.Permissions = Permissions;
        }
    }

    public enum PayloadType
    {
        PermissionsRequest,
        SendPermissions
    }
}
