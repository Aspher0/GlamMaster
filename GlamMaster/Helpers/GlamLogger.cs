using Dalamud.Game.Text;
using GlamMaster.Services;
using System;

namespace GlamMaster.Helpers;

public static class GlamLogger
{
    public static void LogError(this Exception e)
    {
        Service.Logger.Error($"[GLAMOURMASTER] - {e.Message}\n{e.StackTrace ?? ""}");
    }

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

    public static void PrintErrorChannel(string message, bool printPluginName = true)
    {
        XivChatEntry chat = new XivChatEntry();
        chat.Type = XivChatType.ErrorMessage;
        chat.Message = printPluginName ? "[GLAMOURMASTER] - " + message : message;

        Service.ChatGui.Print(chat);
    }

    public static void PrintdError(string message, bool printPluginName = true)
    {
        Service.ChatGui.Print((printPluginName ? "[GLAMOURMASTER] - " : "") + "ERROR - " + message);
    }
}
