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
    }
}
