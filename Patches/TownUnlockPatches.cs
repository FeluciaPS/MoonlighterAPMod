using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using MoonlighterAPMod.Mod;
using static TownBoardShopPanel;

namespace MoonlighterAPMod.Patches
{
    internal class TownUnlockPatches
    {
        [HarmonyPatch(typeof(TownBoardStaffPanel))]
        [HarmonyPatch("IsAvailable")]
        internal static class CanBuyShopPersonPatch
        {
            internal static bool Prefix(TownBoardStaffPanel.TownBoardStaffEntry.Type type, ref bool __result)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;

                switch (type)
                {
                    case TownBoardStaffPanel.TownBoardStaffEntry.Type.Blacksmith:
                    case TownBoardStaffPanel.TownBoardStaffEntry.Type.Witch:
                    case TownBoardStaffPanel.TownBoardStaffEntry.Type.Hawker:
                        __result = WorldStateHelper.UnlockedDungeonCount() > 0;
                        break;
                    case TownBoardStaffPanel.TownBoardStaffEntry.Type.Rival:
                        __result = WorldStateHelper.UnlockedDungeonCount() > 1;
                        break;
                    case TownBoardStaffPanel.TownBoardStaffEntry.Type.Banker:
                        __result = WorldStateHelper.UnlockedDungeonCount() > 2;
                        break;
                    default:
                        __result = false;
                        break;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(TownBoardShopPanel))]
        [HarmonyPatch("IsAvailable")]
        internal static class CanBuyShopUpgradePatch
        {
            internal static bool Prefix(TownBoardShopEntry.Type type, int level, ref bool __result)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;

                __result = WorldStateHelper.UnlockedDungeonCount() >= level + 1;

                return false;
            }
        }
    }
}
