using MoonlighterAPMod.Archipelago;
using System;

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

        internal static int UnlockedDungeonCount()
        {
            ItemMaster.Culture[] cultures = [
                ItemMaster.Culture.Golem,
                ItemMaster.Culture.Forest,
                ItemMaster.Culture.Desert,
                ItemMaster.Culture.Tech
            ];

            int n = 0;
            
            foreach (ItemMaster.Culture culture in cultures)
            {
                bool canReach = ItemHelper.CanReachFloor(culture, 1);
                if (canReach) n++;
            }

            return n;
        }
    }
}
