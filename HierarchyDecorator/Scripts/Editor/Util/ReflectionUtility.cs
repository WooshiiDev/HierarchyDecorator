using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HierarchyDecorator
{
    internal static class ReflectionUtility
    {
        public static Type[] GetTypesFromAssembly(Type type)
        {
            Assembly assem = Assembly.GetAssembly (type);

            if (assem == null)
            {
                Debug.LogError ("Cannot find assembly of " + type + "!");
            }

            return assem.GetTypes ().Where (t => t.IsSubclassOf (type)).ToArray ();
        }

        public static Type[] GetTypesFromAllAssemblies(Type type)
        {
            List<Type> types = new List<Type> ();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();

            if (assemblies == null)
            {
                Debug.LogError ("Cannot find assembly of " + type + "!");
            }

            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];

                foreach (Type t in assembly.GetTypes ())
                {
                    if (t.IsSubclassOf (type))
                    {
                        types.Add (t);
                    }
                }
            }

            return types.ToArray ();
        }
    }
}