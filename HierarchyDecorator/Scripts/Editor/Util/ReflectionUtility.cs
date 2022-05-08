using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HierarchyDecorator
{
    internal static class ReflectionUtility
    {
        // --- Subtypes

        /// <summary>
        /// Get all subtypes for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the subtypes found.</returns>
        public static Type[] GetSubTypes(Type type)
        {
            // Cannot get subtypes if the given type is null

            if (type == null)
            {
                Debug.Log ("Cannot get subtypes for null type.");
                return null;
            }

            // Return the subclasses of the type 

            return GetTypesFromAssembly (GetAssemblyFromType (type)).Where (t => t.IsSubclassOf (type)).ToArray ();
        }

        /// <summary>
        /// Get all subtypes for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="condition">The condition to check subtypes against.</param>
        /// <returns>Returns the subtypes found.</returns>
        public static Type[] GetSubTypes(Type type, Predicate<Type> condition)
        {
            // Cannot get subtypes if the given type is null

            if (type == null)
            {
                Debug.Log ("Cannot get subtypes for null type.");
                return null;
            }

            return GetSubTypes (type).Where (t => condition(t)).ToArray ();
        }

        /// <summary>
        /// Get all types from all assemblies.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the subtypes found.</returns>
        public static Type[] GetSubTypesFromAssemblies(Type type)
        {
            // Cannot get subtypes if the given type is null

            if (type == null)
            {
                Debug.Log ("Cannot get subtypes for null type.");
                return null;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();

            // Loop through all assemblies found and add the found subclasses to the list

            List<Type> types = new List<Type> ();
            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange (GetTypesFromAssembly (assemblies[i], t => t.IsSubclassOf (type)));
            }

            return types.ToArray ();
        }

        /// <summary>
        /// Get all types from all assemblies.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the subtypes found.</returns>
        public static Type[] GetSubTypesFromAssemblies(Type type, Predicate<Type> condition)
        {
            return GetSubTypesFromAssemblies (type).Where(t => condition(t)).ToArray ();
        }

        // --- All Types

        /// <summary>
        /// Get all types from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns the types found. If the assembly given is null, so will the array returned.</returns>
        public static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                Debug.LogError ("Cannot find types from a null assembly.");
                return null;
            }

            return assembly.GetTypes ();
        }

        /// <summary>
        /// Get all types from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns the types found. If the assembly given is null, so will the array returned.</returns>
        public static Type[] GetTypesFromAssembly(Assembly assembly, Predicate<Type> condition)
        {
            return GetTypesFromAssembly (assembly).Where (t => condition(t)).ToArray();
        }

        /// <summary>
        /// Get all types from all assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns the types found.</returns>
        public static Type[] GetTypesFromAssemblies()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();

            // Loop through all assemblies found and add the found subclasses to the list

            List<Type> types = new List<Type> ();
            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange (GetTypesFromAssembly (assemblies[i]));
            }

            return types.ToArray();
        }

        /// <summary>
        /// Get all types from all assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Returns the types found.</returns>
        public static Type[] GetTypesFromAssemblies(Predicate<Type> condition)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();

            // Loop through all assemblies found and add the found subclasses to the list

            List<Type> types = new List<Type> ();
            for (int i = 0; i < assemblies.Length; i++)
            {
                types.AddRange (GetTypesFromAssembly (assemblies[i]).Where(t => condition(t)));
            }

            return types.ToArray ();
        }

        // --- Assembly

        /// <summary>
        /// Get the assembly the given type is in.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Returns the assembly for the type. Will return null if the type is null.</returns>
        public static Assembly GetAssemblyFromType(Type type)
        {
            // Cannot get subtypes if the given type is null

            if (type == null)
            {
                Debug.Log ("Cannot get subtypes for null type.");
                return null;
            }

            return Assembly.GetAssembly (type);
        }
    }
}