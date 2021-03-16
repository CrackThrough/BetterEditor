using BetterEditor.Core.Attributes;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace BetterEditor.Core
{
    public class BECommand
    {
        public BECommand()
        {

        }

        public static ModLogger Logger { get; set; }

        public virtual void Execute(scnEditor instance, string[] args) {
            throw new NotImplementedException("You should override [public void BECommand.Execute()] in order to make the command work.");
        }

        public static bool LimitTypes(Dictionary<Type, bool> typeStatus, string value, Type valueType = null)
        {
            if (valueType == null) valueType = value.GetType();

            if (typeStatus.Keys.Contains(valueType))
            {
                return typeStatus[valueType];
            }
            else return false;
        }

        private static readonly string Quotes = "'\"";
        private static readonly string Numbers = "1234567890";

        public static object castStringToType(string str)
        {
            /**
             * string
             * float
             * double
             * bool
             */
            var s = str.Trim();

            if (s.Length == 0) return null;

            foreach (char c in Quotes)
            {
                if (s.StartsWith(c.ToString()) && s.EndsWith(c.ToString()))
                {
                    return s.Remove(s.Length - 1, 1).Remove(0, 1);
                }
            }

            switch (s.Last())
            {
                case 'f':
                    if (float.TryParse(s, out float rf)) return rf;
                    break;
                case 'd':
                    if (double.TryParse(s, out double rd)) return rd;
                    break;
            }

            if (Numbers.Contains(s.Last()))
            {
                if (s.Contains(".") && float.TryParse(s, out float rf)) return rf;
                else if (int.TryParse(s, out int ri)) return ri;
            }

            if ((new string[] { "true", "false" }).Contains(str.ToLower())) return str.Equals("true");

            return str;
        }

        public static bool isType<T>(object obj, out T castedObject)
        {
            if (obj.GetType().Equals(typeof(T)))
            {
                castedObject = (T)obj;
                return true;
            }
            else if (obj.GetType().IsSubclassOf(typeof(T)))
            {
                castedObject = (T)obj;
                return true;
            }
            else
            {
                castedObject = default;
                return false;
            }
        }

        private static readonly IDictionary<string, BECommand> StoredCommands = new Dictionary<string, BECommand>();

        public static void Setup()
        {
            Logger = BetterEditor.Logger;

            StoredCommands.Clear();
            IEnumerable<Type> CommandTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<CommandInfo>() != null);

            foreach (Type CommandType in CommandTypes)
            {
                string CommandId = CommandType.GetCustomAttribute<CommandInfo>().Id;

                StoredCommands.Add(CommandId, (BECommand) CommandType.GetConstructors().First().Invoke(null, null));
            }
        }

        public static void Execute(scnEditor instance, string input)
        {
            string cmd = input.Substring(0, Math.Max(input.IndexOf(' '), input.Length)).ToLower();
            string[] args = input.Substring(Math.Max(input.IndexOf(' '), Math.Max(input.Length - 1, 0))).Split(' ');

            if (StoredCommands.ContainsKey(cmd))
            {
                StoredCommands[cmd].Execute(instance, args);
            }
            else
            {
                BetterEditor.Logger.Log(
                    BEString.GetAndParse("command_global.unknown_command",
                    new Dictionary<string, object> {
                        { "command_name", cmd }
                    }
                ));
            }
        }
    }
}
