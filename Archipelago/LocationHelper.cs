using Archipelago.MultiClient.Net.Models;
using MoonlighterAPMod.Mod;
using System.Collections.Generic;

namespace MoonlighterAPMod.Archipelago
{
    internal class LocationHelper
    {
        internal static void SendLocation(long locationId)
        {
            MoonlighterAPMod.client.SendLocation(locationId);
        }

        internal static void SendLocation(string locationName)
        {
            MoonlighterAPMod.client.SendLocation(locationName);
        }

        internal static int CountChecked(long[] locationIds)
        {
            int n = 0;
            foreach (long id in locationIds)
            {
                if (IsChecked(id)) n++;
            }

            return n;
        }

        internal static bool IsChecked(long locationId)
        {
            return MoonlighterAPMod.client.IsLocationChecked(locationId);
        }

        internal static bool IsChecked(string locationName)
        {
            return IsChecked(GetLocationId(locationName));
        }

        internal static string GetLocationName(long locationId)
        {
            return MoonlighterAPMod.client.GetLocationName(locationId);
        }

        internal static long GetLocationId(string locationName)
        {
            return MoonlighterAPMod.client.GetLocationId(locationName);
        }

        internal static void HandleScoutedLocations(Dictionary<long, ScoutedItemInfo> locationData)
        {
            CustomItemModule.CreateItemsFromScout(locationData);
        }
    }
}
