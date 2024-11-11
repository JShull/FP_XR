
namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using FuzzPhyte.Utility.Meta;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// This assumes that there is a collider directly on this object of some sorts...
    /// </summary>
    public class FPWorldItem : MonoBehaviour
    {
        [Tooltip("Assumes this collider is directly on this object")]
        public Collider ItemCollider;
        [Tooltip("The Items Transform")]
        public Transform ItemTransform;
        [Tooltip("The RB we are working with")]
        public Rigidbody ItemRigidBody;
        [Tooltip("Event to reset the world")]
        public UnityEvent ResetWorldEvent;
        [Space]
        [SerializeField] protected Vector3 _startPosition;
        [SerializeField] protected Quaternion _startRotation;
        [Tooltip("Managing cache for RB settings")]
        protected bool _isKinematic;
        protected bool _useGravity;
        #region FP Related
        public FP_Tag TheFPTag;
        public FPXRSocketTag SocketTag;
        public FP_Data TheFPData;
        [SerializeField]
        protected FPSocket currentSocket;
        #endregion
        /// <summary>
        /// If we get attached to something
        /// </summary>
        /// <param name="parent"></param>
        public virtual void Attached(FPSocket parent)
        {
            currentSocket = parent;
        }
        public virtual void Detached()
        {
            currentSocket = null;
        }
        public virtual void Start()
        {
            if(ItemCollider == null)
            {
                Debug.LogError($"I am missing a collider and I am throwing myself off a cliff");
                Destroy(this);
                return;
            }
            if (ItemTransform == null)
            {
                ItemTransform = this.transform;
            }
            _startPosition = ItemTransform.position;
            _startRotation = ItemTransform.rotation;
            if (ItemRigidBody != null)
            {
                _isKinematic = ItemRigidBody.isKinematic;
                _useGravity = ItemRigidBody.useGravity;
            }
        }
        public virtual void ResetLocation()
        {
            ItemTransform.position = _startPosition;
            ItemTransform.rotation = _startRotation;
            ResetWorldEvent.Invoke();
            if (ItemRigidBody != null)
            {
                ItemRigidBody.angularVelocity = Vector3.zero;
                ItemRigidBody.linearVelocity = Vector3.zero;

                ItemRigidBody.useGravity = _useGravity;
                ItemRigidBody.isKinematic = _isKinematic;
            }
        }
    }
}
