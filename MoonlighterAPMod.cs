using BepInEx;
using BepInEx.Logging;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using System;
using UnityEngine;

namespace MoonlighterAPMod
{
    [BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
    internal class MoonlighterAPMod : BaseUnityPlugin
    {
        internal static bool DebugMode = false;
        internal static new ManualLogSource Logger;

        internal static ArchipelagoClient client;
        internal static ConnectionUI connectionUI;

        internal static void APMessage(object obj) => Logger.LogMessage(obj);
        internal static void APInfo(object obj) => Logger.LogInfo(obj);
        internal static void APError(object obj) => Logger.LogError(obj);
        internal static void APWarning(object obj) => Logger.LogWarning(obj);

        internal static void APDebug(object obj)
        {
            if (!DebugMode) return;
            APInfo(obj);
        }

        private void Awake()
        {
            GameObject archipelagoClient = new("AP-Client");
            client = archipelagoClient.AddComponent<ArchipelagoClient>();
            GameObject.DontDestroyOnLoad(archipelagoClient);

            Logger = base.Logger;
            PatchLoader.LoadPatches();

            GameObject uiManager = new("AP-UIManager");
            uiManager.AddComponent<UIManager>();

            GameObject connectionUI = new("AP-ConnectionUI");
            MoonlighterAPMod.connectionUI = connectionUI.AddComponent<ConnectionUI>();
            GameObject.DontDestroyOnLoad(connectionUI);
        }

        internal static void ExitDungeon()
        {
            // Placeholder function just so I remember how to exit a dungeon. May be useful as softlock prevention one day.
            DungeonResultsPanel resultsPanel = GUIManager.Instance.GetComponentInChildren<DungeonResultsPanel>();
            resultsPanel.PlayAnimations();
            typeof(DungeonResultsPanel).GetField("_fromEmblem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(resultsPanel, false);
            resultsPanel.OnBButtonPressed();
        }
    }
}
