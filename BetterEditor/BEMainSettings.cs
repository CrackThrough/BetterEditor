using BetterEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterEditor
{
    public class BEMainSettings : BESettings
    {
        public BEMainSettings() : base()
        {

        }

        public bool IsEnabled { get; set; }
        public int LastExecutedModVersion { get; set; } = -1;
        public SystemLanguage Language { get; set; } = SystemLanguage.English;
        public bool DeveloperMode { get; set; } = true;
        public string TestValue { get; set; }
    }
}
