#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grabbit
{
    [ExecuteInEditMode]
    public class GrabbitCollidersWatcher : MonoBehaviour
    {
        public List<Collider> ExistingCollider = new List<Collider>();
        public List<MeshCollider> AddedStaticColliders = new List<MeshCollider>();
        public List<MeshCollider> AddedDynamicColliders = new List<MeshCollider>();

        public bool HasExistingColliders => ExistingCollider.Count > 0;
        public bool HasAddedStaticColliders => AddedStaticColliders.Count > 0;
        public bool HasAddedDynamicColliders => AddedDynamicColliders.Count > 0;

        public void AddPreExistingColliders(GrabbitHandler handler)
        {
            foreach (var col in ExistingCollider)
            {
                if (!handler.PreExistingColliders.Contains(col))
                {
                    handler.PreExistingColliders.Add(col);
                }
            }
        }

        public void Awake()
        {
            // hideFlags = HideFlags.HideInInspector;
        }

        public void OnDestroy()
        {
            foreach (var col in ExistingCollider)
            {
                if (col)
                    col.enabled = true;
            }

            for (var i = AddedStaticColliders.Count - 1; i >= 0; i--)
            {
                var addedStaticCollider = AddedStaticColliders[i];
                if (addedStaticCollider)
                    DestroyImmediate(addedStaticCollider);
            }

            for (var i = AddedDynamicColliders.Count - 1; i >= 0; i--)
            {
                var addedDynamicCollider = AddedDynamicColliders[i];
                if (addedDynamicCollider)
                    DestroyImmediate(addedDynamicCollider);
            }

            ExistingCollider.Clear();
            AddedDynamicColliders.Clear();
            AddedStaticColliders.Clear();
        }
    }
}
#endif