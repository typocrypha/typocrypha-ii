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
            return new List<Type>(Assembly.GetAssembly(typeof(T)).GetExportedTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T))));
        }
        /// <summary> Return every type that is a sub-class of t. t should be a class</summary>
        public static IEnumerable<Type> GetAllSubclassTypes(Type t)
        {
            return new List<Type>(Assembly.GetAssembly(t).GetExportedTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(t)));
        }
    }
}
