using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace ReflectionUtils
{
    public static class ReflectiveEnumerator
    {
        /// <summary> Return every type that is a sub-class of T</summary>
        public static IEnumerable<Type> GetAllSubclassTypes<T>() where T : class
        {
            List<Type> types = new List<Type>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetExportedTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                types.Add(type);
            }
            return types;
        }
        /// <summary> Return every type that is a sub-class of t. t should be a class</summary>
        public static IEnumerable<Type> GetAllSubclassTypes(Type t)
        {
            List<Type> types = new List<Type>();
            foreach (Type type in
                Assembly.GetAssembly(t).GetExportedTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(t)))
            {
                types.Add(type);
            }
            return types;
        }
    }
}
