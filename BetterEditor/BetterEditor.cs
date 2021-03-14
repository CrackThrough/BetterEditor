using static UnityModManagerNet.UnityModManager;
using BetterEditor.Core;
using UnityEngine;
using BetterEditor.Core.StringDB;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Collections;
using System;
using System.IO;

namespace BetterEditor
{
    public static class BetterEditor
    {
        public static ModEntry.ModLogger Logger { get; set; }
        public static bool isEnabled { get; set; }
        public static bool hasUpdate { get; set; }

        public static string SettingsDirectory { get; private set; }
        public static BEMainSettings Settings { get; set; }

        private static Harmony harmony { get; set; }

        public static void Init(ModEntry entry)
        {
            Logger = entry.Logger;

            scnEditorPrivates.Testing();
            return;

            SettingsDirectory = Path.Combine(entry.Path, @"ModSettings\");

            if (!Directory.Exists(SettingsDirectory))
            {
                Directory.CreateDirectory(SettingsDirectory);
            }

            BESettings.Setup();
            Settings = BESettings.Get<BEMainSettings>();

            BEString.Setup(entry);
            
            entry.Info.DisplayName = BEString.Get("global.mod_title");

            entry.OnToggle = OnToggle;
            entry.OnShowGUI = OnShowGUI;
            entry.OnSaveGUI = OnSaveGUI;
            entry.OnGUI = OnGUI;

            isEnabled = entry.Enabled;
            hasUpdate = entry.HasUpdate;

            // scnEditorPrivates.Setup();
            harmony = new Harmony(entry.Info.Id);
        }

        public static bool OnToggle(ModEntry entry, bool enabled)
        {
            hasUpdate = entry.HasUpdate;
            isEnabled = entry.Enabled;

            if (enabled)
            {
                harmony.PatchAll();
            }
            else
            {
                harmony.UnpatchAll();
            }

            return true;
        }

        public static void OnShowGUI(ModEntry entry)
        {
            hasUpdate = entry.HasUpdate;
        }

        public static void OnSaveGUI(ModEntry entry)
        {
            hasUpdate = entry.HasUpdate;
            BESettings.SaveAll();
        }

        private static string[] values = new string[] { "", "", "", "", "" };

        public static void OnGUI(ModEntry entry)
        {
            if (Settings.DeveloperMode)
            {
                GUILayout.Label("### Create Key (key, eng, kor)");

                GUILayout.BeginHorizontal();
                for (int i = 0; i < 3; i++)
                {
                    values[i] = GUILayout.TextField(values[i]);
                }

                if (GUILayout.Button("Apply"))
                {
                    SQLValue value = new SQLValue(values[0], values[1], values[2]);
                    SQLScript.InsertKey(value);
                }
                GUILayout.EndHorizontal();

                GUILayout.Label("### Remove Key (key)");

                GUILayout.BeginHorizontal();
                values[3] = GUILayout.TextField(values[3]);

                if (GUILayout.Button("Apply"))
                {
                    SQLScript.DeleteKey(values[3]);
                }
                GUILayout.EndHorizontal();

                GUILayout.Label("### Get Key Result: ");
                GUILayout.Label(BEString.Get(values[4], SystemLanguage.English));
                GUILayout.Label(BEString.Get(values[4], SystemLanguage.Korean));

                values[4] = GUILayout.TextField(values[4]);

                GUILayout.Space(20f);

                Settings.TestValue = GUILayout.TextField(Settings.TestValue);

                GUILayout.Space(20f);
            }
        }
    }
}
