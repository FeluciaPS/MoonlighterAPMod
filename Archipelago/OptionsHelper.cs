using Newtonsoft.Json.Linq;
using static MoonlighterAPMod.Archipelago.GoalHelper;

namespace MoonlighterAPMod.Archipelago
{
    internal class OptionsHelper
    {
        private static JObject Options { get { return (ArchipelagoClient.IsClientConnected) ? (JObject)ArchipelagoClient.SlotData["options"] : []; } }

        private static JToken GetOption(string name) => Options[name];

        // TODO: My eyes...
        internal static GoalType Goal { get { return (GoalType)(GetOption("goal")?.Value<int>() ?? -1); } }

        internal static bool IsDeathLink { get { return (GetOption("death_link")?.Value<int>() ?? 0) != 0; } }
    }
}
