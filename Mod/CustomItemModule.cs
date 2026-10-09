using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Enums;

using MoonlighterAPMod.Archipelago;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MoonlighterAPMod.Mod
{
    public class CustomItemModule
    {
        // TODO: Create ItemDatabase wrapper instead
        private static readonly FieldInfo InstanceField =
            typeof(ItemDatabase).GetField(
                "_instance",
                BindingFlags.NonPublic | BindingFlags.Static
            );

        private static readonly FieldInfo ItemCollectionsField =
            typeof(ItemDatabase).GetField(
                "itemCollections",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

        private static readonly FieldInfo ItemsField =
            typeof(ItemDatabase)
                .GetNestedType("ItemCollection", BindingFlags.NonPublic)
                .GetField(
                    "items",
                    BindingFlags.Instance | BindingFlags.Public
                );

        public class ArchipelagoItem : ItemMaster
        {
            public string customSpriteName = "item-useful";
            public string locationName;
            public ItemFlags type;

            public ArchipelagoItem(string locationName)
            {
                this.locationName = locationName;

                name = "Archipelago Item";
                nameKey = "MOON-AP::Archipelago Item";
                worldSpriteName = "item_golem_blue_print";
                description = "An Archipelago item for a different player";
                descriptionKey = "MOON-AP::An archipelago item for a different player";
                goldValue = 1000;
                tier = Tier.Tier1;
                culture = Culture.Merchant;
                maxStack = 1;
                canAppearInChest = false;
                chestWeight = 1.0f;
                minChestStack = 1;
                maxChestStack = 1;
                fixedChestStack = 1;
            }

            public void SetDescription(string description)
            {
                this.description = description;
                descriptionKey = $"MOON-AP::{description}";
            }

            public void SetName(string name)
            {
                this.name = name;
                nameKey = $"MOON-AP::{name}";
            }

            public void SetType(ItemFlags type)
            {
                this.type = type;
                if (type == ItemFlags.None)
                {
                    customSpriteName = "item-filler";
                }
                if (type == ItemFlags.Trap)
                {
                    customSpriteName = "item-trap";
                }
            }

            public void SendLocation()
            {
                LocationHelper.SendLocation(locationName);
            }
        }

        // TODO: Re-order some functions for clarity
        private static ArchipelagoItem CreateItem(string locationName = "Unknown Location")
        {
            return new(locationName);
        }

        private static ArchipelagoItem AddItemToGame(string locationName)
        {
            var newItem = CreateItem(locationName);
            ItemDatabase instance = (ItemDatabase)InstanceField.GetValue(null);
            var itemCollections = (Array)ItemCollectionsField.GetValue(instance);
            for (int i = 0; i < 3; i++)
            {
                var collection = itemCollections.GetValue(i);
                var items = (List<ItemMaster>)ItemsField.GetValue(collection);

                items.Add(newItem);
            }

            return newItem;
        }

        private static ArchipelagoItem CreateItemWithFlags(string locationName, string itemName, string targetName, ItemFlags type)
        {
            ArchipelagoItem item = GetOrCreateItem(locationName);
            item.SetName($"{itemName}");
            item.SetDescription($"An archipelago item for {targetName}.");
            item.SetType(type);

            return item;
        }

        public static ItemMaster GetItemByName(string name)
        {
            // I love reflection I love reflectoin
            ItemDatabase instance = (ItemDatabase)InstanceField.GetValue(null);
            var itemCollections = (Array)ItemCollectionsField.GetValue(instance);
            var firstCollection = itemCollections.GetValue(0);
            var items = (List<ItemMaster>)ItemsField.GetValue(firstCollection);

            foreach (ItemMaster itemMaster in items)
            {
                if (itemMaster is not ArchipelagoItem item) 
                    continue;

                if (item.locationName == name)
                {
                    return item;
                }
            }

            return ItemDatabase.GetItemByName(name);
        }

        public static ArchipelagoItem GetOrCreateItem(string locationName)
        {
            if (GetItemByName(locationName) is ArchipelagoItem item) return item;

            item = AddItemToGame(locationName);
            item.SetName("MOON-AP::" + locationName);

            return item;
        }

        internal static void CreateItemsFromScout(Dictionary<long, ScoutedItemInfo> locationData)
        {
            foreach (long id in locationData.Keys)
            {
                ScoutedItemInfo info = locationData[id];
                string locationName = LocationHelper.GetLocationName(id);

                string itemName = info.ItemDisplayName;
                string targetName = info.Player.Name;
                ItemFlags type = ItemFlags.None;
                if (info.Flags.HasFlag(ItemFlags.Advancement)) type = ItemFlags.Advancement;
                else if (info.Flags.HasFlag(ItemFlags.Advancement)) type = ItemFlags.NeverExclude;
                else if (info.Flags.HasFlag(ItemFlags.Trap)) type = ItemFlags.Trap;

                CreateItemWithFlags(locationName, itemName, targetName, type);
            }
        }

        internal static void DropLocationItem(long locationId, Vector3 position)
        {
            string locationName = LocationHelper.GetLocationName(locationId);
            DropLocationItem(locationName, position);
        }

        internal static bool DropLocationItem(string locationName, Vector3 position)
        {
            ItemMaster item = GetItemByName(locationName);
            if (item is not ArchipelagoItem apItem) return false;
            if (HasLocation(apItem.locationName)) return false;

            DropItem(item, position);

            return true;
        }

        internal static void DropItem(ItemMaster item, Vector3 position)
        {
            ItemStack stack = ItemStack.Create(item);
            Vector3 offset = (Vector3)UnityEngine.Random.insideUnitCircle * 32f;

            stack.Drop(position + offset);
        }

        internal static bool HasLocation(long locationId)
        {
            string locationName = LocationHelper.GetLocationName(locationId);
            return HasLocation(locationName);
        }

        internal static bool HasLocation(string locationName)
        {
            if (LocationHelper.IsChecked(locationName)) return true;

            ItemMaster item = GetItemByName(locationName);
            if (item == null) return false;
            if (item is not ArchipelagoItem) return false;

            if (GetItemCount(item) > 0) return true;
            return false;
        }

        internal static int GetItemCount(ItemMaster item)
        {
            int bagItemCount = HeroMerchant.Instance.heroMerchantInventory.GetBagItemCount(item);
            int equipmentItemCount = HeroMerchant.Instance.heroMerchantInventory.GetEquippmentItemCount(item);
            int mimicItemCount = ShopManager.GetItemCountFromMimicFamiliar(item);

            int shopItemCount = (bool)ShopManager.Instance ? GetShopItemCount(item) : GetShopItemCountFromSave(item);

            int droppedItemCount = 0; // TODO: Write this
            return bagItemCount + equipmentItemCount + mimicItemCount + shopItemCount + droppedItemCount;
        }

        internal static int GetShopItemCount(ItemMaster item)
        {
            ShopManager shopManager = ShopManager.Instance;

            if (item == null) return 0;
            int num = 0;

            foreach (Showcase showcase in shopManager.references.shopShowcases)
            {
                foreach (ShowcaseSlot slot in showcase.itemPositions)
                {
                    if (slot == null ||
                        slot.itemStack == null ||
                        slot.itemStack.master == null) continue;

                    if (slot.itemStack.master == item) num += 1;
                }
            }

            num += ShopManager.Instance.GetChestItemCount(item);
            
            return num + shopManager.references.salesBox.GetItemCount(item);
        }

        internal static int GetShopItemCountFromSave(ItemMaster item)
        {
            if (item == null) return 0;

            GameSlot currentGameSlot = GameManager.Instance.currentGameSlot;
            ShopSaveInfo saveInfo = currentGameSlot.shopSaveInfo;
            int num = 0;

            foreach (ItemSaveInfo saveitem in saveInfo.shopShowcases)
            {
                // TODO: This may not work but I don't care right now
                if (saveitem.name == item.name) num += 1;
            }

            for (int num2 = 0; num2 < saveInfo.shopChests.Count; num2++)
            {
                num += ShopManager.GetItemCountFromChestSave(item, saveInfo.shopChests[num2]);
            }
            num += ShopManager.GetItemCountFromChestSave(item, saveInfo.bedChest);
            num += ShopManager.GetItemCountFromChestSave(item, saveInfo.salesBox);
            return num;
        }
    }
}
