using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HierarchyDecorator
{
    internal static class AssetUtility
    {
        /// <summary>
        /// Load an asset with a given GUID
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="GUID">The GUID of the asset</param>
        /// <returns>Returns the loaded asset</returns>
        internal static T LoadAssetFromGUID<T>(string GUID) where T : Object
        {
            string path = AssetDatabase.GUIDToAssetPath (GUID);

            if (string.IsNullOrEmpty (path))
            {
                throw new NullReferenceException ($"Asset with GUID {GUID} does not exist!");
            }

            return AssetDatabase.LoadAssetAtPath<T> (path);
        }

        /// <summary>
        /// Create a scriptable at a path
        /// </summary>
        /// <typeparam name="T">The Scriptable type</typeparam>
        /// <param name="name">The name of the created scriptable</param>
        /// <param name="path">The path of the created scriptable</param>
        /// <returns>Returns the created Scriptable</returns>
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

        /// <summary>
        /// Find a scriptable type or create one at a given path and name
        /// </summary>
        /// <typeparam name="T">The Scriptable type</typeparam>
        /// <param name="type">The type string unity uses to search</param>
        /// <param name="path">The path to create the Scriptable at</param>
        /// <param name="name">The name of the created Scriptable</param>
        /// <param name="onCreate">Callback when creating the scriptable</param>
        /// <returns>Returns the created Scriptable</returns>
        internal static T FindOrCreateScriptable<T>(string type, string name, string path, Action<T> onCreate = null) where T : ScriptableObject
        {
            T scriptable = null;

            // Check the GUIDs and look for a scriptable object
            string[] guids = AssetDatabase.FindAssets ($"t:{type}");
            if (guids.Length > 0)
            {
                foreach (string guid in guids)
                {
                    string guidPath = AssetDatabase.GUIDToAssetPath (guid);
                    T asset = AssetDatabase.LoadAssetAtPath<T> (guidPath);

                    if (asset != null)
                    {
                        scriptable = asset;
                        break;
                    }
                }
            }

            // If no scriptable has been found, create one.
            if (scriptable == null)
            {
                scriptable = CreateScriptableAtPath<T> (name, path);
                onCreate?.Invoke (scriptable);

                Debug.Log (string.Format("Created scriptable {0} at path {1}", type, path));
            }

            return scriptable;
        }
    }
}