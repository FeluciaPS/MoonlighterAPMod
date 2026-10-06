using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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
        internal string Game { get { return _connector.Session.ConnectionInfo.Game; } }
        internal static string PlayerName { get { return MoonlighterAPMod.client._connector.Session.Players.ActivePlayer.Name; } }
        internal static bool IsClientConnected { get { return MoonlighterAPMod.client._connector.IsConnected; } }
        internal static bool IsInGameplay { get { return CheckGameplayActive(); } }
        internal static Dictionary<string, object> SlotData { get { return MoonlighterAPMod.client._connector.SlotData; } }


        private static bool CheckGameplayActive()
        {
            int id = SceneUtility.GetBuildIndexByScenePath("Scenes/MainMenu");
            bool isMenuLoaded = SceneManager.GetSceneByBuildIndex(id).isLoaded;

            return !isMenuLoaded;
        }

        internal async Task Connect(string hostname, int port, string slotName, string password = "")
        {
            await _connector.Connect(hostname, port, slotName, password);
            if (IsClientConnected)
            {
                if (IsInGameplay)
                {
                    SaveManager.Load();
                }

                Items.ItemReceived += OnItemReceived;

                ScoutAllLocations();
                ReceiveAllItems();
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

        internal async Task SendLocation(string locationName)
        {
            await SendLocations([locationName]);
        }

        internal async Task SendLocations(string[] locationNames)
        {
            long[] locationIds = [.. locationNames.Select(name => Locations.GetLocationIdFromName("Moonlighter", name))];

            await SendLocations(locationIds);
        }

        internal async Task SendLocation(long locationId)
        {
            await SendLocations([locationId]);
        }

        internal async Task SendLocations(long[] locationIds)
        {
            if (!IsClientConnected) return;

            MoonlighterAPMod.APInfo($"Sending locations: {string.Join(", ", locationIds)}");
            await Locations.CompleteLocationChecksAsync(locationIds);
        }

        internal async Task ScoutAllLocations()
        {
            await ScoutLocations([.. Locations.AllLocations]);
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

            LocationHelper.HandleScoutedLocations(locationData);
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
            ReceiveAllItems();
        }

        public void ReceiveAllItems()
        {
            // TODO: Verify this is generic code and not moonlighter-specific
            var receivedItems = GetAllReceivedItems().ToList();

            foreach (string item in SaveManager.saveData.collectedItems)
            {
                MoonlighterAPMod.APMessage($"Already received {item}. Don't receive again");
                int index = receivedItems.FindIndex(x => x.ItemName == item);
                if (index != -1)
                {
                    receivedItems.RemoveAt(index);
                }
            }

            while (receivedItems.Count > 0)
            {
                ItemInfo item = receivedItems.Get(0);
                receivedItems.RemoveAt(0);

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

        internal string GetLocationName(long locationId)
        {
            return Locations.GetLocationNameFromId(locationId);
        }

        internal long GetLocationId(string locationName)
        {
            return Locations.GetLocationIdFromName(Game, locationName);
        }
    }
}
