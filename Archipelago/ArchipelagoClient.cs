using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoonlighterAPMod.Archipelago
{
    internal class ArchipelagoClient
    {
        // TODO: Clean this up
        private readonly ArchipelagoConnector _connector = new();
        private ILocationCheckHelper Locations { get { return _connector.Session.Locations; } }
        private IReceivedItemsHelper Items { get { return _connector.Session.Items; } }
        internal int Slot { get { return _connector.Session.ConnectionInfo.Slot;  } }
        internal string Seed { get { return _connector.Session.RoomState.Seed; } }

        internal static bool IsClientConnected { get { return MoonlighterAPMod.client._connector.IsConnected; } }
        internal static Dictionary<string, object> SlotData { get { return MoonlighterAPMod.client._connector.SlotData; } }

        internal async Task Connect(string hostname, int port, string slotName, string password = "")
        {
            await _connector.Connect(hostname, port, slotName, password);
            if (IsClientConnected)
            {
                Items.ItemReceived += OnItemReceived;
            }
        }

        internal async Task Disconnect()
        {
            await _connector.Disconnect();
        }

        internal void SetDeathLink(bool deathLink)
        {
            if (!IsClientConnected) return;
            _connector.SetDeathLink(deathLink);
        }

        /**
         * Location related functions
         */
        internal async Task SendLocation(long locationId)
        {
            await SendLocations([locationId]);
        }

        internal async Task SendLocations(long[] locationIds)
        {
            if (!IsClientConnected) return;

            await Locations.CompleteLocationChecksAsync(locationIds);
        }

        internal async Task ScoutLocations(long[] locationIds)
        {
            if (!IsClientConnected) return;

            // Check if the data we're looking for is already saved
            Dictionary<long, ScoutedItemInfo> locationData = LocationScoutCache.Get(locationIds);
            if (locationData == null)
            {
                // If not, ask the server for the data, and save it for later
                locationData = await _connector.Session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, locationIds);
                LocationScoutCache.Set(locationData);
            }

            // TODO: Continue this
        }

        internal bool IsLocationChecked(long locationId)
        {
            if (!IsClientConnected) return false;

            return Locations.AllLocationsChecked.Contains(locationId);
        }

        /**
         * Item related functions
         */ 
        private void OnItemReceived(ReceivedItemsHelper helper)
        {
            // TODO: This may not belong here
            while (Items.PeekItem() != null)
            {
                ItemInfo item = Items.DequeueItem();
                if (item.Flags.HasFlag(ItemFlags.Trap)) ItemHelper.ReceiveTrap(item.ItemId, item.ItemName);
                else ItemHelper.ReceiveItem(item.ItemId, item.ItemName);
            }
        }

        internal ReadOnlyCollection<ItemInfo> GetAllReceivedItems()
        {
            return Items.AllItemsReceived;
        }

        /**
         * Goal functions
         */
        internal void Goal()
        {
            _connector.Session.SetGoalAchieved();
        }
    }
}
