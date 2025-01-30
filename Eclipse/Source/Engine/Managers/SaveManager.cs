using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;
using Eclipse.Engine.Core;

namespace Eclipse.Engine.Managers
{
    internal class SaveData
    {
        public float PlayerHealth { get; set; } = 100f;
        public Vector2 PlayerPosition { get; set; }
        public int CurrentLevel { get; set; } = 1;
        public DateTime LastSaveTime { get; set; } = DateTime.Now;
        public int SaveVersion { get; set; } = 1;
    }
    internal class SaveManager : Singleton<SaveManager>
    {

        private readonly string _saveDirectory;

        private SaveManager()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _saveDirectory = Path.Combine(appData, "YourGameName", "Saves");
            Directory.CreateDirectory(_saveDirectory);
        }

        private string GetSavePath(int slot) => Path.Combine(_saveDirectory, $"save_{slot}.json");

        public void SaveGame(SaveData saveData, int slot = 0)
        {
            try
            {
                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(GetSavePath(slot), json);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving game to slot {slot}: {e.Message}");
            }
        }

        public SaveData LoadGame(int slot = 0)
        {
            try
            {
                string path = GetSavePath(slot);
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<SaveData>(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading game from slot {slot}: {e.Message}");
            }

            return new SaveData();
        }

        public bool DoesSaveExist(int slot = 0) => File.Exists(GetSavePath(slot));

        public void DeleteSave(int slot = 0)
        {
            string path = GetSavePath(slot);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public SaveData[] GetAllSaves()
        {
            var saves = new List<SaveData>();
            var files = Directory.GetFiles(_saveDirectory, "save_*.json");

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var save = JsonConvert.DeserializeObject<SaveData>(json);
                    if (save != null)
                    {
                        saves.Add(save);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error loading save file {file}: {e.Message}");
                }
            }

            return saves.ToArray();
        }
    }
}
