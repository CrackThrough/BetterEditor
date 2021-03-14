using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.Core.Attributes
{
    public class CommandInfo : Attribute
    {
        public CommandInfo() : base()
        {

        }

        public string Id { get; set; }
    }
}
