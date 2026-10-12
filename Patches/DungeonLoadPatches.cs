using HarmonyLib;
using MoonlighterAPMod.Archipelago;

namespace MoonlighterAPMod.Patches
{
    internal class DungeonLoadPatches
    {
        [HarmonyPatch(typeof(WorldDungeonDoor))]
        [HarmonyPatch(nameof(WorldDungeonDoor.LoadDungeon))]
        internal static class DungeonLoadPatch
        {
            internal static void Prefix(WorldDungeonDoor __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                if (__instance.dungeonType == ItemMaster.Culture.Unknown)
                {
                    GoalHelper.TrySendGoal(GoalHelper.GoalType.FifthDungeon);
                }
            }
        }

        [HarmonyPatch(typeof(GUIManager))]
        [HarmonyPatch(nameof(GUIManager.FadeOut))]
        internal static class FadeOutPatch
        {
            internal static void Prefix()
            {
                if (!ArchipelagoClient.IsInGameplay) return;
                if (!ArchipelagoClient.IsClientConnected) return;

                MoonlighterAPMod.client.ReceiveAllItems();
            }
        }
    }
}
