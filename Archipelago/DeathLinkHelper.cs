using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using MoonlighterAPMod.Mod;

namespace MoonlighterAPMod.Archipelago
{
    internal class DeathLinkHelper
    {
        internal static DeathLinkService Service { get; private set; }

        internal static void Start(ArchipelagoSession session)
        {
            Service = session.CreateDeathLinkService();
            Service.OnDeathLinkReceived += OnDeathLinkReceived;
            SetDeathLinkState();
        }

        internal static void SetDeathLinkState()
        {
            if (OptionsHelper.IsDeathLink) Service.EnableDeathLink();
            else Service.DisableDeathLink();
        }

        internal static void Stop()
        {
            Service = null;
        }

        internal static void OnDeathLinkReceived(DeathLink deathLinkObject)
        {
            DeathLinkManager.KillPlayer();
        }

        internal static void SendDeathLink(string message = null)
        {
            DeathLink deathLink = new(ArchipelagoClient.PlayerName, message);
            Service.SendDeathLink(deathLink);
        }
    }
}
