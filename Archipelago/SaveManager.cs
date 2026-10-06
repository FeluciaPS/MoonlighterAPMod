using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MoonlighterAPMod.Archipelago
{
    internal static class SaveManager
    {
        internal class SaveData
        {
            [JsonProperty]
            internal List<string> collectedItems = [];

            [JsonProperty]
            internal List<string> pendingTraps = [];

            [JsonProperty]
            internal List<string> pendingItems = [];
        }

        internal static SaveData saveData;

        internal static string RootPath
        {
            get
            {
                // Create folder if it doesn't exist
                string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string dir = Path.Combine(root, "Felucia Mods", "MoonlighterAPMod", "SaveData");
                Directory.CreateDirectory(dir);

                return dir;
            }
        }

        internal static int SaveSeed { get => GameManager.Instance?.currentGameSlot?.seed ?? -1; }
        internal static string ArchipelagoSeed { get => MoonlighterAPMod.client.Seed; }
        internal static int Slot { get => MoonlighterAPMod.client.Slot; }
        internal readonly static string FileExtension = ".sav";

        internal static string FileName { 
            get => SaveSeed + "-" + ArchipelagoSeed + "-" + Slot + FileExtension;
        }
        
        internal static string FilePath { get => Path.Combine(RootPath, FileName); }

        internal static bool Write(string path, string data)
        {
            string tmp = path + ".tmp";
            string bak = path + ".bak";

            try
            {
                using (var stream = new FileStream(tmp, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(data);
                    writer.Flush();
                    stream.Flush(true);
                }

                if (File.Exists(path))
                    File.Replace(tmp, path, bak);
                else
                    File.Move(tmp, path);

                return true;
            }
            catch (Exception e)
            {
                MoonlighterAPMod.APError($"Failed to write save data: {e.Message}");

                try {
                    if (File.Exists(tmp)) File.Delete(tmp); 
                } 
                catch { }

                return false;
            }
        }
        internal static void Save<T>(T data)
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            Write(FilePath, jsonData);
        }

        internal static T Load<T>(T defaultValue = default)
        {
            if (!File.Exists(FilePath))
                return defaultValue;

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));
        }

        internal static void ReceiveItem(string itemName)
        {
            saveData.collectedItems.Add(itemName);
        }

        internal static void Load()
        {
            saveData = Load(new SaveData());
        }

        internal static void Save()
        {
            if (saveData == null) return;
            Save(saveData);
        }
    }

}
