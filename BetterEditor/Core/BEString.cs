using UnityEngine;
using BetterEditor.Core.StringDB;
using static UnityModManagerNet.UnityModManager;
using System.Collections.Generic;
using System;
using BetterEditor.Core.Exceptions;

namespace BetterEditor.Core
{
    public static class BEString
    {
        private static SystemLanguage ToSupportedLanguage(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.English:
                case SystemLanguage.Korean:
                    return language;
                default:
                    return SystemLanguage.English;
            }
        }

        public static void Setup(ModEntry entry)
        {
            SQLScript.Setup(entry.Path); //ok
        }

        public static string Get(string key)
        {
            return Get(key, BetterEditor.Settings.Language);
        }

        public static string Get(string key, SystemLanguage language)
        {
            return SQLScript.GetReaderResult(key, ToSupportedLanguage(language)) ?? $"no such key '{key}'";
        }

        public static string Parse(string translatedString, Dictionary<string, object> paramDictionary, string fromKey = "unknown key provided")
        {
            int closeIndex, latestIndex;
            string key;

            while((latestIndex = translatedString.IndexOf("$(")) >= 0 && // $( 가 스트링 내에 있고
                (latestIndex == 0 ? 'H' : translatedString[Math.Max(latestIndex - 1, 0)]) == '\\' && // latestIndex 0을 제외하고 translatedString에서의 $( 직전 글자가 \ 이고
                (closeIndex = translatedString.IndexOf(")", latestIndex)) < 0) // 닫는 괄호가 그 다음에 존재한다면
            {
                // $(' ')
                if (paramDictionary.ContainsKey(
                    key = translatedString.Substring(
                        // 인덱스 이후 남는 스트링의 총 길이가 0 이상일 때 $( 직후 글씨부터 )의 인덱스까지 빼서 길이로 구하고 안의 키 얻기
                        translatedString.Length - latestIndex - 1 >= 0 ? latestIndex + 2 : latestIndex, closeIndex
                        ).Trim()))
                {
                    translatedString = translatedString.Replace(key, paramDictionary[key].ToString());
                }
                else throw new BECommandRequiredKeyNotFoundException($"Required key not found in paramDictionary: '{key}'.\n\t└ While parsing text: {translatedString}\n\t└ While parsing key: '{fromKey}'.");
            }

            return translatedString;
        }

        public static string GetAndParse(string key, Dictionary<string, object> paramDictionary)
        {
            return Parse(Get(key), paramDictionary, key);
        }

        public static string GetAndParse(string key, SystemLanguage language, Dictionary<string, object> paramDictionary)
        {
            return Parse(Get(key, language), paramDictionary, key);
        }
    }
}
