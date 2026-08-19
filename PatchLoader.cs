using HarmonyLib;
using System.Reflection;

namespace MoonlighterAPMod
{
    internal class PatchLoader
    {
        internal static Harmony harmony;
        internal static void LoadPatches()
        {
            MoonlighterAPMod.APInfo($"Loading patches for {ModInfo.PLUGIN_GUID}...");

            harmony = new Harmony(ModInfo.PLUGIN_GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MoonlighterAPMod.APInfo($"Patches for {ModInfo.PLUGIN_GUID} loaded!");
        }
    }
}
