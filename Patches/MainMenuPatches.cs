using HarmonyLib;
using MoonlighterAPMod.Mod;

namespace MoonlighterAPMod.Patches
{
    internal class MainMenuPatches
    {
        [HarmonyPatch(typeof(MainMenuPanel))]
        [HarmonyPatch("CloseGamepadPanel")]
        internal static class ArchipelagoMenuButtonPatch
        {
            // Using a variable so visual studio doesn't scream about unreachable code because I hate that stuff
            private static readonly bool finishLater = true;
            internal static void Prefix(MainMenuPanel __instance)
            {
                UIManager.Instance.CreateOrUpdateVersionLabel(__instance.transform);

                // Work in progress stuff I don't want to finish right now
                if (!finishLater)
                {
                    UIManager.Instance.CreateOrUpdateArchipelagoMenuButton(__instance);
                    UIManager.Instance.CreateOptionsPanel();
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuPanel))]
        [HarmonyPatch("PrepareMenuButtons")]
        internal static class ArchipelagoConnectionUIPatch
        {
            internal static void Prefix()
            {
                MoonlighterAPMod.connectionUI.Show();
            }
        }
    }
}
