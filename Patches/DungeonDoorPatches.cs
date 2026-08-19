using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using UnityEngine;

namespace MoonlighterAPMod.Patches
{
    internal class DungeonDoorPatches
    {
        [HarmonyPatch(typeof(WorldDungeonDoor))]
        [HarmonyPatch(nameof(WorldDungeonDoor.UpdateByCulturesDefeated))]
        internal static class DoorOpenPatch
        {
            private static void SetState(WorldDungeonDoor __instance, bool state)
            {
                if (state)
                {
                    __instance.UnlockDoor();
                }
                else
                {
                    __instance.LockDoor();
                }
            }

            private static void SetUnknownDungeonState(WorldDungeonDoor __instance)
            {
                MoonlighterAPMod.APInfo($"Placing keys in 5th dungeon door");

                GameObject.Find("GolemLock").SetActive(value: ItemHelper.HasItem("Golem Key"));
                GameObject.Find("ForestLock").SetActive(value: ItemHelper.HasItem("Forest Key"));
                GameObject.Find("DesertLock").SetActive(value: ItemHelper.HasItem("Desert Key"));
                GameObject.Find("TechLock").SetActive(value: ItemHelper.HasItem("Tech Key"));

                bool hasUnknownDungeon = ItemHelper.HasItems(["Golem Key", "Forest Key", "Desert Key", "Tech Key"]);
                SetState(__instance, hasUnknownDungeon);
            }

            internal static bool Prefix(WorldDungeonDoor __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;

                MoonlighterAPMod.APInfo($"Setting door state for {__instance.dungeonType}");
                switch (__instance.dungeonType)
                {
                    case ItemMaster.Culture.Golem:
                        SetState(__instance, ItemHelper.HasItem("Unlock Golem Dungeon"));
                        break;
                    case ItemMaster.Culture.Forest:
                        SetState(__instance, ItemHelper.HasItem("Unlock Forest Dungeon"));
                        break;
                    case ItemMaster.Culture.Desert:
                        SetState(__instance, ItemHelper.HasItem("Unlock Desert Dungeon"));
                        break;
                    case ItemMaster.Culture.Tech:
                        SetState(__instance, ItemHelper.HasItem("Unlock Tech Dungeon"));
                        break;
                    case ItemMaster.Culture.Unknown:
                        SetUnknownDungeonState(__instance);
                        break;
                }
                return false;
            }

        }
    }
}
