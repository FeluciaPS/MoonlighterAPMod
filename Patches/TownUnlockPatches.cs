using HarmonyLib;
using MoonlighterAPMod.Archipelago;

namespace MoonlighterAPMod.Patches
{
    internal class TownUnlockPatches
    {
        [HarmonyPatch(typeof(TownboardSaveInfo))]
        [HarmonyPatch(nameof(TownboardSaveInfo.IsBlacksmithUnlocked))]
        internal static class BlacksmithUnlockedPatch
        {
            internal static bool Prefix(ref bool __result)
            {
                if (!ArchipelagoClient.IsClientConnected) 
                    return true;

                __result = true;

                return false;
            }
        }

        [HarmonyPatch(typeof(TownboardSaveInfo))]
        [HarmonyPatch(nameof(TownboardSaveInfo.IsHawkerUnlocked))]
        internal static class HawkerUnlockedPatch
        {
            internal static bool Prefix(ref bool __result)
            {
                if (!ArchipelagoClient.IsClientConnected)
                    return true;

                __result = true;

                return false;
            }
        }
    }
}
