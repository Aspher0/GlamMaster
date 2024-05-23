using System;
using System.Text;
using System.Text.RegularExpressions;
using GlamMaster.Services;
using System.Linq;
using GlamMaster.Structs.WhitelistedPlayers;

namespace GlamMaster.Helpers
{
    internal class GlobalHelper
    {
        private static readonly char[] AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_".ToCharArray();
        private static readonly Regex UrlRegex = new Regex(@"^(http|https|ws|wss)://([\w\-\.]+)(:\d+)?(/.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string GenerateRandomString(int length = 50, bool unique = false)
        {
            length = (length <= 0) ? 50 : length;

            var random = new Random();
            var result = new StringBuilder(length);

            if (unique)
            {
                string uniquePrefix = Guid.NewGuid().ToString("N").Substring(0, 8);
                result.Append(uniquePrefix).Append("-");
            }

            for (int i = 0; i < length; i++)
            {
                result.Append(AllowedChars[random.Next(AllowedChars.Length)]);
            }

            return result.ToString();
        }

        public static bool RegExpMatch(string text, string regexp)
        {
            if (string.IsNullOrWhiteSpace(regexp))
                return true;

            try
            {
                return Regex.IsMatch(text, regexp, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                GlamLogger.Error("Invalid RegEXP: " + regexp);
                return false;
            }
        }

        public static bool IsValidServerUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            Match match = UrlRegex.Match(url);

            if (!match.Success)
                return false;

            if (match.Groups[3].Success)
            {
                string portStr = match.Groups[3].Value.Substring(1);

                if (!int.TryParse(portStr, out int port) || port < 1 || port > 65535)
                    return false;
            }

            return true;
        }

        public static PairedPlayer? TryGetExistingPlayerAlreadyInConfig(string playerName, string playerHomeworld)
        {
            return Service.Configuration!.PairedPlayers.Find(player => player.playerName == playerName && player.homeWorld == playerHomeworld);
        }
    }
}
