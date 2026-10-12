using HarmonyLib;
using MoonlighterAPMod.Archipelago;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace MoonlighterAPMod.Patches.Locations
{
    internal class DeathLinkPatches
    {
        [HarmonyPatch(typeof(HeroMerchant))]
        [HarmonyPatch(nameof(HeroMerchant.DealDamageToPlayer))]
        internal static class DamageFunctionTranspiler
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // Damn it I had to write another transpiler
                foreach (CodeInstruction instr in instructions)
                {
                    if (instr.opcode == OpCodes.Callvirt && instr.operand is MethodInfo method && method.Name == "IsTownSceneLoaded")
                    {
                        MethodInfo newOperation = AccessTools.Method(typeof(DamageFunctionTranspiler), nameof(IsTownSceneLoaded));
                        yield return new CodeInstruction(OpCodes.Call, newOperation);
                    }
                    else
                    {
                        yield return instr;
                    }
                }
            }

            internal static bool IsTownSceneLoaded(GameManager gameManager)
            {
                MoonlighterAPMod.APMessage("Overriding IsTownSceneLoaded in damage function");
                return false;
                if (ArchipelagoClient.IsClientConnected) return false;
                return gameManager.IsTownSceneLoaded();
            }
        }
    }
}
