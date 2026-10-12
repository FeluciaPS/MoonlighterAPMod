using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod.Wrappers;
using System.Collections.Generic;

namespace MoonlighterAPMod.Patches.Items
{
    internal class DecorationPatches
    {
        [HarmonyPatch(typeof(DecorativePanel))]
        [HarmonyPatch("RefreshDecorativeItems")]
        internal static class ItemReceivePatch
        {
            internal static bool Prefix(DecorativePanel __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;
                GameSlot currentGameSlot = GameManager.Instance.currentGameSlot;

                DecorativePanelWrapper panel = new(__instance);

                List<ItemMaster> ownedItems = ItemDatabase.GetItems(x => x is DecorativeItemMaster && ItemHelper.HasItem(x.displayName));

                panel.DecorativeItems.Clear();
                foreach (ItemMaster item in ownedItems)
                {
                    panel.DecorativeItems.Add(item as DecorativeItemMaster);
                }

                panel.ValidItems = panel.DecorativeItems.FindAll(x => x.place == panel.DecorativeSlot.place);
                panel.MaxPage = panel.ValidItems.Count / __instance.decorativeSlots.Count;

                return false;
            }
        }
    }
}
