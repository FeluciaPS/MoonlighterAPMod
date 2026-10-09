using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using MoonlighterAPMod.Patches.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static MoonlighterAPMod.Mod.Wrappers.BlacksmithWrappers;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class BlacksmithLocationPatches
    {
        // TODO: This file is a bit of a trainwreck and I don't feel like cleaning it up right now

        [HarmonyPatch(typeof(BlacksmithPanel))]
        [HarmonyPatch("RefreshRecipesByTab")]
        internal static class RefreshRecipesPatch
        {
            private static readonly FieldInfo recipesField = typeof(BlacksmithPanel).GetField(
                "recipesByTab", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            private static readonly FieldInfo tabsField = typeof(TreeItemSelector).GetField(
                "_tabs", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            internal static void RefreshTab(BlacksmithPanel __instance, int tabIndex)
            {
                var recipes = (List<List<List<Recipe>>>)recipesField.GetValue(__instance);
                UpdateRecipes(recipes);

                TreeItemSelector itemSelector = __instance.itemSelector;
                var tabs = (IList)tabsField.GetValue(itemSelector);
                var tab = new TabInfoWrapper(tabs[tabIndex]);

                foreach (object rawPage in tab.Pages)
                {
                    var page = new PageInfoWrapper(rawPage);

                    List<ItemMaster> items = page.Items;
                    UpdateItems(items);

                    List<bool> unlocked = page.Unlocked;
                    UpdateItemsUnlocked(unlocked, page.IsWeaponFamily);

                    List<bool> seen = page.Seen;
                    UpdateItemsSeen(seen, items);
                }

                itemSelector.SelectTab(0);
            }

            internal static string TranslateItemName(string originalName)
            {
                string name = originalName.Replace("Hardened Steel", "Steel");
                if (name.Contains("Sword") && !name.Contains("Big Sword")) name = name.Replace("Sword", "Short Sword");
                name = name.Replace("Tesla", "Vampire");

                return name;
            }

            internal static void UpdateItems(List<ItemMaster> items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemMaster item = items[i];
                    if (item is CustomItemModule.ArchipelagoItem) continue;

                    string name = TranslateItemName(item.name);

                    ItemMaster newItem = CustomItemModule.GetItemByName($"Craft {name}");
                    if (newItem != null)
                    {
                        items[i] = newItem;
                    }
                }
            }

            internal static void UpdateItemsUnlocked(List<bool> items, bool isWeaponFamily)
            {
                int requiredFloor = isWeaponFamily ? 2 : 1;
                requiredFloor = 1; // TODO: Think about this later
                if (isWeaponFamily) items[0] = ItemHelper.CanReachFloor(ItemMaster.Culture.Golem, 1);

                for (int i = 0; i < items.Count; i++)
                {
                    if (i == 0 && isWeaponFamily) continue;

                    int index = isWeaponFamily ? i - 1 : i;
                    ItemMaster.Culture culture = (ItemMaster.Culture)(index % 4 + 1);

                    items[i] = ItemHelper.CanReachFloor(culture, requiredFloor);
                }
            }

            internal static void UpdateItemsSeen(List<bool> seen, List<ItemMaster> items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] is not CustomItemModule.ArchipelagoItem item) continue;
                    //seen[i] = true;
                }
            }

            internal static void UpdateRecipe(Recipe recipe)
            {
                FieldInfo _itemsField = typeof(Recipe).GetField(
                    "_craftedItem",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                FieldInfo _costField = typeof(Recipe).GetField(
                    "_totalCost",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                string apItemName = TranslateItemName(recipe.craftedItemName);

                var _craftedItem = CustomItemModule.GetItemByName($"Craft {apItemName}");
                recipe.craftedItemName = $"Craft {apItemName}";
                recipe.craftedItemNameKey = _craftedItem.nameKey;

                int cost = (int)_costField.GetValue(recipe);
                cost /= 2;

                if (!OptionsHelper.RequireSale)
                {
                    _costField.SetValue(recipe, cost);
                    recipe.labourCostGold = cost;
                }
                else
                {
                    MoonlighterAPMod.APInfo(cost);
                    _craftedItem.goldValue = cost;
                }

                _itemsField.SetValue(recipe, _craftedItem);
            }

            internal static Recipe CopyRecipe(Recipe recipe)
            {
                FieldInfo _itemField = typeof(RecipeIngredient).GetField(
                    "_requiredItem",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                Recipe newRecipe = recipe.Copy();
                foreach (RecipeIngredient ingredient in newRecipe.ingredients)
                {
                    ItemMaster newIngredient = InventoryItemHandler.GetItem(ingredient.GetRequiredItem().displayName);
                    _itemField.SetValue(ingredient, newIngredient);
                }
                return newRecipe;
            }

            internal static void UpdateRecipes(List<List<List<Recipe>>> recipes)
            {
                foreach (var tab in recipes)
                {
                    foreach (var page in tab)
                    {
                        for (int recipeId = 0; recipeId < page.Count; recipeId++)
                        {
                            Recipe originalRecipe = page[recipeId];
                            Recipe recipe = null;
                            if (originalRecipe.ingredients.Exists(x => x.GetRequiredItem() is EquipmentItemMaster))
                            {
                                recipe = CopyRecipe(originalRecipe);
                                recipe.ingredients.RemoveAll(x => x.GetRequiredItem() is EquipmentItemMaster);
                                
                            }
                            if (!originalRecipe.craftedItemName.StartsWith("Craft "))
                            {
                                recipe ??= CopyRecipe(originalRecipe);
                                UpdateRecipe(recipe);
                            }
                            if (recipe == null) continue;

                            page[recipeId] = recipe;
                        }
                    }
                }
            }

            public static void Postfix(BlacksmithPanel __instance, int tab)
            {
                if (!ArchipelagoClient.IsClientConnected) 
                    return;

                if (tab != -1)
                {
                    RefreshTab(__instance, tab);
                    return;
                }

                for (int i = 0; i <= 5; i++)
                {
                    RefreshTab(__instance, i);
                }
            }
        }

        [HarmonyPatch(typeof(TreeItemSelector))]
        [HarmonyPatch(nameof(TreeItemSelector.SelectPage))]
        internal static class PageChangePatch
        {
            internal static void Postfix(TreeItemSelector __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                var panel = __instance.gameObject.GetComponentInParent(typeof(BlacksmithPanel));

                // Only trigger on blacksmith
                if (panel == null) return;

                foreach (InventorySlotGUI slot in __instance.slots)
                {
                    if (slot.imageItem.color.r == 0) continue;
                    ItemMaster item = slot.item;
                    if (item is not CustomItemModule.ArchipelagoItem apItem) continue;

                    float a = 1f;
                    if (CustomItemModule.HasLocation(apItem.locationName)) a = 0.2f;
                    slot.imageItem.color = new UnityEngine.Color(1, 1, 1, a);
                }
            }
        }

        [HarmonyPatch(typeof(UpgradeDetailsPanel))]
        [HarmonyPatch(nameof(UpgradeDetailsPanel.DetailItem), [typeof(ItemMaster), typeof(Recipe), typeof(float)])]
        internal static class DetailItemTranspiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // Please for the love of all that is holy never make me write a transpiler again...
                foreach (CodeInstruction instr in instructions)
                {
                    if (instr.opcode == OpCodes.Ldfld && instr.operand is FieldInfo field && field.Name == "stats")
                    {
                        MethodInfo newOperation = AccessTools.Method(typeof(DetailItemTranspiler), nameof(GetStatsHook));
                        yield return new CodeInstruction(OpCodes.Call, newOperation);
                    }
                    else
                    {
                        yield return instr;
                    }
                }
            }

            internal static StatsModificator GetStatsHook(EquipmentItemMaster item)
            {
                if (item == null)
                {
                    StatsModificator stats = new()
                    {
                        health = 0,
                        strength = 0,
                        armor = 0,
                        speed = 0,
                        intelligence = 0
                    };
                    return stats;
                }

                return item.stats;
            }
        }
    }
}
