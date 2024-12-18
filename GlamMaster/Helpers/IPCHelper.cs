using ECommons;
using GlamMaster.Services;
using GlamMaster.Structs.Penumbra;
using Newtonsoft.Json.Linq;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GlamMaster.Helpers;

public static class IPCHelper
{
    public static bool IsPenumbraAPIAvailable()
    {
        try
        {
            var ApiVersion = Service.PenumbraIPC_Caller.ApiVersion();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static PenumbraMod MakePenumbraModFromDirectoryAndName(Guid collectionId, KeyValuePair<string, string> mod)
    {
        if (!Service.PenumbraIPC_Caller.isPenumbraAvailable)
            throw new Exception("Penumbra API is not available while retrieving all installed penumbra mods.");

        var directoryName = mod.Key;
        var modName = mod.Value;

        var modFullPathObject = Service.PenumbraIPC_Caller.GetModPath(directoryName, modName);

        if (modFullPathObject.Item1 == PenumbraApiEc.ModMissing)
            throw new Exception($"Mod with directory {directoryName} does not exist while trying to create a PenumbraMod.");

        var fullPath = modFullPathObject.FullPath;
        var isFullPathDefault = modFullPathObject.FullDefault;
        var isSortOrderNameDefault = modFullPathObject.FullDefault;

        var overallChangedItems = Service.PenumbraIPC_Caller.GetChangedItems(directoryName, modName);
        var availableModSettings = Service.PenumbraIPC_Caller.GetAvailableModSettings(directoryName, modName);

        if (availableModSettings == null)
            throw new Exception($"Available mod settings for mod with directory {directoryName} returned null while trying to create a PenumbraMod.");

        var TryGetCurrentModSettings = Service.PenumbraIPC_Caller.GetCurrentModSettings(collectionId, directoryName, modName, false);

        CurrentModSettings currentModSettings;

        if (TryGetCurrentModSettings.StatusCode == 0) // Mod settings have been found and mod is in a configured state
        {
            currentModSettings = TryGetCurrentModSettings.CurrentModSettings!;
        }
        else if (TryGetCurrentModSettings.StatusCode == 1) // Mod or collection has not been found
        {
            throw new Exception($"Mod with directory {directoryName} does not exist or collection with GUID {collectionId} does not exist while trying to create a PenumbraMod.");
        }
        else // Mod is in an unconfigured state, will need to retrieve options manually from the json files from the mod folder
        {
            var defaultOptions = GetDefaultPenumbraModSettingsFromModFolder(directoryName, availableModSettings);

            currentModSettings = new CurrentModSettings(false, 0, defaultOptions, true);
        }

        return new PenumbraMod(directoryName, modName, fullPath, isFullPathDefault, isSortOrderNameDefault, overallChangedItems, availableModSettings, currentModSettings);
    }

    public static List<PenumbraMod> GetAllInstalledPenumbraMods()
    {
        if (!Service.PenumbraIPC_Caller.isPenumbraAvailable)
            throw new Exception("Penumbra API is not available while retrieving all installed penumbra mods.");

        if (Service.ClientState!.LocalPlayer == null)
            throw new Exception("The player is not logged in while retrieving all installed penumbra mods.");

        var currentlyAppliedCollection = Service.PenumbraIPC_Caller.GetCollectionForObject(Service.ClientState!.LocalPlayer.ObjectIndex);

        if (!currentlyAppliedCollection.ObjectValid)
            throw new Exception("Game object was not valid when retrieving currently used Penumbra collection.");

        List<PenumbraMod> penumbraModList = new List<PenumbraMod>();

        if (currentlyAppliedCollection.EffectiveCollection.Id == Guid.Empty)
            return penumbraModList;

        Dictionary<string, string> modList = Service.PenumbraIPC_Caller.GetModList();

        foreach (var mod in modList)
        {
            try
            {
                PenumbraMod penumbraMod = MakePenumbraModFromDirectoryAndName(currentlyAppliedCollection.EffectiveCollection.Id, mod);
                penumbraModList.Add(penumbraMod);
            }
            catch (Exception ex)
            {
                GlamLogger.Error(ex.Message);
                continue;
            }
        }

        return penumbraModList;
    }

    // TODO: BIG OPTIMIZATION
    public static Dictionary<string, List<string>> GetDefaultPenumbraModSettingsFromModFolder(string modFolder, IReadOnlyDictionary<string, (string[], GroupType)> availableOptions)
    {
        var penumbraRootPath = Service.PenumbraIPC_Caller.GetModDirectory();
        var fullModDiskPath = Path.Combine(penumbraRootPath, modFolder);

        var defaultPenumbraModSettings = new Dictionary<string, List<string>>();

        for (int i = 0; i < availableOptions.Count; i++)
        {
            string filePrefix = $"group_{(i + 1).ToString("D3")}_";
            string[] jsonFiles = Directory.GetFiles(fullModDiskPath, filePrefix + "*.json");

            foreach (var jsonFile in jsonFiles)
            {
                try
                {
                    var fileContent = File.ReadAllText(jsonFile);
                    var jsonData = JObject.Parse(fileContent);

                    var optionName = jsonData["Name"]?.ToString();
                    var optionType = jsonData["Type"]?.ToString();
                    var optionDefaultSettingsToken = jsonData["DefaultSettings"];

                    if (optionName == null || optionType == null || optionDefaultSettingsToken == null)
                        continue;

                    var foundAvailableOption = availableOptions.FirstOrNull(e => e.Key.ToString().ToLower() == optionName.ToLower());

                    if (foundAvailableOption == null)
                        continue;

                    // Sometimes, DefaultSettings bugs and becomes a huge number in the penumbra mod JSON file for some reason apparently
                    // TODO: Optimize this to merge it with the condition below, optionType == "Single" && optionDefaultSettings == 0
                    if (!int.TryParse(optionDefaultSettingsToken.ToString(), out var optionDefaultSettings))
                    {
                        if (foundAvailableOption.Value.Value.Item1.Count() > 0)
                            defaultPenumbraModSettings[foundAvailableOption.Value.Key] = new List<string>() { foundAvailableOption.Value.Value.Item1[0] };

                        break;
                    }

                    if ((optionType == "Multi" || optionType == "Imc") && optionDefaultSettings == 0)
                    {
                        defaultPenumbraModSettings[foundAvailableOption.Value.Key] = new List<string>();
                        break;
                    }

                    var listOfOptionsAvailable = foundAvailableOption.Value.Value.Item1;
                    var optionDefaultSettingsToBinaryString = Convert.ToString(optionDefaultSettings, 2).PadLeft(listOfOptionsAvailable.Length, '0');

                    List<string> defaultOptions = new List<string>();

                    if (optionType == "Single" && optionDefaultSettings == 0)
                    {
                        if (listOfOptionsAvailable.Count() > 0)
                        {
                            defaultOptions.Add(listOfOptionsAvailable[0]);
                            defaultPenumbraModSettings[foundAvailableOption.Value.Key] = defaultOptions;
                        }

                        break;
                    }

                    for (int index = optionDefaultSettingsToBinaryString.Length - 1; index >= 0; index--)
                    {
                        if (optionDefaultSettingsToBinaryString[index] == '1')
                        {
                            int optionIndex = optionDefaultSettingsToBinaryString.Length - 1 - index;
                            if (optionIndex < listOfOptionsAvailable.Length)
                            {
                                defaultOptions.Add(listOfOptionsAvailable[optionIndex]);
                            }
                        }
                    }

                    defaultPenumbraModSettings[foundAvailableOption.Value.Key] = defaultOptions;

                    break; // Once we find the right file, we can break and move on to the next option group
                }
                catch (Exception ex)
                {
                    GlamLogger.Error($"Error processing file {jsonFile}: {ex.Message}");
                    continue;
                }
            }
        }

        return defaultPenumbraModSettings;
    }
}
