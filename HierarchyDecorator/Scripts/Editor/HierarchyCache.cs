using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HierarchyDecorator
{
    public static class HierarchyCache
    {
        public class HierarchyData : IComparable<HierarchyData>, IEquatable<HierarchyData>
        {
            // Instance 

            public readonly int ID;
            public readonly Transform Transform;

            // - Properties

            public bool Foldout { get; set; }

            public bool HasChildren => Transform != null && Transform.childCount > 0;

            public Transform Parent
            {
                get
                {
                    if (Transform == null)
                    {
                        return null;
                    }
                    
                    return Transform.parent;
                }
            }

            public bool IsRoot => Transform != null && Parent == null;

            // - Methods

            public HierarchyData(Transform transform)
            {
                ID = transform.GetInstanceID();
                Transform = transform;
            }

            public int GetSiblingIndex(SceneCache scene)
            {
                if (IsRoot)
                {
                    return scene.Roots.IndexOf(ID);
                }

                return Transform.GetSiblingIndex();
            }

            public bool IsLastSibling(SceneCache scene)
            {
                if (IsRoot)
                {
                    return scene.Roots.IndexOf(ID) == scene.Roots.Count - 1;
                }

                return Transform.GetSiblingIndex() == Parent.childCount - 1;
            }

            public int CalculateDepth()
            {
                Transform p = Parent;
                int index = 0;
                while (p != null)
                {
                    p = p.parent;
                    index++;
                }

                return index;
            }

            public bool IsValid()
            {
                return Transform != null;
            }

            public int CompareTo(HierarchyData other)
            {
                return ID.CompareTo(other.ID);
            }

            public bool Equals(HierarchyData other)
            {
                return ID == other.ID;
            }
        }

        public class SceneCache : IDisposable
        {
            // --- Fields

            private List<int> validIDs = new List<int>();
            private Dictionary<int, HierarchyData> lookup = new Dictionary<int, HierarchyData>();

            // --- Properties

            public readonly Scene Scene; 

            public HierarchyData First { get; private set; }
            public HierarchyData Current { get; private set; }
            public HierarchyData Previous { get; private set; }
            public List<int> Roots { get; private set; } = new List<int>();

            // --- Creation

            public SceneCache(Scene scene)
            {
                Scene = scene;
            }

            public void Dispose()
            {
                for (int i = 0; i < validIDs.Count; i++)
                {
                    if (!lookup.TryGetValue(validIDs[i], out HierarchyData data))
                    {
                        continue;
                    }

                    Remove(data);
                }
            }

            // --- Methods

            public bool Add(Transform instance)
            {
                if (instance == null)
                {
                    Debug.LogWarning("Cannot add a null instance.");
                    return false;
                }

                HierarchyData data = new HierarchyData(instance);
                int id = data.ID;

                if (validIDs.Contains(id))
                {
                    return false;
                }

                lookup.Add(id, data);
                
                if (instance.parent == null)
                {
                    Roots.Add(id);
                    Roots.OrderBy(x => x);
                }

                // Update

                SetTarget(data);

                return true;
            }

            public bool Remove(int id)
            {
                if (!lookup.TryGetValue(id, out HierarchyData data))
                {
                    Debug.LogError($"Missing HierarchyData with ID {id}.");
                    return false;
                }

                return Remove(data);
            }

            private bool Remove(HierarchyData data)
            {
                if (data == null)
                {
                    Debug.LogError("Attempt to delete null HiearchyData instance.");
                    return false;
                }

                int id = data.ID;

                //if (Roots.Contains(data.ID))
                //{
                //    Roots.Remove(data.ID);
                //}

                lookup.Remove(id);
                return true;
            }

            public bool TryGetInstance(int id, out HierarchyData instance)
            {
                return lookup.TryGetValue(id, out instance);
            }

            public void SetTarget(HierarchyData data)
            {
                if (data == null)
                {
                    Debug.LogWarning("Cannot assign a null instance as the target.");
                    return;
                }

                Previous = Current;
                Current = data;

                // Check foldout state for last instance

                if (Previous != null && Previous.IsValid() && Previous.HasChildren)
                {
                    Previous.Foldout = Current.Transform.parent == Previous.Transform;
                }

                // Refresh if at the start of the hierarchy

                if (First == data)
                {
                    Refresh();
                }

                if (data.IsRoot && data.GetSiblingIndex(this) == 0)
                {
                    First = data;
                }

                validIDs.Add(data.ID);
            }

            public void SetTarget(Transform transform)
            {
                if (transform == null)
                {
                    return;
                }

                int id = transform.GetInstanceID();
                if (TryGetInstance(id, out HierarchyData data))
                {
                    SetTarget(data);
                    return;
                }

                Add(transform);
            }

            public void Clear()
            {
                lookup.Clear();
                validIDs.Clear();
            }

            private void Refresh()
            {
                List<HierarchyData> invalid = new List<HierarchyData>();
                foreach (int key in lookup.Keys)
                {
                    HierarchyData data = lookup[key];

                    if (!validIDs.Contains(key) || data.Transform == null)
                    {
                        invalid.Add(lookup[key]);
                    }
                }

                for (int i = 0; i < invalid.Count; i++)
                {
                    Remove(invalid[i]);
                }

                validIDs.Clear();
            }
        }

        private static Dictionary<Scene, SceneCache> Scenes = new Dictionary<Scene, SceneCache>();

        public static SceneCache Target;

        public static bool RegisterScene(Scene scene)
        {
            if (!scene.IsValid())
            {
                return false;
            }

            if (Exists(scene))
            {
                Debug.LogWarning("Attempted to add preexisting scene to cache.");
                return false;
            }

            SceneCache cache = new SceneCache(scene);

            Scenes.Add(scene, cache);

            if (Target == null)
            {
                Target = cache;
            }

            return true;
        }

        public static bool RemoveScene(Scene scene)
        {
            if (!TryGetScene(scene, out SceneCache cache))
            {
                return false;
            }

            cache.Dispose();
            return Scenes.Remove(scene);
        }

        public static bool TryGetScene(Scene scene, out SceneCache cache)
        {
            return Scenes.TryGetValue(scene, out cache);
        }

        public static bool Exists(Scene scene)
        {
            return Scenes.ContainsKey(scene);
        }

        public static bool IsTarget(Scene scene)
        {
            return Target.Scene.handle == scene.handle;
        }

        public static SceneCache SetTarget(Scene scene)
        {
            if (IsTarget(scene))
            {
                return Target;
            }

            if (!TryGetScene(scene, out SceneCache cache))
            {
                RegisterScene(scene);
                return Target;
            }

            SetTarget(cache);
            return Target;
        }

        public static void SetTarget(SceneCache cache)
        {
            if (cache == null)
            {
                Debug.LogError("Null cache, cannot set target.");
                return;
            }

            Target = cache;
        }
    }
}