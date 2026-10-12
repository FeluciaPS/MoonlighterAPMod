using DG.Tweening;
using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using UnityEngine.UI;
using static ItemMaster;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class BossPatches
    {

        [HarmonyPatch(typeof(DungeonBossRoom))]
        [HarmonyPatch(nameof(DungeonBossRoom.IsBossDefeated), MethodType.Getter)]
        internal static class IsBossDefeatedPatch
        {
            internal static void Postfix(ref bool __result)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                var culture = DungeonManager.Instance.culture;
                __result = WorldStateHelper.IsBossDefeated(culture);

                if (__result)
                {
                    MoonlighterAPMod.APInfo($"Preventing {culture} boss spawn because location is checked");
                }
                else
                {
                    MoonlighterAPMod.APInfo($"Forcing {culture} boss spawn because location is unchecked");
                }
            }
        }

        [HarmonyPatch(typeof(DungeonBossRoom))]
        [HarmonyPatch(nameof(DungeonBossRoom.InternalOnBossDeath))]
        internal static class BossDeathPatch
        {
            internal static void Postfix(DungeonBossRoom __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                Culture culture = DungeonManager.Instance.culture;

                if (culture == Culture.Unknown)
                {
                    GoalHelper.TrySendGoal(GoalHelper.GoalType.PirateBoss);
                    return;
                }

                // TODO: Don't hardcode base ID
                long locationId = ((int)culture) * 100 + 10;

                MoonlighterAPMod.APInfo($"Dropping location item for boss kill {culture}");
                CustomItemModule.DropLocationItem(locationId, __instance.BossHolder.position);

                // Lying to the game and pretending no boss was defeated so no scary cutscenes happen
                GameManager.Instance.bossDefeatedCulture = ItemMaster.Culture.Developer;
            }
        }
    }
}
