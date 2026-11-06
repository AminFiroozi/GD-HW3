using System.IO;
using UnityEngine;

namespace ChickenShooter.Core
{
    public static class SaveSystem
    {
        const string FileName = "savegame.json";

        static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        public static bool HasSave() => File.Exists(FilePath);

        public static void Save(SaveData data)
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
        }

        public static SaveData Load()
        {
            if (!HasSave()) return null;
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
        }

        public static void Delete()
        {
            if (HasSave()) File.Delete(FilePath);
        }
    }
}
