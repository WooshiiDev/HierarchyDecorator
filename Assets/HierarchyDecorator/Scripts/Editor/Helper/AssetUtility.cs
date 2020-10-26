using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
    {
    internal static class AssetUtility
        {
        internal static T LoadAssetFromGUID<T>(string GUID) where T : Object
            {
            string path = AssetDatabase.GUIDToAssetPath (GUID);

            if (string.IsNullOrEmpty(path))
                throw new NullReferenceException ($"Asset with GUID {GUID} does not exist!");

            return AssetDatabase.LoadAssetAtPath<T> (path);
            }

        internal static T CreateScriptableAtPath<T>(string name, string path) where T : ScriptableObject
            {
            if (string.IsNullOrEmpty(name))
                throw new StringNullOrEmptyException ($"Cannot create Scriptable with null name!");

            if (string.IsNullOrEmpty (path))
                path = Application.dataPath;

            T scriptable = ScriptableObject.CreateInstance<T> ();

            if (!Directory.Exists (path))
                Directory.CreateDirectory (path);

            string fullPath = path + name + ".asset";
            AssetDatabase.CreateAsset (scriptable, fullPath);
            AssetDatabase.SaveAssets ();

            return scriptable;
            }
        }
    }
