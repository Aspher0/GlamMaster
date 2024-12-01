using GlamMaster.UI.HelpInfos;
using System.Collections.Generic;

namespace GlamMaster.Structs;

public class HelpUITabsList
{
    public static List<HelpUITab> SelectableTabs = new List<HelpUITab>
    {
        new HelpUITab("Plugin Informations", InformationsTab.DrawInformations),
        new HelpUITab("Help with the \"Glamour Control\" tab", GlamourControlTab.DrawGlamourControlHelp),
        new HelpUITab("Help with the \"Player Pairing\" tab", PlayerPairingTab.DrawPlayerPairingHelp),
        new HelpUITab("Help with the \"Settings\" tab", SettingsTab.DrawSettingsHelp),
    };
}
