using HarmonyLib;
using I2.Loc;
using System.Collections.Generic;

namespace MoonlighterAPMod.Patches
{
    internal class DialoguePatches
    {
        // TODO: Maybe move these elsewhere?
        internal static Dictionary<string, string> dialogueReplacements = new()
        {
            ["TUTORIAL/GREEN_MAN_SCENE_8_6"] = "By the way, the world is looking a little different than you're used to.(n)"
                                             + "There's these weird [i]Archipelago[i] items laying around.(n)"
                                             + "I have no use for this. Take it."
        };

        [HarmonyPatch(typeof(LocalizationManager))]
        [HarmonyPatch(nameof(LocalizationManager.GetTranslation))]
        internal static class TranslationOverridePatch
        {
            internal static bool Prefix(ref string __result, string Term)
            {
                if (Term.StartsWith("MOON-AP::"))
                    __result = Term.Substring(9);
                else if (dialogueReplacements.ContainsKey(Term))
                    __result = dialogueReplacements[Term];
                else
                    return true;

                return false;
            }
        }
    }
}
