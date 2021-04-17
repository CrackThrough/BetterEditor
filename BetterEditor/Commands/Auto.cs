using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using UnityEngine;

namespace BetterEditor.Commands
{
    [CommandInfo(Id = "auto")] 
    class Auto : BECommand
    {
        Auto() : base() { }

        public override void Execute(scnEditor instance, string[] args)
        {
            if (args.Length == 0) RDC.auto = !RDC.auto;

            switch (args[0].ToLower())
            {
                case "on":
                    RDC.auto = true;
                    break;
                case "off":
                    RDC.auto = false;
                    break;
                case "toggle":
                    RDC.auto = !RDC.auto;
                    break;
                case "get":
                    break;
                default:
                    return;
            }

            scnEditorPrivates.SetField("autoFailed", false);

            AudioSource ottoSrc = scnEditorPrivates.GetField<AudioSource>("ottoSrc");
            bool highBPM = scnEditorPrivates.GetField<bool>("highBPM");
            AudioClip[] ottoClips = scnEditorPrivates.GetField<AudioClip[]>("ottoClips");

            if (RDC.auto)
            {
                if (highBPM)
                {
                    ottoSrc.clip = ottoClips[1];
                }
                else
                {
                    ottoSrc.clip = ottoClips[0];
                }
            }
            else
            {
                ottoSrc.clip = ottoClips[3];
            }

            if (args[0].ToLower() != "get")
            {
                ottoSrc.Play();
            }
        }
    }
}
