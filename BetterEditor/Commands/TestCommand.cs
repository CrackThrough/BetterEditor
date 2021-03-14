using BetterEditor.Core;
using BetterEditor.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.Commands
{
    [CommandInfo(Id = "test_command")]
    public class TestCommand : BECommand
    {
        public TestCommand() : base()
        {

        }

        public override void Execute(scnEditor instance, string[] args) //자동완성이 안대요 살려주세요 저런
        {
            object myObj = castStringToType(args[0]);

            if (isType(myObj, out bool sdsaf))
            {

            }

            string cc = BEString.Get("command.test_command.name");

            Dictionary<string, object> paramDictionary = new Dictionary<string, object>();

            paramDictionary.Add("parameter_name", 1213);
            paramDictionary.Add("parameter_name2", 456f);
            paramDictionary.Add("parameter_name3", false);
            paramDictionary.Add("parameter_name4", null);

            cc = BEString.GetAndParse("command.test_command.description", paramDictionary);

        }
    }
}
