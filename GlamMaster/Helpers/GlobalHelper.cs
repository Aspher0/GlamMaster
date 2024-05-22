using GlamMaster.Services;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GlamMaster.Helpers
{
    public class GlobalHelper
    {
        private static readonly char[] AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_".ToCharArray();

        public static string GenerateRandomString(int length = 50)
        {
            length = (length <= 0) ? 50 : length;

            var random = new Random();
            var result = new StringBuilder(length);

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
    }
}
