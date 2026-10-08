using MoonlighterAPMod.Archipelago;

namespace MoonlighterAPMod.Mod
{
    internal class WorldStateHelper
    {
        internal static bool IsBossDefeated(ItemMaster.Culture culture)
        {
            // TODO: Create constant instead of hardcoded location ID
            long locationId = ((int)culture) * 100 + 10;

            return CustomItemModule.HasLocation(locationId);
        }
    }
}
