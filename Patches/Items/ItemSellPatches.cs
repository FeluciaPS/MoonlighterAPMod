using HarmonyLib;
using System.Collections.Generic;
using static MoonlighterAPMod.Mod.CustomItemModule;

namespace MoonlighterAPMod.Patches.Items
{
    internal class ItemSellPatches
    {
        [HarmonyPatch(typeof(HeroStatistics))]
        [HarmonyPatch(nameof(HeroStatistics.RegisterItemSell))]
        internal static class ItemSellPatch
        {
            internal static void Prefix(ItemMaster master)
            {
                if (master is not ArchipelagoItem apItem) return;
                apItem.SendLocation();
            }
        }

        [HarmonyPatch(typeof(HeroStatistics))]
        [HarmonyPatch(nameof(HeroStatistics.RegisterSalesBoxSell), [typeof(ItemMaster), typeof(int)])]
        internal static class ItemBoxSellPatch
        {
            internal static void Prefix(ItemMaster master)
            {
                if (master is not ArchipelagoItem apItem) return;
                apItem.SendLocation();
            }
        }

        [HarmonyPatch(typeof(HeroStatistics))]
        [HarmonyPatch(nameof(HeroStatistics.RegisterSalesBoxSell), [typeof(List<ItemMaster>), typeof(int)])]
        internal static class ItemBoxMassSellPatch
        {
            internal static void Prefix(List<ItemMaster> master)
            {
                foreach (ItemMaster item in master)
                {
                    if (item is not ArchipelagoItem apItem) continue;
                    apItem.SendLocation();
                }
            }
        }

        [HarmonyPatch(typeof(HeroStatistics))]
        [HarmonyPatch(nameof(HeroStatistics.RegisterHelperSell))]
        internal static class ItemHelperSellPatch
        {
            internal static void Prefix(List<ItemMaster> master)
            {
                foreach (ItemMaster item in master)
                {
                    if (item is not ArchipelagoItem apItem) continue;
                    apItem.SendLocation();
                }
            }
        }
    }
}
