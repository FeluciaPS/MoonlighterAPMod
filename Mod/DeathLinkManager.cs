namespace MoonlighterAPMod.Mod
{
    internal class DeathLinkManager
    {
        internal static void KillPlayer()
        {
            MoonlighterAPMod.APWarning("Death Link received, but Death Link is disabled.");
            return;
            var stats = HeroMerchant.Instance.heroMerchantStats;
            var merchant = HeroMerchant.Instance;
            stats.absorption.Value = 0;
            stats.shield.Value = 0;

            var damage = 99999999;

            merchant.DealDamageToPlayer(damage, "DamageOverTime");

            if (GameManager.Instance.IsTownSceneLoaded())
            {
                UIManager.Instance.RunOverworldDeath();
            }
        }
    }
}
