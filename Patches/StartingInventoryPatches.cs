using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using System.Collections.Generic;

namespace MoonlighterAPMod.Patches
{
    internal class StartingInventoryPatches
    {
        [HarmonyPatch(typeof(GameManager))]
        [HarmonyPatch("ResetSlotInfo")]
        internal static class NewGamePatch
        {
            private static void ChangeStartingItems(GameManager __instance)
            {
                var startingWeapon = ItemHelper.GetStartingWeapon();
                if (startingWeapon != null)
                    __instance.initialEquippedItems[0] = startingWeapon;
                else
                    __instance.initialEquippedItems.RemoveAt(0);

                
            }
            private static List<string> originalInitialEquippedItems;

            internal static void Prefix(GameManager __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                originalInitialEquippedItems = [.. __instance.initialEquippedItems];

                ChangeStartingItems(__instance);
            }

            internal static void Postfix(GameManager __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                __instance.initialEquippedItems = [.. originalInitialEquippedItems];
            }
        }

        [HarmonyPatch(typeof(TutorialScene3))]
        [HarmonyPatch("EndScene")]
        internal static class TutorialEndPatch
        {
            internal static void Prefix()
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                InventoryItemHandler.ClearInventory();
                
                var startingWeapon = ItemHelper.GetStartingWeapon();
                InventoryItemHandler.EquipStartingWeapon(startingWeapon);

                SaveManager.saveData.collectedItems = [startingWeapon];
                SaveManager.Save();
            }
        }

    }
}
