using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using UnityEngine;
using static MoonlighterAPMod.Mod.CustomItemModule;

namespace MoonlighterAPMod.Patches.Items
{
    internal class ArchipelagoItemPatches
    {
        [HarmonyPatch(typeof(ItemDatabase))]
        [HarmonyPatch(nameof(ItemDatabase.ReadDefaultFile))]
        internal static class ItemCreatePatch
        {
            internal static void Postfix()
            {
                // Archipelago Dummy Item is primarily used for rendering, such as in the tutorial
                var dummyItem = GetOrCreateItem("Archipelago Dummy Item");
                dummyItem.SetName("Archipelago Dummy Item");

                MoonlighterAPMod.client.ScoutAllLocations();
            }
        }

        [HarmonyPatch(typeof(HeroMerchantInventory))]
        [HarmonyPatch(nameof(HeroMerchantInventory.TryAddItem), [typeof(ItemStack), typeof(int)])]
        internal static class ItemReceivePatch
        {
            internal static bool Prefix(ref bool __result, ItemStack item)
            {
                if (!ArchipelagoClient.IsClientConnected)
                    return true;

                if (OptionsHelper.RequireSale) 
                    return true;

                ItemMaster itemMaster = item.master;
                
                if (itemMaster is not ArchipelagoItem apItem)
                    return true;

                apItem.SendLocation();
                __result = true;
                GameObject.Destroy(item.gameObject);
                return false;
            }
        }
    }
}
