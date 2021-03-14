using System.Data.SQLite;
using System.IO;
using UnityEngine;

namespace BetterEditor.Core.StringDB
{
    public static class SQLScript
    {
        private static string filePath { get; set; }
        private static SQLiteConnection SQLConn { get; set; }

        public static async void Setup(string entryPath)
        {
            filePath = Path.Combine(entryPath, "TranslationStrings.sqlite");

            if (!File.Exists(filePath)) {
                SQLiteConnection.CreateFile(filePath);
            }

            SQLConn = new SQLiteConnection($"Data Source={filePath};Version=3");
            await SQLConn.OpenAsync();

            CreateTable();
        }

        public static string GetReaderResult(string key, SystemLanguage language)
        {
            SQLiteDataReader reader = GetReader(key);
            return reader.Read() ? (string)(reader[language.ToString().ToLower().Substring(0, 3)] ?? reader["eng"]) : null;
        }

        private static SQLiteDataReader GetReader(string key)
        {
            key = EscapeString(key);

            return new SQLiteCommand(
                $"SELECT * FROM translation WHERE key = '{key}' AND key is not NULL", SQLConn).ExecuteReader();
        }

        public static int InsertKey(SQLValue value)
        {
            BetterEditor.Logger.Log($"$> INSERT INTO translation (key, eng, kor) values ({value})");

            return new SQLiteCommand(
                $"INSERT INTO translation (key, eng, kor) values ({value})", SQLConn).ExecuteNonQuery();
        }

        public static int DeleteKey(string key)
        {
            key = EscapeString(key);

            return new SQLiteCommand(
                $"DELETE FROM translation WHERE key = '{key}'", SQLConn).ExecuteNonQuery();
        }

        private static int CreateTable()
        {
            return new SQLiteCommand(
                $"CREATE TABLE IF NOT EXISTS translation (key varchar(128), eng varchar(512), kor varchar(512))", SQLConn).ExecuteNonQuery();
        }
        
        public static string EscapeString(string str) => str.Replace("'", "''");
    }
}