using Archipelago.MultiClient.Net.Models;
using System.Collections.Generic;

namespace MoonlighterAPMod.Archipelago
{
    /**
     * Rule 1 of the universe: If something can be cached Felucia will cache it
     */
    internal static class LocationScoutCache
    {
        private class LocationScoutCacheEntry
        {
            private readonly Dictionary<long, ScoutedItemInfo> _entries = [];

            internal Dictionary<long, ScoutedItemInfo> Get(long[] locationIds)
            {
                Dictionary<long, ScoutedItemInfo> returnValue = [];
                foreach (long id in locationIds)
                {
                    if (!_entries.ContainsKey(id)) return null;
                    returnValue[id] = _entries[id];
                }

                return returnValue;
            }

            internal void Set(Dictionary<long, ScoutedItemInfo> entries)
            {
                foreach (long entry in entries.Keys)
                {
                    _entries[entry] = entries[entry];
                }
            }
        }

        private static string GetSlotKey()
        {
            if (!ArchipelagoClient.IsClientConnected) return null;

            return $"{MoonlighterAPMod.client.Seed}-{MoonlighterAPMod.client.Slot}";
        }

        private static readonly Dictionary<string, LocationScoutCacheEntry> _cache = [];

        internal static Dictionary<long, ScoutedItemInfo> Get(long[] locationIds)
        {
            string slotKey = GetSlotKey();
            if (slotKey == null) return null;
            if (!_cache.ContainsKey(slotKey)) return null;
            return _cache[slotKey].Get(locationIds);
        }

        internal static void Set(Dictionary<long, ScoutedItemInfo> locationInfo)
        {
            string slotKey = GetSlotKey();
            if (slotKey == null) return;
            if (!_cache.ContainsKey(slotKey)) _cache[slotKey] = new();
            _cache[slotKey].Set(locationInfo);
        }
    }
}
