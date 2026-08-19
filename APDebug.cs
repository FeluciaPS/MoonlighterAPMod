namespace MoonlighterAPMod
{
    internal class APDebug
    {
        private static int n;

        internal static void StartDebug()
        {
            n = 0;
        }

        internal static void Debug()
        {
            MoonlighterAPMod.APInfo($"Debug message: {++n}");
        }
    }
}
