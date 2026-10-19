using MoonlighterAPMod.Mod;
using System.Collections.Generic;
using System.Linq;

namespace MoonlighterAPMod.Archipelago
{
    internal class ItemHelper
    {
        internal static bool HasItem(string itemName)
        {
            return HasItems([itemName]);
        }

        internal static bool HasItem(long itemId)
        {
            return HasItems([itemId]);
        }

        internal static bool HasItems(string[] itemNames)
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            foreach (string name in itemNames)
            {
                if (items.FirstOrDefault(item => item.ItemName == name) == null) return false;
            }

            return true;
        }

        internal static bool HasItems(long[] itemIds)
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            foreach (long id in itemIds)
            {
                if (items.FirstOrDefault(item => item.ItemId == id) == null) return false;
            }

            return true;
        }

        internal static int CountItem(string name)
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            return items.Where(item => item.ItemName == name).Count;
        }

        internal static int CountItem(long itemId)
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            return items.Where(item => item.ItemId== itemId).Count;
        }

        internal static void ReceiveItem(long itemId, string itemName)
        {
            MoonlighterAPMod.APMessage($"Attempting to receive item {itemName}");
            if (!ArchipelagoClient.IsInGameplay)
            {
                ReceiveLater(itemId, itemName);
                return;
            }

            var gameItemName = InventoryItemHandler.GetGameItemName(itemName);
            MoonlighterAPMod.APInfo($"Game item name: {gameItemName ?? "Not a game item"}");
            // If the received item isn't a special item, mark it as received.
            bool received = gameItemName == null;
            
            // If it's not already received that means it's a special item, so we ask the inventory item handler to
            // run whatever code it needs to.
            if (!received)
                received = InventoryItemHandler.TryReceiveItem(gameItemName, itemName, itemId);

            if (!received) return;

            MoonlighterAPMod.APInfo("Should've received item");
            SaveManager.ReceiveItem(itemName);
        }

        internal static void ReceiveTrap(long itemId, string itemName)
        {
            // TODO: Nothing uses this yet so it's fine
        }

        internal static void ReceiveLater(long itemId, string itemName)
        {
            // TODO: I'm not sure what I want to do with this, but it's better to have it than not
        }

        /**
         * Specific item logic
         */
        internal static bool CanReachFloor(ItemMaster.Culture culture, int floorNum)
        {
            if (OptionsHelper.IsProgressiveFloors) 
                return CountItem($"Progressive {culture} Floor") >= floorNum;

            return HasItem($"Unlock {culture} Dungeon");
        }

        internal static string GetStartingWeapon()
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            return items.FirstOrDefault(item => MoonlighterAPConstants.STARTING_WEAPON_NAMES.Contains(item.ItemName))?.ItemName;
        }

        internal static int GetReceivedItemCount(string itemName)
        {
            List<string> collected = SaveManager.saveData.collectedItems;
            return collected.Count(item => item == itemName);
        }
    }
}
