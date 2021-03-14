using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace BetterEditor.Core
{
    public class BEKeybind
    {
        public BEKeybind(List<KeyCode> keyCodes, string boundCommand)
        {
            KeyCodes = keyCodes;
            BoundCommand = boundCommand;
        }

        public List<KeyCode> KeyCodes { get; set; } = new List<KeyCode>();
        public string BoundCommand { get; set; } = "";

        public bool CheckPressed()
        {
            if (KeyCodes.Count > 0)
            {
                for (int i = 0; i < KeyCodes.Count; i++)
                {
                    if (!(i + 1 == KeyCodes.Count ?
                        Input.GetKeyDown(KeyCodes[i]) :
                        Input.GetKey(KeyCodes[i])))
                        return false;
                }
            }
            else return false;

            return true;
        }

        private static List<BEKeybind> keybinds { get; set; } = new List<BEKeybind>();

        public static void LoadKeybinds()
        {
            using (var sr = new StreamReader(BESettings.GetPath("BEMainKeybinds")))
            {
                var s = new XmlSerializer(typeof(List<BEKeybind>));
                keybinds = (List<BEKeybind>) (s.Deserialize(sr) ?? new List<BEKeybind>());
            }
        }

        public static void SaveKeybinds()
        {
            using (var sw = new StreamWriter(BESettings.GetPath("BEMainKeybinds")))
            {
                var s = new XmlSerializer(typeof(List<BEKeybind>));
                s.Serialize(sw, keybinds);
            }
        }

        public static void OnUpdate(scnEditor instance)
        {
            foreach (BEKeybind keybind in keybinds)
            {
                if (keybind.CheckPressed())
                {
                    BECommand.Execute(instance, keybind.BoundCommand);
                }
            }
        }
    }
}
