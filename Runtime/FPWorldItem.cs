
namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using FuzzPhyte.Utility.Meta;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using TMPro;

    /// <summary>
    /// This assumes that there is a collider directly on this object of some sorts...
    /// Needs to know when it gets picked up and dropped to maintain state
    /// Manages connection points to labels if needed
    /// One-stop shop for references associated with what a 'world item' might actually be
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
        [Header("Label Related")]
        [Tooltip("The Interaction Label")]
        public GameObject InteractionLabelRoot;
        public TMP_Text InteractionDisplayText;
        public UnityEvent ActivatedInteractionLabelEvent;        
        [Space]
        [SerializeField] protected Vector3 _startPosition;
        [SerializeField] protected Quaternion _startRotation;
        [Tooltip("Managing cache for RB settings")]
        protected bool _isKinematic;
        protected bool _useGravity;
        #region FP Related
        public FP_Tag TheFPTag;
        public FP_Data TheFPData;
        [SerializeField]
        protected FPSocket currentSocket;
        [SerializeField]
        protected XRHandedness handState;
        public XRHandedness InHandState { get { return handState; } }
        #endregion
        #region Action Related
        // Action events for external listeners
        public event Action<FPWorldItem, XRHandedness> ItemGrabbed;
        public event Action<FPWorldItem, XRHandedness> ItemDropped;
        public event Action<FPWorldItem> ItemDestroyed;
        public event Action<FPWorldItem> ItemSpawned;
        public event Action<FPWorldItem, FPSocket> ItemSocketSet;
        public event Action<FPWorldItem, FPSocket> ItemSocketRemoved;
        public event Action<FPWorldItem, XRHandedness> ItemRaySelect;
        #endregion

        #region Methods for Actions
        /// these are called probably from some sort of Unity Event or external system

        /// <summary>
        /// If we get attached to something
        /// </summary>
        /// <param name="parent"></param>
        public virtual void LinkSocket(FPSocket parent)
        {
            currentSocket = parent;
            ItemSocketSet?.Invoke(this, parent);
        }
        public virtual void PickedUpItem(int handState)
        {
            this.handState = (XRHandedness)handState;
            ItemGrabbed?.Invoke(this, this.handState);
        }
        public virtual void RayInteracted(int handState)
        {
            this.handState = (XRHandedness)handState;
            ItemRaySelect?.Invoke(this, this.handState);
        }
        public virtual void DroppedItem()
        {
            if (this.handState != XRHandedness.NONE)
            {

                ItemDropped?.Invoke(this, this.handState);
            }
            else
            {
                ItemDropped?.Invoke(this, XRHandedness.NONE);
            }
            this.handState = XRHandedness.NONE;
        }
        public virtual void UnlinkSocket(FPSocket passedParent)
        {
            if (currentSocket == passedParent)
            {
                currentSocket = null;
                ItemSocketRemoved?.Invoke(this, passedParent);
            }
        }
        /// <summary>
        /// Called if something else spawns this item - don't want this in start/onEnable
        /// basically an external way for a spawner system to fire this off without knowing it exactly...
        /// </summary>
        public virtual void ItemSpawnedEvent()
        {
            ItemSpawned?.Invoke(this);
        }
        #endregion
        public virtual void Start()
        {
            if (ItemCollider == null)
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
        public virtual void SetParentNull()
        {
            if (ItemTransform != null)
            {
                ItemTransform.SetParent(null);
            }
        }
        public virtual void SetParent(Transform parent)
        {
            if (ItemTransform != null)
            {
                ItemTransform.SetParent(parent);
            }
        }
        public virtual void SetupInteractionLabelText(string text)
        {
            if (InteractionDisplayText != null)
            {
                InteractionDisplayText.text = text;
            }
        }
        public virtual void ActivateInteractionLabel(bool state)
        {
            if (InteractionLabelRoot != null)
            {
                InteractionLabelRoot.SetActive(state);
                ActivatedInteractionLabelEvent.Invoke();
            }
        }
        /// <summary>
        /// Turns on the label and then will turn it off after a certain amount of time
        /// </summary>
        /// <param name="time"></param>
        public virtual void ActivateInteractionLabelTimer(float time)
        {
            if (InteractionLabelRoot != null)
            {
                InteractionLabelRoot.SetActive(true);
                ActivatedInteractionLabelEvent.Invoke();
                if (FP_Timer.CCTimer != null)
                {
                    FP_Timer.CCTimer.StartTimer(time, () => { InteractionLabelRoot.SetActive(false); });
                }
            }
        }
    }
}
