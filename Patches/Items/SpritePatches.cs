using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using UnityEngine;
using static MoonlighterAPMod.Mod.CustomItemModule;

namespace MoonlighterAPMod.Patches.Items
{
    internal class SpritePatches
    {
        [HarmonyPatch(typeof(ItemDatabase))]
        [HarmonyPatch(nameof(ItemDatabase.GetSprite))]
        internal static class SpriteReplacePatch
        {
            internal static bool Prefix(ref Sprite __result, ItemMaster master)
            {
                if (master is not ArchipelagoItem apItem)
                    return true;

                __result = SpriteLoader.GetSprite(apItem.customSpriteName);
                return false;
            }
        }
    }
}
