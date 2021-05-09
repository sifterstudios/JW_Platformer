#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Grabbit
{
    [Serializable]
    public struct UndoState
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public UndoState(Rigidbody body)
        {
            var transform = body.transform;
            Position = transform.position;
            Rotation = transform.rotation;
        }

        public void RecordUndoBodyMove(Rigidbody body, bool force = false)
        {
            var transform = body.transform;

            if (Position == transform.position && Rotation == transform.rotation && !force)
                return;

            var current = new UndoState {Position = transform.position, Rotation = transform.rotation};
            transform.position = Position;
            transform.rotation = Rotation;

            Undo.RegisterCompleteObjectUndo(body.transform, body.name);

            transform.position = current.Position;
            transform.rotation = current.Rotation;
            Position = transform.position;
            Rotation = transform.rotation;
            Undo.FlushUndoRecordObjects();
        }
    }

    [Serializable]
    public struct RbSaveState
    {
        public bool UseGravity;
        public bool DetectCollision;
        public bool IsKinematic;
        public float MaxAngularVelocity;
        public float MaxDepenetrationVelocity;
        public float AngularDrag;
        public float Drag;
        public CollisionDetectionMode CollisionMode;
        public int Layer;
        public RigidbodyInterpolation Interpolation;
        public RigidbodyConstraints Constraints;

        public void RegisterRigidBody(Rigidbody body)
        {
            Interpolation = body.interpolation;
            UseGravity = body.useGravity;
            IsKinematic = body.isKinematic;
            MaxAngularVelocity = body.maxAngularVelocity;
            MaxDepenetrationVelocity = body.maxDepenetrationVelocity;
            AngularDrag = body.angularDrag;
            Drag = body.drag;
            CollisionMode = body.collisionDetectionMode;
            Layer = body.gameObject.layer;
            DetectCollision = body.detectCollisions;
            Constraints = body.constraints;
        }

        public void UnregisterRigidBody(Rigidbody body)
        {
            if (!body)
                return;

            body.useGravity = UseGravity;
            body.isKinematic = IsKinematic;
            body.collisionDetectionMode = CollisionMode;
            body.maxAngularVelocity = MaxAngularVelocity;
            body.maxDepenetrationVelocity = MaxDepenetrationVelocity;
            body.angularDrag = AngularDrag;
            body.drag = Drag;
            body.gameObject.layer = Layer;
            body.detectCollisions = DetectCollision;
            body.interpolation = Interpolation;
            body.constraints = Constraints;
        }
    }


    [ExecuteInEditMode]
    public class GrabbitHandler : MonoBehaviour
    {
        public static bool InstantDestroyFlag = false;
        private static GrabbitSettings lastSettings;

        [SerializeField] private bool SelectionConfigured;
        [SerializeField] private bool StaticConfigured;

        #region COLLISION PARAMS

        private readonly List<ContactPoint> contacts = new List<ContactPoint>(3);

        public bool IsCollidingWithStaticGeo;

        //public List<Collider> collidingStaticGeo = new List<Collider>();
        public HashSet<Collider> CollidingStaticGeo = new HashSet<Collider>();
        public Vector3 AverageCollisionNormal;

        #endregion

        #region SELECTION MODE

        public bool IsInSelectionMode;
        public Vector3 DistanceToCentroid;
        public Quaternion OriginalRotation;
        [SerializeField] private UndoState UndoState;

        #endregion

        #region PRE EXISTING DATA

        public bool isPartOfPrefab;
        public List<Collider> PreExistingColliders = new List<Collider>();
        [SerializeField] private bool WasRigidBodyAdded;
        [SerializeField] private RbSaveState RigidBodySave;
        [SerializeField] private List<PropertyModification> registeredModifications;

        public Bounds Bounds;
        public float Volume => Bounds.size.x * Bounds.size.y * Bounds.size.z;

        public float BoundMaxDimension => Mathf.Max(Bounds.size.x, Bounds.size.y, Bounds.size.z);

        #endregion

        #region Added Components

        public Rigidbody Body;

        [SerializeField] private List<FixedJoint> AddedJoints = new List<FixedJoint>();

        [SerializeField] private List<MeshCollider> AddedDynamicConvexColliders = new List<MeshCollider>();
        [SerializeField] private List<MeshCollider> AddedStaticConvexColliders = new List<MeshCollider>();
        public List<GrabbitCollidersWatcher> AddedCollidersList = new List<GrabbitCollidersWatcher>();

        public bool IsResponsibleForColliders => AddedCollidersList.Count > 0;

        #endregion

        private bool DestroyIfNotActive()
        {
            if (lastSettings && !lastSettings.IsGrabbitActive)
            {
                DestroyImmediate(this);
                return true;
            }

            return false;
        }

        public void Awake()
        {
            /*modifications=PrefabUtility.GetPropertyModifications(gameObject);
            foreach (var propertyModification in modifications)
            {
                propertyModification.
            }*/
        }

        public void OnEnable()
        {
            if (DestroyIfNotActive())
                return;

            if (!StaticConfigured)
                //then configure the whole thing
                ConfigureStaticMode();


            ClearCollisionStatus();
        }

        public void RecordUndo()
        {
            UndoState.RecordUndoBodyMove(Body);
        }

        private void ClearCollisionStatus()
        {
            CollidingStaticGeo.Clear();
            IsCollidingWithStaticGeo = false;
        }

        public void DisableSelectionModeColliders(GrabbitSettings settings)
        {
            if (settings.UsePredefinedColliders)
            {
                foreach (var preExistingCollider in PreExistingColliders)
                {
                    preExistingCollider.enabled = false;
                }

                foreach (var col in AddedStaticConvexColliders)
                    if (!settings.UseDynamicNonConvexColliders)
                    {
                        col.convex = false;
                    }
            }
            else
            {
                foreach (var col in AddedStaticConvexColliders)
                    if (!settings.UseDynamicNonConvexColliders)
                    {
                        col.convex = false;
                    }

                foreach (var col in AddedDynamicConvexColliders)
                {
                    col.enabled = false;
                }
            }
        }

        public void EnableSelectionModeColliders(GrabbitSettings settings)
        {
            if (settings.UsePredefinedColliders)
            {
                foreach (var preExistingCollider in PreExistingColliders)
                {
                    preExistingCollider.enabled = true;
                }

                foreach (var col in AddedStaticConvexColliders)
                    if (!settings.UseDynamicNonConvexColliders)
                    {
                        col.convex = true;
                    }
            }
            else
            {
                foreach (var col in AddedStaticConvexColliders)
                    if (!settings.UseDynamicNonConvexColliders)
                    {
                        col.convex = true;
                    }

                foreach (var col in AddedDynamicConvexColliders)
                {
                    col.enabled = true;
                }
            }
        }

        private void ConfigureStaticMode()
        {
            /* isPartOfPrefab = PrefabUtility.IsPartOfPrefabInstance(gameObject);
             if (isPartOfPrefab)
             {
              /*   var modifs =
                     PrefabUtility.GetPropertyModifications(PrefabUtility.GetNearestPrefabInstanceRoot(gameObject));
                 foreach (var modif in modifs)
                 {
                     modif.target
                 }
             }*/

            Body = GetComponent<Rigidbody>();

            if (Body)
            {
                //do not let the tool handle the body anymore (to ensure proper save)
                RigidBodySave = new RbSaveState();
                RigidBodySave.RegisterRigidBody(Body);
                //pre-check to avoid the error when registering new colliders
                Body.isKinematic = true;
                WasRigidBodyAdded = false;
            }

            var preExistingWatchers = new Dictionary<GameObject, GrabbitCollidersWatcher>();
            var addedWatchers = new Dictionary<GameObject, GrabbitCollidersWatcher>();

            RegisterExistingColliders(preExistingWatchers, addedWatchers);
            RegisterMeshes(preExistingWatchers, addedWatchers);

            IsCollidingWithStaticGeo = false;
            CollidingStaticGeo.Clear();
            StaticConfigured = true;

            ActivateBackgroundMode();
        }

        private void RegisterMeshes(Dictionary<GameObject, GrabbitCollidersWatcher> preExistingWatchers,
            Dictionary<GameObject, GrabbitCollidersWatcher> addedWatchers)
        {
            var bounds = new Bounds();

            var meshes = GetComponentsInChildren<MeshFilter>();
            int i = 0;
            foreach (var mesh in meshes)
            {
                if (!mesh.sharedMesh || mesh.sharedMesh.triangles.Length <= 1)
                    continue;

                GrabbitCollidersWatcher addedColliders = mesh.gameObject.GetComponent<GrabbitCollidersWatcher>();

                bool hasRegisteredPreExisting = preExistingWatchers.ContainsKey(mesh.gameObject);
                bool hasAdded = addedWatchers.ContainsKey(mesh.gameObject);
                bool justAdded = false;

                if (!hasRegisteredPreExisting && !hasAdded && !addedColliders)
                {
                    addedColliders = mesh.gameObject.AddComponent<GrabbitCollidersWatcher>();
                    AddedCollidersList.Add(addedColliders);
                    justAdded = true;
                }
                else
                {
                    if (hasAdded)
                        addedColliders = addedWatchers[mesh.gameObject];
                    else if (hasRegisteredPreExisting)
                    {
                        addedColliders = preExistingWatchers[mesh.gameObject];
                    }
                }

                if (hasRegisteredPreExisting)
                {
                    foreach (var meshCollider in addedColliders.AddedStaticColliders)
                    {
                        if (i == 0)
                            bounds = meshCollider.bounds;
                        else
                            bounds.Encapsulate(meshCollider.bounds);
                    }
                }
                else
                {
                    if (justAdded)
                    {
                        var col = mesh.gameObject.AddComponent<MeshCollider>();
                        col.sharedMesh = mesh.sharedMesh;

                        addedColliders.AddedStaticColliders.Add(col);
                        AddedStaticConvexColliders.Add(col);
                        if (i == 0)
                            bounds = col.bounds;
                        else
                            bounds.Encapsulate(col.bounds);
                    }
                    else
                    {
                        //then the watcher exists, but we haven't registered it just yet.
                        PreExistingColliders.AddRange(addedColliders.ExistingCollider);
                        AddedStaticConvexColliders.AddRange(addedColliders.AddedStaticColliders);

                        foreach (var collider in addedColliders.AddedStaticColliders)
                        {
                            if (i == 0)
                                bounds = collider.bounds;
                            else
                                bounds.Encapsulate(collider.bounds);
                        }

                        AddedDynamicConvexColliders.AddRange(addedColliders.AddedDynamicColliders);

                        preExistingWatchers.Add(mesh.gameObject, addedColliders);
                    }
                }

                i++;
            }

            Bounds = bounds;
        }

        private void RegisterExistingColliders(Dictionary<GameObject, GrabbitCollidersWatcher> preExistingWatchers,
            Dictionary<GameObject, GrabbitCollidersWatcher> addedWatchers)
        {
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                if (preExistingWatchers.ContainsKey(col.gameObject))
                    continue;

                var addedColliders = col.GetComponent<GrabbitCollidersWatcher>();
                var addedAlready = addedWatchers.ContainsKey(col.gameObject);
                if (!addedColliders || addedAlready)
                {
                    if (!addedAlready)
                    {
                        addedColliders = col.gameObject.AddComponent<GrabbitCollidersWatcher>();
                        AddedCollidersList.Add(addedColliders);
                    }

                    addedColliders.ExistingCollider.Add(col);
                    PreExistingColliders.Add(col);
                    if (!addedAlready)
                        addedWatchers.Add(col.gameObject, addedColliders);
                }
                else
                {
                    PreExistingColliders.AddRange(addedColliders.ExistingCollider);
                    AddedStaticConvexColliders.AddRange(addedColliders.AddedStaticColliders);
                    AddedDynamicConvexColliders.AddRange(addedColliders.AddedDynamicColliders);
                    preExistingWatchers.Add(col.gameObject, addedColliders);
                }
            }
        }

        public void ConfigureSelectionMode(GrabbitSettings settings,
            ColliderMeshContainer colliderMeshContainer = null)
        {
            if (!Body)
            {
                Body = GetComponent<Rigidbody>();

                if (!Body)
                {
                    Body = gameObject.AddComponent<Rigidbody>();
                    WasRigidBodyAdded = true;
                }
                else
                {
                    RigidBodySave.RegisterRigidBody(Body);
                }
            }

            UndoState = new UndoState(Body);

            if (settings.UseDynamicNonConvexColliders && colliderMeshContainer)
            {
                //averaging the estimated added convex mesh colliders to 3
                AddedDynamicConvexColliders = new List<MeshCollider>(AddedStaticConvexColliders.Count * 3);

                foreach (var watcher in AddedCollidersList)
                {
                    foreach (var col in watcher.AddedStaticColliders)
                    {
                        if (!colliderMeshContainer.IsMeshDefined(col.sharedMesh))
                            //then it needs to be generated first
                            colliderMeshContainer.RegisterCollidersFromSelection(col, settings);

                        var meshes = colliderMeshContainer.GetMeshListAndRegenerateIfNeeded(col.sharedMesh, settings);

                        if (meshes.Count > 0)
                        {
                            foreach (var mesh in meshes)
                            {
                                AddMeshColliderToDynamicColliders(settings, col, mesh, watcher);
                            }
                        }
                        else
                        {
                            AddMeshColliderToDynamicColliders(settings, col, col.sharedMesh, watcher);
                        }
                    }
                }
            }
            else
            {
                AddedDynamicConvexColliders = new List<MeshCollider>(0);
            }

            SelectionConfigured = true;
        }

        private void AddMeshColliderToDynamicColliders(GrabbitSettings settings, MeshCollider col, Mesh mesh,
            GrabbitCollidersWatcher watcher)
        {
            var existingColliders = col.gameObject.GetComponents<MeshCollider>();

            //check for existing colliders to not have to duplicate the ones of other handlers, if subobjects are involved
            var existing = existingColliders.FirstOrDefault(_ => _.sharedMesh == mesh);

            var mc = existing ? existing : col.gameObject.AddComponent<MeshCollider>();

            if (!existing)
            {
                if (!settings.useLowQualityConvexCollidersOnSelection)
                    mc.cookingOptions &= ~MeshColliderCookingOptions.UseFastMidphase;
                mc.sharedMesh = mesh;
                mc.convex = true;
            }

            AddedDynamicConvexColliders.Add(mc);
            watcher.AddedDynamicColliders.Add(mc);
        }

        public void ActivateBackgroundMode()
        {
            if (GrabbitEditor.Instance.CurrentSettings.UsePredefinedColliders && PreExistingColliders.Count > 0)
            {
                foreach (var preExistingCollider in PreExistingColliders)
                {
                    preExistingCollider.enabled = true;
                }

                if (SelectionConfigured)
                    foreach (var convexCollider in AddedDynamicConvexColliders)
                        convexCollider.enabled = false;


                if (StaticConfigured)
                {
                    foreach (var col in AddedStaticConvexColliders) col.enabled = false;

                    if (Body)
                    {
                        Body.collisionDetectionMode = CollisionDetectionMode.Discrete;
                        Body.isKinematic = true;
                    }
                }
            }
            else
            {
                if (SelectionConfigured)
                    foreach (var convexCollider in AddedDynamicConvexColliders)
                        convexCollider.enabled = false;

                if (StaticConfigured)
                {
                    foreach (var col in AddedStaticConvexColliders) col.enabled = true;

                    if (Body)
                    {
                        Body.collisionDetectionMode = CollisionDetectionMode.Discrete;
                        Body.isKinematic = true;
                    }
                }
            }

            foreach (var addedJoint in AddedJoints)
            {
                DestroyImmediate(addedJoint);
            }

            AverageCollisionNormal = Vector3.zero;
            IsCollidingWithStaticGeo = false;
            CollidingStaticGeo.Clear();
            IsInSelectionMode = false;
            enabled = false;
        }

        public void ActivateSelectionMode(GrabbitSettings settings, ColliderMeshContainer colliderMeshContainer = null)
        {
            lastSettings = settings;

            //raises the overall perfs
            InternalEditorUtility.SetIsInspectorExpanded(this, true);
            InternalEditorUtility.SetIsInspectorExpanded(transform, false);
            InternalEditorUtility.SetIsInspectorExpanded(Body, false);

            if (!SelectionConfigured)
                ConfigureSelectionMode(settings, colliderMeshContainer);

            IsInSelectionMode = true;

            if (settings.UsePredefinedColliders && PreExistingColliders.Count > 0)
            {
                foreach (var preExistingCollider in PreExistingColliders)
                {
                    preExistingCollider.enabled = true;
                }

                foreach (var col in AddedStaticConvexColliders)
                {
                    col.enabled = false;
                    if (!settings.UseDynamicNonConvexColliders)
                    {
                        col.convex = true;
                        InternalEditorUtility.SetIsInspectorExpanded(col, false);
                    }
                }


                foreach (var col in AddedDynamicConvexColliders)
                {
                    col.enabled = false;
                    InternalEditorUtility.SetIsInspectorExpanded(col, false);
                }
            }
            else
            {
                foreach (var col in AddedStaticConvexColliders)
                    if (settings.UseDynamicNonConvexColliders)
                    {
                        col.enabled = false;
                    }
                    else
                    {
                        col.convex = true;
                        InternalEditorUtility.SetIsInspectorExpanded(col, false);
                    }

                foreach (var col in AddedDynamicConvexColliders)
                {
                    col.enabled = true;
                    InternalEditorUtility.SetIsInspectorExpanded(col, false);
                }
            }

            //ensuring sub rigidbodies are detected and handled properly
            foreach (var body in GetComponentsInChildren<Rigidbody>())
            {
                if (body == Body)
                    continue;
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                body.isKinematic = false;
                body.angularDrag = 99999;

                FixedJoint join = gameObject.AddComponent<FixedJoint>();
                join.connectedBody = body;
                join.enableCollision = false;
                AddedJoints.Add(join);
            }


            Body.isKinematic = false;
            Body.useGravity = false;
            Body.detectCollisions = true;
            Body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Body.WakeUp();

            UndoState.RecordUndoBodyMove(Body);
            enabled = true;
            IsInSelectionMode = true;
        }


        public void OnDestroy()
        {
            EditorUtility.SetDirty(gameObject);
            if (InstantDestroyFlag)
                Cleanup();
            else
                EditorApplication.delayCall += Cleanup;
        }

        public void Cleanup()
        {
            EditorApplication.delayCall -= Cleanup;

            if (StaticConfigured)
            {
                foreach (var addedJoint in AddedJoints)
                {
                    DestroyImmediate(addedJoint);
                }

                //body can be null from another handler in case of subobjects
                if (WasRigidBodyAdded && Body)
                {
                    DestroyImmediate(Body);
                    Body = null;
                }
                else if (Body)
                {
                    //RigidBodySave.UnregisterRigidBody(Body);
                }

                PreExistingColliders.Clear();
                AddedStaticConvexColliders.Clear();

                StaticConfigured = false;
            }

            if (SelectionConfigured)
            {
                AddedDynamicConvexColliders.Clear();
                SelectionConfigured = false;
            }

            for (var i = AddedCollidersList.Count - 1; i >= 0; i--)
            {
                var watcher = AddedCollidersList[i];
                if (watcher)
                    DestroyImmediate(watcher);
            }
        }

        public void NotifyHandlerNowSelected(GrabbitHandler otherHandler)
        {
            foreach (var otherCollider in otherHandler.AddedDynamicConvexColliders)
                CollidingStaticGeo.Remove(otherCollider);

            foreach (var staticCollider in otherHandler.AddedStaticConvexColliders)
                CollidingStaticGeo.Remove(staticCollider);

            if (CollidingStaticGeo.Count == 0)
            {
                IsCollidingWithStaticGeo = false;
                AverageCollisionNormal = Vector3.zero;
                contacts.Clear();
            }
        }

        //stupid trick because of collision exit bug

        //giving it a bunch of frames for safety
        public static int FrameDifferenceConcern = 5;
        public int NumberOfFramesWithoutDifference;
        public bool CollisionStayCalls;

        public void OnCollisionEnter(Collision other)
        {
            CollisionStayCalls = false;
            NumberOfFramesWithoutDifference = 0;

            //TODO make it that when there is no collision, the object just teleport directly back to where they should be
            if (!lastSettings.UseSoftCollision || !enabled)
                return;

            var handler = other.gameObject.GetComponent<GrabbitHandler>();
            if (handler && !handler.IsInSelectionMode && !CollidingStaticGeo.Contains(other.collider))
            {
                IsCollidingWithStaticGeo = true;
                CollidingStaticGeo.Add(other.collider);
            }
        }


        public void OnCollisionStay(Collision other)
        {
            if (!enabled || !CollidingStaticGeo.Contains(other.collider))
                return;

            AverageCollisionNormal = Vector3.zero;
            contacts.Clear();
            other.GetContacts(contacts);

            foreach (var contact in contacts) AverageCollisionNormal += contact.normal;

            AverageCollisionNormal.Normalize();
            CollisionStayCalls = true;
        }

        public void OnCollisionExit(Collision other)
        {
            if (!enabled || !lastSettings.UseSoftCollision)
            {
                return;
            }

            CollidingStaticGeo.Remove(other.collider);

            if (CollidingStaticGeo.Count == 0)
            {
                NotifyNoMoreCollisions();
            }
        }

        public void NotifyNoMoreCollisions(bool alsoClear = false)
        {
            if (alsoClear)
                CollidingStaticGeo.Clear();
            IsCollidingWithStaticGeo = false;
            AverageCollisionNormal = Vector3.zero;
            contacts.Clear();
        }

        public void OnDrawGizmosSelected()
        {
            if (lastSettings && lastSettings.debugCollisionDirection)
            {
                var position = transform.position;
                Gizmos.DrawLine(position, position + AverageCollisionNormal);
                Gizmos.color = Color.yellow;
                foreach (var contact in contacts) Gizmos.DrawLine(contact.point, contact.point + contact.normal);
            }
        }
    }
}
#endif