using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using GlamMaster.Structs;
using GlamMaster.Structs.WhitelistedPlayers;
using System.Linq;
using GlamMaster.Structs.Payloads;

public static class PayloadHelper
{
    public static string EncryptPayload(object payload, string key)
    {
        string jsonPayload = JsonConvert.SerializeObject(payload);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(jsonPayload);
            byte[] hashBytes = hmac.ComputeHash(plaintextBytes);
            return Convert.ToBase64String(hashBytes) + "." + Convert.ToBase64String(plaintextBytes);
        }
    }

    public static Payload? DecryptPayload(string encryptedPayload, string key)
    {
        string[] parts = encryptedPayload.Split('.');

        if (parts.Length != 2)
            return null;

        string receivedHashBase64 = parts[0];
        string receivedPayloadBase64 = parts[1];

        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] receivedHashBytes = Convert.FromBase64String(receivedHashBase64);
            byte[] computedHashBytes = hmac.ComputeHash(Convert.FromBase64String(receivedPayloadBase64));

            if (!computedHashBytes.SequenceEqual(receivedHashBytes))
            {
                return null;
            }

            string jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(receivedPayloadBase64));
            return JsonConvert.DeserializeObject<Payload>(jsonPayload);
        }
    }

    public static FullPayloadToPlayer BuildFullPayload(Player fromPlayer, PairedPlayer pairedPlayerToSendTo, Payload payloadToEncrypt)
    {
        string encryptionKey = pairedPlayerToSendTo.mySecretEncryptionKey;
        string encryptedPayload = EncryptPayload(payloadToEncrypt, encryptionKey);
        return new FullPayloadToPlayer(fromPlayer, pairedPlayerToSendTo.pairedPlayer, encryptedPayload);
    }
}
