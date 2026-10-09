using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using System;
using System.Collections.Generic;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class HawkerLocationPatches
    {
        [HarmonyPatch(typeof(HawkerPanel))]
        [HarmonyPatch("ResfrestTabCulture")]
        internal static class LoadHawkerInventoryTabPatch
        {
            internal static bool Prefix(HawkerPanel __instance, int index, string name, ItemMaster.Culture culture, List<string> discoveredDecorative)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;
                if (culture == ItemMaster.Culture.Wanderer) return true; // Don't touch wanderer stuff


                int cultureIndex = (int)culture - 1;
                List<ItemMaster> items = [];
                List<bool> visibleItems = [];

                if (cultureIndex < 0 || cultureIndex > 3) throw new Exception($"Culture index should be 0-3, but is {cultureIndex} from {culture} Culture.");

                if (!ItemHelper.CanReachFloor(culture, 1))
                {
                    __instance.itemSelector.SetTabInfo(index, name, items, visibleItems);
                    return false;
                }

                string[] itemNames = MoonlighterAPConstants.HAWKER_ITEMS[cultureIndex];
                int baseLocationId = 501 + cultureIndex * 4; // TODO: Move 501 to constants

                for (int i = 0; i < 4; i++)
                {
                    string itemName = itemNames[i];
                    ItemMaster itemMaster = InventoryItemHandler.GetItem(itemName);


                    if (itemMaster is not DecorativeItemMaster item)
                        throw new Exception("What the fuck this should never happen");

                    // Boss items are only available when the boss has been defeated
                    if (item.isFromBoss && !WorldStateHelper.IsBossDefeated(culture))
                        continue;

                    long locationId = baseLocationId + i;

                    string locationName = LocationHelper.GetLocationName(locationId);

                    CustomItemModule.ArchipelagoItem gameItem = CustomItemModule.GetOrCreateItem(locationName);

                    items.Add(gameItem);
                    bool isBought = CustomItemModule.HasLocation(locationName);
                    visibleItems.Add(!isBought);
                }

                __instance.itemSelector.SetTabInfo(index, name, items, visibleItems);
                return false;
                
                // TODO: Hawker items are buyable even if they're not marked as visible.
                // The original code probably checks against decorative item ownership when trying to buy.
            }
        }

        [HarmonyPatch(typeof(HawkerPanel))]
        [HarmonyPatch("ItemSelector_OnSelectedSlotChange")]
        internal static class UpdateHawkerSidebarPatch
        {
            internal static void Postfix(HawkerPanel __instance, int index, InventorySlotGUI slot)
            {
                // Method does not need to check if archipelago is connected, because no ArchipelagoItem should end up in this function
                // without being connected
                ItemMaster itemMaster = slot.item;
                if (itemMaster is not CustomItemModule.ArchipelagoItem item) return;

                DecorationBuyPanel buyDetailsPanel = __instance.buyDetailsPanel;
                buyDetailsPanel.textName.text = item.name;
                buyDetailsPanel.itemImage.overrideSprite = ItemDatabase.GetSprite(item);
                buyDetailsPanel.textOwned.text = CustomItemModule.HasLocation(item.locationName) ? "1" : "0";
                buyDetailsPanel.textDescription.text = buyDetailsPanel.colorTable.CheckTable(item.description);
                buyDetailsPanel.textCost.text = item.goldValue.ToString("0.");
            }
        }
    }
}
