using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SZ.ModelingTool.Utilities
{
    public static class Reflection
    {
        public static Type FindType(string name) => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(_assembly => _assembly.GetTypes())
                .FirstOrDefault(_type => _type.FullName == name);

        public static List<Type> FindDerived<T>()=> FindDerived<T>(AppDomain.CurrentDomain.GetAssemblies());

        public static List<Type> FindDerived<T>(Assembly assembly) => assembly
                .GetTypes()
                .Where(_type => _type != typeof(T) && _type.IsSubclassOf(typeof(T)))
                .ToList();

        public static List<Type> FindDerived<T>(Assembly[] assemblies) => assemblies
                .SelectMany(_assembly => _assembly.GetTypes())
                .Where(_type => _type != typeof(T) && _type.IsSubclassOf(typeof(T)))
                .ToList();

        public static IEnumerable<KeyValuePair<Type, AttribType>> FindAllTypesWithAttribute<AttribType>(Assembly[] assemblies = null)
            where AttribType : Attribute
        {
            if (assemblies == null)
                assemblies = AppDomain.CurrentDomain.GetAssemblies();

            return assemblies
                .SelectMany(_assembly => _assembly.GetTypes())
                .Select(_type => new KeyValuePair<Type, AttribType>(_type, Attribute.GetCustomAttribute(_type, typeof(AttribType)) as AttribType))
                .Where(pair => pair.Value != null);
        }

        public static object CreatetObject<T>()
            where T : class
        {
            return CreateObject(typeof(T));
        }

        public static object CreateObject(Type type)
        {
            if (type.IsSubclassOf(typeof(ScriptableObject)))
                return ScriptableObject.CreateInstance(type);

            return Activator.CreateInstance(type);
        }
    }
}