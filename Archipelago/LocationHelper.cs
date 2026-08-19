namespace MoonlighterAPMod.Archipelago
{
    internal class LocationHelper
    {
        internal static void SendLocation(long locationId)
        {
            MoonlighterAPMod.client.SendLocation(locationId);
        }

        internal static bool IsChecked(long locationId)
        {
            return MoonlighterAPMod.client.IsLocationChecked(locationId);
        }
    }
}
