using GlamMaster.Services;
using Dalamud.Game.Text;

namespace GlamMaster.Helpers
{
    internal class GlamLogger
    {
        public static void Verbose(string message)
        {
            Service.Logger.Verbose("[GLAMOURMASTER] - " + message);
        }

        public static void Debug(string message)
        {
            Service.Logger.Debug("[GLAMOURMASTER] - " + message);
        }

        public static void Information(string message)
        {
            Service.Logger.Information("[GLAMOURMASTER] - " + message);
        }

        public static void Warning(string message)
        {
            Service.Logger.Warning("[GLAMOURMASTER] - " + message);
        }

        public static void Error(string message)
        {
            Service.Logger.Error("[GLAMOURMASTER] - " + message);
        }

        public static void Fatal(string message)
        {
            Service.Logger.Fatal("[GLAMOURMASTER] - " + message);
        }

        public static void Print(string message, bool printPluginName = true)
        {
            Service.ChatGui.Print(printPluginName ? "[GLAMOURMASTER] - " + message : message);
        }

        public static void Print(XivChatEntry chatEntry, bool printPluginName = true)
        {
            if (printPluginName)
                chatEntry.Message = "[GLAMOURMASTER] - " + chatEntry.Message;

            Service.ChatGui.Print(chatEntry);
        }
    }
}
