namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Ray;
    using Unity.Mathematics;
    using FuzzPhyte.Utility;
    public class FPSurfaceRay : MonoBehaviour, IFPRaySetup
    {
        #region Setup Variables
        [Header("Raycaster!")]
        public string RaycastInformation;
        public SO_FPRaycaster SurfaceLockRayData;
        public SO_FPRaycaster InverseSurfaceRayData;
        public Transform RaycastOrigin;
        public Transform RaycastEndDir;

        [Header("Surface Lock Settings")]
        public bool ActivateSurfaceLock; // Boolean flag to enable/disable surface lock
        public GameObject ObjectToLock; // The object you want to lock to the surface
        public Renderer ObjectRenderer; // Renderer to calculate the bounding box
        #endregion
        #region Interface Requirements
        public SO_FPRaycaster FPRayInformation
        {
            get { return SurfaceLockRayData; }
            set { SurfaceLockRayData = value; }
        }
        public Transform RayOrigin
        {
            get { return RaycastOrigin; }
            set { RaycastOrigin = value; }
        }
        //quick fix for cases in which we end up destroying endpoints
        public float3 RayDirection
        {
            get
            {
                if (RaycastEndDir == null || RaycastOrigin == null)
                {
                    return Vector3.zero;
                }
                return Vector3.Normalize(RaycastEndDir.position - RaycastOrigin.position);
            }
            set { RayDirection = value; }
        }
        protected FP_Raycaster _raycaster;
        protected FP_RayArgumentHit _rayHit;
        public FP_Raycaster Raycaster { get { return _raycaster; } set { _raycaster = value; } }
        public virtual void SetupRaycaster()
        {
            _raycaster = new FP_Raycaster(this);
        }
        #endregion
        protected FPSurfaceLock fPSurfaceLock;
        [SerializeField] protected bool SurfaceLocked;
        public GameObject GhostPrefabVariant;
        [SerializeField] private GameObject ghostCreated;
        protected virtual void Awake()
        {
            SetupRaycaster();
        }
        protected virtual void Start()
        {
            fPSurfaceLock = new FPSurfaceLock();
            ghostCreated= fPSurfaceLock.InitializeGhostObject(ObjectToLock.transform, GhostPrefabVariant);
        }
        public virtual void OnEnable()
        {
            _raycaster.OnFPRayFireHit += OnRayStay;
            _raycaster.OnFPRayEnterHit += OnRayEnter;
            _raycaster.OnFPRayExit += OnRayExit;
            _raycaster.ActivateRaycaster();
        }
        public virtual void OnDisable()
        {
            _raycaster.OnFPRayFireHit -= OnRayStay;
            _raycaster.OnFPRayEnterHit -= OnRayEnter;
            _raycaster.OnFPRayExit -= OnRayExit;
            _raycaster.DeactivateRaycaster();
        }
        #region Callback Functions for Raycast Delegates
        public virtual void OnRayEnter(object sender, FP_RayArgumentHit arg)
        {
            if (arg.HitObject != null)
            {
                //Debug.LogWarning($"RAY Enter: {arg.HitObject.name}");
            }

            _rayHit = arg;
        }
        public virtual void OnRayStay(object sender, FP_RayArgumentHit arg)
        {
            if (arg.HitObject != null)
            {
                //Debug.LogWarning($"RAY Stay: {arg.HitObject.name}");
            }

            _rayHit = arg;
        }
        public virtual void OnRayExit(object sender, FP_RayArgumentHit arg)
        {
            if (arg.HitObject != null)
            {
                //Debug.LogWarning($"RAY Exit: {arg.HitObject.name}");
            }

            _rayHit = arg;
        }
        #endregion
        
        public virtual void Update()
        {
            if (ActivateSurfaceLock && ObjectToLock != null && ObjectRenderer != null)
            {
                // Calculate the bounding box of the object
                var boundingBoxInfo = FP_UtilityData.CreateBoundingBox(ObjectToLock.transform.position,ObjectToLock.transform.rotation, ObjectRenderer);

                if (boundingBoxInfo.HasValue)
                {
                    // Try to lock the object to the surface
                    //var bBox = boundingBoxInfo.Value;
                    SurfaceLocked = fPSurfaceLock.LockToSurface(_raycaster, InverseSurfaceRayData, ObjectToLock, ObjectRenderer,collisionCheck: true, alignment: false);
                    /*
                    if (locked)
                    {
                        Debug.Log("Object successfully locked to surface.");
                        //ActivateSurfaceLock = false;
                    }
                    else
                    {
                        Debug.Log("Failed to lock object to surface.");
                    }
                    */
                }
                else
                {
                    Debug.LogError("BoundingBoxInfo could not be calculated.");
                }
                if (!SurfaceLocked && ghostCreated != null)
                {
                    ghostCreated.SetActive(false);
                }
            }
            if(!ActivateSurfaceLock && SurfaceLocked && ghostCreated!=null)
            {
                //we have a surface and we want to jump our item to this location of our ghost and then turn it off
                ObjectToLock.transform.position = ghostCreated.transform.position;
                ObjectToLock.transform.rotation = ghostCreated.transform.rotation;
                ghostCreated.SetActive(false);
                SurfaceLocked = false;
            }
        }

        /// <summary>
        /// Draw Gizmos to visualize the raycast in the Unity Editor
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == this.gameObject)
            {
                if (RaycastOrigin != null && RaycastEndDir != null)
                {
                    // Draw the ray line
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(RaycastOrigin.position, RaycastEndDir.position);

                    // Draw spheres at the start and end points for clarity
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(RaycastOrigin.position, 0.1f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(RaycastEndDir.position, 0.1f);
                }
            }
            
#endif
        }
    }
}
