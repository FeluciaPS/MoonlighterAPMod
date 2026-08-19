namespace MoonlighterAPMod.Archipelago
{
    internal class ItemHelper
    {
        internal static bool HasItem(string itemName)
        {
            return HasItems([itemName]);
        }

        internal static bool HasItem(long itemId)
        {
            return HasItems([itemId]);
        }

        internal static bool HasItems(string[] itemNames)
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            foreach (string name in itemNames)
            {
                if (items.FirstOrDefault(item => item.ItemName == name) == null) return false;
            }

            return true;
        }

        internal static bool HasItems(long[] itemIds)
        {
            var items = MoonlighterAPMod.client.GetAllReceivedItems();
            foreach (long id in itemIds)
            {
                if (items.FirstOrDefault(item => item.ItemId == id) == null) return false;
            }

            return true;
        }

        internal static void ReceiveItem(long itemId, string itemName)
        {
            // TODO: Nothing uses this yet so it's fine
        }

        internal static void ReceiveTrap(long itemId, string itemName)
        {
            // TODO: Nothing uses this yet so it's fine
        }
    }
}
