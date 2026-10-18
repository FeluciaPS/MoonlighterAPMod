using MoonlighterAPMod.Archipelago;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MoonlighterAPMod.Mod
{
    internal class InventoryItemHandler
    {
        internal static bool TryReceiveItem(string gameItemName, string itemName, long itemId)
        {
            if (ReceiveMoney(gameItemName)) return true;

            ItemMaster item = GetItem(gameItemName);

            if (item != null && item is not DecorativeItemMaster)
            {
                ItemStack stack = ItemStack.Create(item);
                return HeroMerchant.Instance.heroMerchantInventory.TryAddItem(stack);
            }

            return false;
        }

        internal static bool ReceiveMoney(string amount)
        {
            if (amount == null) return false;
            if (amount == "Money") return ReceiveMoney(0.1f);

            Regex flatMoneyRegex = new(@"^(-?\d+)\sMoney$", RegexOptions.IgnoreCase);
            Regex percentMoneyRegex = new(@"^(-?\d+\.?\d)*%\sMoney$", RegexOptions.IgnoreCase);

            var flatMatch = flatMoneyRegex.Match(amount);
            if (flatMatch.Success)
            {
                int money = int.Parse(flatMatch.Groups[1].Value);
                return ReceiveMoney(money);
            }

            var percentMatch = percentMoneyRegex.Match(amount);
            if (percentMatch.Success)
            {
                float money = float.Parse(flatMatch.Groups[1].Value) / 100;
                return ReceiveMoney(money);
            }

            return false;
        }

        internal static bool ReceiveMoney(int amount = 0)
        {
            HeroMerchant merchant = HeroMerchant.Instance;
            merchant.addGold(amount);
            return true;
        }

        internal static bool ReceiveMoney(float amount = 0)
        {
            HeroMerchant merchant = HeroMerchant.Instance;

            int toAdd = (int)(merchant.GetCurrentGold() * amount);
            merchant.addGold(toAdd);
            return true;
        }

        internal static ItemMaster GetItem(string name)
        {
            return ItemDatabase.GetItems(x => x is not CustomItemModule.ArchipelagoItem && x.displayName == name).FirstOrDefault();
        }

        internal static string GetGameItemName(string itemName)
        {
            if (itemName == "Money") return itemName;

            if (itemName.Contains("Progressive"))
            {
                string lastWord = itemName.Split(' ').Last();

                switch (lastWord)
                {
                    case "Sword":
                    case "Spear":
                    case "Gloves":
                    case "Bow":
                        return GetProgressiveWeaponItem(itemName);

                    case "Bandana":
                    case "Helmet":
                    case "Chestplate":
                    case "Boots":
                        return GetProgressiveArmorItem(itemName);
                }
            }

            if (MoonlighterAPConstants.ALL_EQUIPMENT_NAMES.Contains(itemName)) return itemName;

            // DecorativeItemMaster items are items but not inventory items, so don't try to receive them.
            ItemMaster item = GetItem(itemName);
            if (item != null && item is not DecorativeItemMaster) return itemName;

            return null;
        }

        internal static string GetProgressiveWeaponItem(string itemName)
        {
            string[] parts = itemName.Split(' ');
            string type = parts[1];

            if (type != "Power" && type != "Elemental")
                throw new Exception("Received invalid progressive weapon.");

            string weaponType = String.Join(" ", parts.Skip(2));

            var weaponObj = MoonlighterAPConstants.GetWeaponObject(weaponType) ?? throw new Exception("Received invalid progressive weapon.");
            int count = ItemHelper.GetReceivedItemCount(itemName);

            if (count > 3) count = 3;

            string weaponName = (type == "Power" ? weaponObj.powerPath : weaponObj.elementalPath)[count];
            return $"{weaponName} {weaponObj.name}";
        }

        internal static string GetProgressiveArmorItem(string itemName)
        {
            string[] numerals = ["", " II", " III", " IV"];

            string[] parts = itemName.Split(' ');

            string armorType = String.Join(" ", parts.Skip(1));

            int count = ItemHelper.GetReceivedItemCount(itemName);

            if (count > 3) count = 3;

            return armorType + numerals[count];
        }

        internal static void ClearInventory()
        {
            HeroMerchantInventory inventory = HeroMerchant.Instance.heroMerchantInventory;

            int i = 0;
            foreach (InventorySlot slot in inventory.slots)
            {
                i++;
                MoonlighterAPMod.APInfo($"Removing item {i}");

                if (slot.item == null) continue;
                inventory.TryRemoveItem(slot.item);
            }

            foreach (ItemStack item in inventory.equippedItems.ToArray())
            {
                MoonlighterAPMod.APInfo($"Removing item {i}");

                if (item == null) continue;
                if (item.master is AbilityItemMaster) continue;

                inventory.TryRemoveItem(item);
            }
        }

        internal static void EquipStartingWeapon(string startingWeapon)
        {
            HeroMerchantInventory inventory = HeroMerchant.Instance.heroMerchantInventory;

            ItemMaster item = GetItem(startingWeapon);
            if (item == null) return;

            ItemStack itemStack = ItemStack.Create(item);

            inventory.SetEquippedItemByType(itemStack, HeroMerchantInventory.EquipmentSlot.Weapon1);
        }
    }
}
