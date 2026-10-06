using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using static ItemMaster;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class MiniBossLocationPatches
    {
        [HarmonyPatch(typeof(Enemy))]
        [HarmonyPatch(nameof(Enemy.DropItemsOnDeath))]
        internal static class MinibossDeathPatch
        {
            internal static void Postfix(Enemy __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;
                if (!__instance.IsMiniBoss) return;

                Culture culture = DungeonManager.Instance.culture;
                long locationId = ((int)culture) * 100 + 11 + DungeonManager.Instance.level;

                MoonlighterAPMod.APInfo($"Sending location for miniboss kill {culture} floor {DungeonManager.Instance.level + 1}");

                CustomItemModule.DropLocationItem(locationId, __instance.transform.position);
            }
        }
    }
}
