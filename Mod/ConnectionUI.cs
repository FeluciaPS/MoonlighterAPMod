using MoonlighterAPMod.Archipelago;
using System;
using UnityEngine;

namespace MoonlighterAPMod.Mod
{
    internal class ConnectionUI : MonoBehaviour
    {
        private Rect windowRect = new(20, 20, 400, 400);

        private string host = "localhost";
        private string port = "38281";
        private string slot = "Player1";
        private string password = "";

        private string statusMessage = "";

        internal bool showWindow = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (showWindow) Hide();
                else Show();
            }
        }

        private void OnGUI()
        {
            if (!showWindow) return;

            windowRect = GUILayout.Window(0, windowRect, DrawWindow, "Archipelago Connection [F1 to show/hide]");
        }

        internal void Show()
        {
            showWindow = true;
            Cursor.visible = true;
        }

        internal void Hide()
        {
            showWindow = false;
            Cursor.visible = false;
        }

        private void DrawWindow(int id)
        {
            GUILayout.Label("Host");
            host = GUILayout.TextField(host);

            GUILayout.Label("Port");
            port = GUILayout.TextField(port);

            GUILayout.Label("Slot Name");
            slot = GUILayout.TextField(slot);

            GUILayout.Label("Password (optional)");
            password = GUILayout.PasswordField(password, '*');

            GUILayout.Space(10);

            if (!ArchipelagoClient.IsClientConnected)
            {
                if (GUILayout.Button("Connect"))
                    Connect();
            }
            else
            {
                if (GUILayout.Button("Disconnect"))
                    Disconnect();
            }

            GUILayout.Space(10);
            GUILayout.Label(statusMessage);

            GUI.DragWindow();
        }

        private void Connect()
        {
            Int32.TryParse(port, out int result);
            MoonlighterAPMod.client.Connect(host, result, slot, password);
        }

        private void Disconnect()
        {
            MoonlighterAPMod.client.Disconnect();
            statusMessage = "Disconnected.";
        }

        internal void ShowError(string errorMsg)
        {
            statusMessage = errorMsg;
        }

        internal void ShowSuccess(string successMsg = "")
        {
            statusMessage = successMsg ?? "Connected.";
        }
    }
}
