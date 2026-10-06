using DG.Tweening;
using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace MoonlighterAPMod.Patches
{
    internal class InventoryPatches
    {
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
