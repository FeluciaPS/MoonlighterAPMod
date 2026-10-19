using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace MoonlighterAPMod.Patches
{
    internal class DungeonDoorPatches
    {
        [HarmonyPatch(typeof(TravelToDungeonZone))]
        [HarmonyPatch(nameof(TravelToDungeonZone.GoToDungeonEntrance), [])]
        internal static class TriggerDoorCheckPatch
        {
            private static void Prefix()
            {
                UnityEngine.Object.FindObjectOfType<TownCinematics>().UpdateDoorsByCultureDefeated();
            }
        }

        [HarmonyPatch(typeof(WorldDungeonDoor))]
        [HarmonyPatch(nameof(WorldDungeonDoor.UpdateByCulturesDefeated))]
        internal static class DoorOpenPatch
        {
            private static void SetState(WorldDungeonDoor __instance, bool state)
            {
                string stateString = state ? "Open" : "Closed";
                MoonlighterAPMod.APInfo($"Setting door state for {__instance.dungeonType}: {stateString}");
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

                string[] dungeons = ["Golem", "Forest", "Desert", "Tech"];

                foreach (string dungeon in dungeons)
                {
                    GameObject.Find($"{dungeon}Lock")?.SetActive(value: ItemHelper.HasItem($"{dungeon} Key"));
                    __instance.portalAnimator.SetTrigger($"{dungeon}BossDefeated");
                }

                bool hasUnknownDungeon = ItemHelper.HasItems(["Golem Key", "Forest Key", "Desert Key", "Tech Key"]);
                if (OptionsHelper.RequireBosses) hasUnknownDungeon = hasUnknownDungeon && LocationHelper.CountChecked([110, 210, 310, 410]) == 4;
                SetState(__instance, hasUnknownDungeon);
            }

            internal static bool Prefix(WorldDungeonDoor __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return true;
                
                
                switch (__instance.dungeonType)
                {
                    case ItemMaster.Culture.Unknown:
                        SetUnknownDungeonState(__instance);
                        break;

                    case ItemMaster.Culture.Golem:
                        if (GameManager.Instance.currentGameSlot.playedTutorial < 4)
                        {
                            SetState(__instance, true);
                            break;
                        }

                        goto case ItemMaster.Culture.Forest;

                    case ItemMaster.Culture.Forest:
                    case ItemMaster.Culture.Desert:
                    case ItemMaster.Culture.Tech:
                        SetState(__instance, ItemHelper.CanReachFloor(__instance.dungeonType, 1));
                        break;
                }
                return false;
            }

        }

        [HarmonyPatch(typeof(DungeonRoom))]
        [HarmonyPatch(nameof(DungeonRoom.EnterRoom))]
        internal static class RemoveNextLevelDoorPatch
        {
            internal static void Prefix(DungeonRoom __instance)
            {
                if (!ArchipelagoClient.IsClientConnected) return;

                bool wip = true;
                if (wip) return;

                int nextFloor = DungeonManager.Instance.level + 2;
                ItemMaster.Culture dungeonType = DungeonManager.Instance.culture;

                if (ItemHelper.CanReachFloor(dungeonType, nextFloor)) return;

                __instance.GetSpecialDoor()?.DisableDoor();
            }
        }

        [HarmonyPatch(typeof(DungeonRoom))]
        [HarmonyPatch(nameof(DungeonRoom.OpenAllDoors))]
        internal static class OpenAllDoorsTranspiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                int n = 0;
                foreach (CodeInstruction instr in instructions)
                {
                    if (instr.opcode == OpCodes.Ldfld && instr.operand is FieldInfo field && field.Name == "specialDoor")
                    {
                        if (n == 0)
                        {
                            n++;
                            yield return instr;
                            continue;
                        }

                        MethodInfo newOperation = AccessTools.Method(typeof(OpenAllDoorsTranspiler), nameof(GetDoorHook));
                        yield return new CodeInstruction(OpCodes.Call, newOperation);
                    }
                    else
                    {
                        yield return instr;
                    }
                }
            }

            internal static DungeonDoor GetDoorHook(DungeonRoom room)
            {
                DungeonDoor door = room.GetSpecialDoor();
                if (door is not NextLevelDoor) return door;

                if (!ArchipelagoClient.IsClientConnected) return door;

                int nextFloor = DungeonManager.Instance.level + 2;
                ItemMaster.Culture dungeonType = DungeonManager.Instance.culture;

                bool canProgress = ItemHelper.CanReachFloor(dungeonType, nextFloor);

                if (canProgress) return door;

                return null;
            }
        }
    }
}
