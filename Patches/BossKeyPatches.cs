using DG.Tweening;
using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using UnityEngine.UI;
using static ItemMaster;

namespace MoonlighterAPMod.Patches
{
    internal class BossKeyPatches
    {

        [HarmonyPatch(typeof(DungeonBossRoom))]
        [HarmonyPatch(nameof(DungeonBossRoom.IsBossDefeated), MethodType.Getter)]
        internal static class IsBossDefeatedPatch
        {
            internal static void Postfix(ref bool __result)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                Culture culture = DungeonManager.Instance.culture;

                long locationId = ((int)culture) * 100 + 10;

                __result = LocationHelper.IsChecked(locationId);

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

                long locationId = ((int)culture) * 100 + 10;

                MoonlighterAPMod.APInfo($"Sending location for boss kill {culture}");
                LocationHelper.SendLocation(locationId);

                MoonlighterAPMod.APInfo($"Lying to the game and pretending no boss was defeated so no scary cutscenes happen");
                GameManager.Instance.bossDefeatedCulture = ItemMaster.Culture.Developer;
            }
        }

        [HarmonyPatch(typeof(HeroInventoryPanel))]
        [HarmonyPatch("UpdateBossKeys")]
        internal static class InventoryKeysPatch
        {
            private static void ShowKey(Image image)
            {
                image.DOFade(1f, 0.1f);
            }

            private static void HideKey(Image image)
            {
                image.DOFade(0f, 0.1f);
            }

            internal static bool Prefix(HeroInventoryPanel __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;
                MoonlighterAPMod.APInfo("Modifying boss key display");


                if (ItemHelper.HasItem("Golem Key"))
                    ShowKey(__instance.golemKey);
                else
                    HideKey(__instance.golemKey);

                if (ItemHelper.HasItem("Forest Key"))
                    ShowKey(__instance.forestKey);
                else
                    HideKey(__instance.forestKey);

                if (ItemHelper.HasItem("Desert Key"))
                    ShowKey(__instance.desertKey);
                else
                    HideKey(__instance.desertKey);

                if (ItemHelper.HasItem("Tech Key"))
                    ShowKey(__instance.techKey);
                else
                    HideKey(__instance.techKey);

                return false;
            }
        }
    }
}
