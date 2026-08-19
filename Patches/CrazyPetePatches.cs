using HarmonyLib;
using MoonlighterAPMod.Archipelago;

namespace MoonlighterAPMod.Patches
{
    internal class CrazyPetePatches
    {
        [HarmonyPatch(typeof(CrazyPeteNotes))]
        [HarmonyPatch(nameof(CrazyPeteNotes.Start))]
        internal static class PeteInitPatch
        {
            internal static void Postfix(CrazyPeteNotes __instance)
            {
                // Forcefully set the pete index to one that hasn't been collected yet
                // so that it's not a total RNG check.
                ItemMaster.Culture culture = DungeonManager.Instance.culture;
                for (int index = 0; index < 3; index++)
                {
                    long locationId = ((int)culture) * 100 + index + 1;
                    if (LocationHelper.IsChecked(locationId)) continue;

                    __instance.crazyPeteNoteIndex = index;
                    break;
                }
            }
        }

        [HarmonyPatch(typeof(CrazyPeteNotes))]
        [HarmonyPatch(nameof(CrazyPeteNotes.Interact))]
        internal static class NotePatch
        {
            internal static void Prefix(CrazyPeteNotes __instance)
            {
                ItemMaster.Culture culture = DungeonManager.Instance.culture;
                int index = __instance.crazyPeteNoteIndex % 3 + 1;

                long locationId = ((int)culture) * 100 + index;

                MoonlighterAPMod.APInfo($"Sending crazy pete location {culture} {index} with ID {locationId}.");
                LocationHelper.SendLocation(locationId);
            }
        }
    }
}
