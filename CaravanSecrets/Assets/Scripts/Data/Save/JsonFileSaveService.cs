using System;
using System.IO;
using UnityEngine;

namespace CaravanSecrets.Data.Save
{
    public interface ISaveService
    {
        PlayerSaveData Load();
        void Save(PlayerSaveData data);
        void DeleteSave();
    }

    public sealed class JsonFileSaveService : ISaveService
    {
        private string SavePath => Path.Combine(Application.persistentDataPath, "player-save.json");
        private string BackupPath => SavePath + ".backup";

        public PlayerSaveData Load()
        {
            if (TryLoad(SavePath, out var data) || TryLoad(BackupPath, out data)) return data;
            return new PlayerSaveData();
        }

        public void Save(PlayerSaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Directory.CreateDirectory(Application.persistentDataPath);
            var temp = SavePath + ".tmp";
            File.WriteAllText(temp, JsonUtility.ToJson(data, true));
            if (File.Exists(SavePath)) File.Copy(SavePath, BackupPath, true);
            File.Copy(temp, SavePath, true);
            File.Delete(temp);
        }

        public void DeleteSave()
        {
            if (File.Exists(SavePath)) File.Delete(SavePath);
            if (File.Exists(BackupPath)) File.Delete(BackupPath);
        }

        private static bool TryLoad(string path, out PlayerSaveData data)
        {
            data = null;
            if (!File.Exists(path)) return false;
            try { data = JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(path)); return data != null && data.SaveVersion > 0; }
            catch (Exception exception) { Debug.LogWarning($"Could not load save at {path}: {exception.Message}"); return false; }
        }
    }
}
