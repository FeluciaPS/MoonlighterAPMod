using HarmonyLib;
using MoonlighterAPMod.Archipelago;

namespace MoonlighterAPMod.Patches
{
    internal class GameSavePatches
    {
        [HarmonyPatch(typeof(GameManager))]
        [HarmonyPatch(nameof(GameManager.SerializeGameSlot))]
        internal static class GameSavePatch
        {
            internal static void Prefix()
            {
                if (!ArchipelagoClient.IsClientConnected) return;
                SaveManager.Save();
            }
        }

        [HarmonyPatch(typeof(GameManager))]
        [HarmonyPatch(nameof(GameManager.LoadSlot))]
        internal static class GameLoadPatch
        {
            internal static void Prefix()
            {
                if (!ArchipelagoClient.IsClientConnected) return;
                SaveManager.Load();
            }
        }
    }
}
