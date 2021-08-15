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

            if (string.IsNullOrEmpty (path))
            {
                throw new NullReferenceException ($"Asset with GUID {GUID} does not exist!");
            }

            return AssetDatabase.LoadAssetAtPath<T> (path);
        }

        internal static T CreateScriptableAtPath<T>(string name, string path) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty (name))
            {
                throw new ArgumentNullException ("Cannot create scriptable with null name.");
            }

            if (string.IsNullOrEmpty (path))
            {
                path = Application.dataPath;
            }

            T scriptable = ScriptableObject.CreateInstance<T> ();

            if (!Directory.Exists (path))
            {
                Directory.CreateDirectory (path);
            }

            string fullPath = path + name + ".asset";

            AssetDatabase.CreateAsset (scriptable, fullPath);
            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();

            return scriptable;
        }

        internal static T FindOrCreateScriptable<T>(string type, string createPath, Action<T> onCreate = null) where T : ScriptableObject
        {
            T scriptable = null;
            string[] guids = AssetDatabase.FindAssets ($"t:{type}");

            if (guids.Length == 0)
            {
                scriptable = CreateScriptableAtPath<T> (type, createPath);
                onCreate?.Invoke (scriptable);

                Debug.Log (string.Format("Created scriptable {0} at path {1}", type, createPath));
            }
            else
            {
                string scriptablePath = AssetDatabase.GUIDToAssetPath (guids[0]);
                scriptable = AssetDatabase.LoadAssetAtPath<T> (scriptablePath);
            }

            return scriptable;
        }
    }
}