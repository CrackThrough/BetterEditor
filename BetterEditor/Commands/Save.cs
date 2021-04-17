using ADOFAI;
using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using UnityEngine;

namespace BetterEditor.Commands
{
    [CommandInfo(Id = "save")]
    class Save : BECommand
    {
        Save() : base() { }

        public override void Execute(scnEditor instance, string[] args)
        {
            if (args.Length == 0) return;

            switch (args[0].ToLower())
            {
                case "as":
                    if (!string.IsNullOrEmpty(args[1]))
                    {
                        RDFile.WriteAllText(args[1], scnEditorPrivates.GetField<LevelData>("levelData").Encode());
                    }
                    break;
                default:
                    scnEditorPrivates.InvokeMethod("SaveLevel");
                    return;
            }
        }
    }
}
