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
            var assem = Assembly.GetAssembly (type);

            if (assem == null)
                Debug.Log ("Cannot find assembly of " + type + "!");

            return assem.GetTypes ().Where (t => t.IsSubclassOf (type)).ToArray ();
            }

        public static Type[] GetTypesFromAllAssemblies(Type type)
            {
            List<Type> types = new List<Type> ();
            var assem = AppDomain.CurrentDomain.GetAssemblies ();

            if (assem == null)
                Debug.Log ("Cannot find assembly of " + type + "!");

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies ())
                {
                foreach (Type t in a.GetTypes ())
                    {
                    if (t.IsSubclassOf (type))
                        types.Add (t);
                    }
                }

            return types.ToArray ();
            }
        }
    }
