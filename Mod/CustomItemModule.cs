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

        internal static void DropLocationItem(string locationName, Vector3 position)
        {
            ItemMaster item = GetItemByName(locationName);
            ItemStack stack = ItemStack.Create(item);
            Vector3 offset = (Vector3)UnityEngine.Random.insideUnitCircle * 32f;

            stack.Drop(position + offset);
        }
    }
}
