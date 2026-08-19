namespace MoonlighterAPMod.Archipelago
{
    internal class GoalHelper
    {
        internal enum GoalType
        {
            Unknown = -1,
            FifthDungeon = 0,
            PirateBoss = 1,
            Collector = 2
        }

        internal static void TrySendGoal(GoalType type)
        {
            if (!ArchipelagoClient.IsClientConnected) return;
            if (type != OptionsHelper.Goal) return;

            MoonlighterAPMod.APInfo($"Achieved player goal {type}");
            MoonlighterAPMod.client.Goal();
        }
    }
}
