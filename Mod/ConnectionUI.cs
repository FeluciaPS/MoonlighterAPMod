using MoonlighterAPMod.Archipelago;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace MoonlighterAPMod.Mod
{
    internal class ConnectionUI : MonoBehaviour
    {
        private class ConnectionInfo
        {
            [JsonProperty]
            internal string host = "localhost";
            [JsonProperty]
            internal string port = "38281";
            [JsonProperty]
            internal string slot = "Player1";
            [JsonProperty]
            internal string password = "";

            private static readonly string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            private static readonly string dir = Path.Combine(root, "Felucia Mods", "MoonlighterAPMod");
            private static readonly string FilePath = Path.Combine(dir, "ConnectionInfo.sav");

            private static ConnectionInfo _instance;
            internal static ConnectionInfo Instance { 
                get 
                {
                    _instance ??= Load();
                    return _instance;
                }
            }

            internal static ConnectionInfo Load()
            {
                ConnectionInfo data = SaveManager.Load(new ConnectionInfo(), FilePath);
                return data;
            }

            internal static void Save()
            {
                SaveManager.Save(Instance, FilePath);
            }
        }

        private Rect windowRect = new(20, 20, 400, 400);

        private string statusMessage = "";

        internal bool showWindow = false;

        public string Host { get => ConnectionInfo.Instance.host; set => ConnectionInfo.Instance.host = value; }
        public string Port { get => ConnectionInfo.Instance.port; set => ConnectionInfo.Instance.port = value; }
        public string Slot { get => ConnectionInfo.Instance.slot; set => ConnectionInfo.Instance.slot = value; }
        public string Password { get => ConnectionInfo.Instance.password; set => ConnectionInfo.Instance.password = value; }

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
            Host = GUILayout.TextField(Host);

            GUILayout.Label("Port");
            Port = GUILayout.TextField(Port);

            GUILayout.Label("Slot Name");
            Slot = GUILayout.TextField(Slot);

            GUILayout.Label("Password (optional)");
            Password = GUILayout.PasswordField(Password, '*');

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
            Int32.TryParse(Port, out int result);
            MoonlighterAPMod.client.Connect(Host, result, Slot, Password);
        }

        private void Disconnect()
        {
            MoonlighterAPMod.client.Disconnect();
            statusMessage = "Disconnected.";
        }

        internal void OnError(string errorMsg)
        {
            statusMessage = errorMsg;
        }

        internal void OnSuccess(string successMsg = "")
        {
            ConnectionInfo.Save();
            statusMessage = successMsg ?? "Connected.";
        }
    }
}
