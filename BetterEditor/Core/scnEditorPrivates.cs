using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetterEditor.Core
{
    public static class scnEditorPrivates
    {
        private static Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        private static Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
        private static Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();

        public static void Setup()
        {
            Type s = typeof(scnEditor);

            methods.Clear();
            properties.Clear();
            fields.Clear();

            int overloadCount = 0;

            foreach (MethodInfo i in s.GetMethods(AccessTools.all))
            {
                if (methods.ContainsKey(i.Name))
                {
                    BetterEditor.Logger.Log($"Method overload warning: '{i.Name}_overload${overloadCount}'");
                    methods.Add($"{i.Name}_overload${overloadCount}", i);
                    overloadCount++;
                }
                else methods.Add(i.Name, i);
            }

            foreach (PropertyInfo i in s.GetProperties(AccessTools.all))
            {
                if (properties.ContainsKey(i.Name))
                {
                    properties.Add($"{i.Name}_overload${overloadCount}", i);
                }
                else properties.Add(i.Name, i);
            }

            foreach (FieldInfo i in s.GetFields(AccessTools.all))
            {
                if (fields.ContainsKey(i.Name))
                {
                    fields.Add($"{i.Name}_overload${overloadCount}", i);
                }
                else fields.Add(i.Name, i);
            }

            BetterEditor.Logger.Log($"m: {methods.Count} | p: {properties.Count} | f: {fields.Count}");

            instance = scnEditor.instance;
        }

        public static scnEditor instance { get; set; } = scnEditor.instance;

        public static object InvokeMethod(string methodName, object[] parameter = null)
        {
            if (methods.ContainsKey(methodName))
            {
                return methods[methodName].Invoke(instance, parameter);
            }

            return null;
        }

        public static object GetProperty(string propertyName, object[] parameter = null)
        {
            if (properties.ContainsKey(propertyName))
            {
                return properties[propertyName].GetGetMethod(true).Invoke(instance, parameter);
            }

            return null;
        }

        public static void SetProperty(string propertyName, object[] parameter)
        {
            if (properties.ContainsKey(propertyName))
            {
                properties[propertyName].GetSetMethod(true).Invoke(instance, parameter);
            }
        }

        public static object GetField(string fieldName)
        {
            if (fields.ContainsKey(fieldName))
            {
                return fields[fieldName].GetValue(instance);
            }

            return null;
        }

        public static void SetField(string fieldName, object value)
        {
            if (fields.ContainsKey(fieldName))
            {
                fields[fieldName].SetValue(instance, value);
            }
        }
    }
}
