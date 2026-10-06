using Newtonsoft.Json.Linq;
using static MoonlighterAPMod.Archipelago.GoalHelper;

namespace MoonlighterAPMod.Archipelago
{
    internal class OptionsHelper
    {
        private static JObject Options { get => ArchipelagoClient.IsClientConnected ? (JObject)ArchipelagoClient.SlotData["options"] : []; }

        private static JToken GetOption(string name) => Options[name];

        // TODO: My eyes...
        internal static GoalType Goal { get => (GoalType)(GetOption("goal")?.Value<int>() ?? -1); }

        internal static bool IsDeathLink { get => (GetOption("death_link")?.Value<int>() ?? 0) != 0; }

        internal static bool IsProgressiveFloors { get => (GetOption("progressive_dungeon_floors")?.Value<int>() ?? 0) != 0; }

        internal static bool RequireSale { get => (GetOption("require_sale")?.Value<int>() ?? 0) != 0; }

        internal static bool RequireBosses { get => (GetOption("require_bosses")?.Value<int>() ?? 0) != 0;  }
    }
}
