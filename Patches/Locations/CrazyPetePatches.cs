using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;

namespace MoonlighterAPMod.Patches.Locations
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
                    string locationName = $"{culture} Note {index + 1}";
                    if (CustomItemModule.HasLocation(locationName)) continue;

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

                string locationName = $"{culture} Note {index}";

                MoonlighterAPMod.APInfo($"Trying to drop crazy pete location {locationName}.");

                CustomItemModule.DropLocationItem(locationName, __instance.transform.position);
            }
        }
    }
}
