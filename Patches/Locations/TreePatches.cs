using HarmonyLib;
using MoonlighterAPMod.Mod;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class TreePatches
    {
        [HarmonyPatch(typeof(TreeBehaviour))]
        [HarmonyPatch(nameof(TreeBehaviour.OnTriggerEnter2D))]
        internal static class TreeMoneyPatch
        {
            private static float n;
            internal static void Prefix(TreeBehaviour __instance, float ___timesTreeHitDropped)
            {
                n = ___timesTreeHitDropped;
            }

            internal static void Postfix(TreeBehaviour __instance, float ___timesTreeHitDropped)
            {
                if (___timesTreeHitDropped != n)
                {
                    CustomItemModule.DropLocationItem("Tree Money", __instance.transform.position);
                }
            }
        }
    }
}
