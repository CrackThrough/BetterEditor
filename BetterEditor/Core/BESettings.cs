using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityModManagerNet.UnityModManager;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace BetterEditor.Core
{
    public abstract class BESettings : ModSettings
    {
        public BESettings()
        {

        }

        public static string GetPath(string typeName)
        {
            return Path.Combine(BetterEditor.SettingsDirectory, $"{typeName}.xml");
        }

        public string GetPath()
        {
            return GetPath(GetType().Name);
        }

        private static IDictionary<Type, BESettings> SettingsStorage = new Dictionary<Type, BESettings>();

        public static void Setup()
        {
            SettingsStorage.Clear();
            IEnumerable<Type> stgSubclasses = Assembly.GetExecutingAssembly().GetTypes().Where(
                t => t.IsSubclassOf(typeof(BESettings)));

            StringBuilder nullSb = new StringBuilder("Those types returned null while loading types: ");

            foreach (Type stgType in stgSubclasses)
            {
                BESettings stgInstance = Load(stgType);
                if (stgInstance == null)
                {
                    nullSb.Append($"{stgType.Name} ");
                    stgInstance = (BESettings) stgType.GetConstructors().First().Invoke(null, null);
                }

                SettingsStorage.Add(stgType, stgInstance);
            }

            BetterEditor.Logger.Log(nullSb.ToString());
        }

        public static BESettings Load(Type stgType)
        {
            string filePath = GetPath(stgType.Name);

            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    var s = new XmlSerializer(stgType);
                    return (BESettings) s.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                BetterEditor.Logger.Log($"Settings loading failed for the type '{stgType.Name}'.");
                BetterEditor.Logger.LogException(e);
            }

            return null;
        }

        public static T Get<T>() where T : BESettings
        {
            return (T) SettingsStorage.FirstOrDefault(s => s.Key == typeof(T)).Value;
        }

        public static void Save(BESettings stgObject, Type stgType)
        {
            string filePath = GetPath(stgType.Name);

            try
            {
                using (var sw = new StreamWriter(filePath))
                {
                    var s = new XmlSerializer(stgType);
                    s.Serialize(sw, stgObject);
                }
            }
            catch (Exception e)
            {
                BetterEditor.Logger.Log($"Settings saving failed for the type '{stgType.Name}'.");
                BetterEditor.Logger.LogException(e);
            }
        }

        public static void SaveAll()
        {
            foreach (KeyValuePair<Type, BESettings> stgData in SettingsStorage)
            {
                Save(stgData.Value, stgData.Key);
            }
        }
    }
}
