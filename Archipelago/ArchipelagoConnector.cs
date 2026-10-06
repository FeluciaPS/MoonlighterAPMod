using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoonlighterAPMod.Archipelago
{
    internal class ArchipelagoConnector
    {
        internal ArchipelagoSession Session { get; private set; }
        internal Dictionary<string, object> SlotData { get; private set; }

        internal bool IsConnected => Session?.Socket.Connected ?? false;

        internal async Task Connect(string hostname, int port, string slotName, string password = "")
        {
            MoonlighterAPMod.APInfo($"Attempting to connect to {hostname}:{port} as {slotName}");
            
            // Create session
            Session = ArchipelagoSessionFactory.CreateSession(hostname, port);

            // I'd kill to figure this out without a try/catch
            try
            {
                await Session.ConnectAsync();
            }
            catch (Exception)
            {
                OnLoginFailure();
                return;
            }

            LoginResult result = await Session.LoginAsync(
                ModInfo.GAME_NAME,
                slotName,
                ItemsHandlingFlags.AllItems,
                new Version(0, 6, 7),
                // For future reference: You can connect as DeathLink directly from here if we ever make it a mod option
                // tags: ["DeathLink"],
                password: string.IsNullOrEmpty(password) ? null : password,
                requestSlotData: true
            );

            // Handle failure first so we can early-exit
            if (!result.Successful)
            {
                OnLoginFailure((LoginFailure)result);
                return;
            }

            OnLoginSuccessful((LoginSuccessful)result);
        }

        internal async Task Disconnect()
        {
            await Session.Socket.DisconnectAsync();
        }

        private void OnLoginFailure(LoginFailure failureResult = null)
        {
            string errorMsg = failureResult == null ? "Connection timed out." : string.Join(", ", failureResult.Errors);

            errorMsg = $"Connection failed: {errorMsg}";
            MoonlighterAPMod.APError(errorMsg);

            MoonlighterAPMod.connectionUI.ShowError(errorMsg);
            Session = null;
        }

        private void OnLoginSuccessful(LoginSuccessful successResult)
        {
            string successMessage = $"Connected to slot {successResult.Slot} for team {successResult.Team}!";
            MoonlighterAPMod.APInfo(successMessage);

            SlotData = successResult.SlotData;

            MoonlighterAPMod.connectionUI.ShowSuccess(successMessage);

            DeathLinkHelper.Start(Session);
        }

        internal void SetDeathLink(bool deathLink)
        {
            Session.ConnectionInfo.UpdateConnectionOptions(deathLink ? ["DeathLink"] : []);
        }
    }
}
