using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.Core.StringDB
{
    public class SQLValue
    {
        public SQLValue(string key, string t_eng, string t_kor)
        {
            Key = key;
            T_English = t_eng;
            T_Korean = t_kor;
        }

        public string Key { get; private set; }
        public string T_English { get; private set; }
        public string T_Korean { get; private set; }

        public override string ToString() => $"'{SQLScript.EscapeString(Key)}', '{SQLScript.EscapeString(T_English)}', '{SQLScript.EscapeString(T_Korean)}'";
    }
}
