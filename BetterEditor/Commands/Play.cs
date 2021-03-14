using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.Commands
{
    [CommandInfo(Id = "play")]
    public class Play : BECommand
    {
        public Play() : base()
        {

        }

        public override void Execute(scnEditor instance, string[] args)
        {
            instance.Play();
        }
    }
}
