
namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections;

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
        public FPXRInteractableLabel InteractableLabel;
        public UnityEvent ActivatedInteractionLabelEvent;       
        
        [Space]
        [SerializeField] protected Vector3 _startPosition;
        [SerializeField] protected Quaternion _startRotation;
        [Tooltip("Managing cache for RB settings")]
        protected bool _isKinematic;
        protected bool _useGravity;
        protected float lastTimeTimerRan=0;
        protected float lastTimerValue=0;
        #region FP Related
        [Space]
        [Header("FP Data")]
        //[Tooltip("Generic Data Object")]
        //public FP_Data TheFPData;
        [Tooltip("Place to centralize all the data for this item")]
        public XRDetailedLabelData DetailedLabelData;
        public FP_Language StartingFPLanguage;
        public bool UseSupportCombinedVocabData = false;
        public bool StartDetailedLabelOnStart;
        public UnityEvent DetailedLabelActivated;
        public UnityEvent DetailedLabelDeactivated;
        [SerializeField] protected List<UnityEngine.Object> interfaceObjects = new List<UnityEngine.Object>();
        public List<IFPXRLabel> LabelInterfaces
        {
            get
            {
                var interfaces = new List<IFPXRLabel>();
                for (int i = 0; i < interfaceObjects.Count; i++)
                {
                    var obj = interfaceObjects[i];
                    if (obj is IFPXRLabel label)
                    {
                        interfaces.Add(label);
                    }
                }
                return interfaces;
            }
        }
        protected virtual void OnValidate()
        {
            // Validate that all objects in the list implement the interface
            for (int i = 0; i < interfaceObjects.Count; i++)
            {
                if (!(interfaceObjects[i] is IFPXRLabel))
                {
                    Debug.LogError($"Object {interfaceObjects[i]?.name} does not implement IPFXRLabel.");
                    interfaceObjects[i] = null; // Clear invalid entries
                }
            }
        }
        [Space]
        [SerializeField]
        protected FPSocket currentSocket;
        public FPSocket CurrentSocket { get { return currentSocket; } } 
        [SerializeField]
        protected XRHandedness handState;
        public XRHandedness InHandState { get { return handState; } }
        [Space]
        [Header("Force Drop Unity Events")]
        public UnityEvent OnForceDropEvent;
        public UnityEvent OnEndFrameForceDropEvent;
        #endregion
        #region Action Related
        // Action events for external listeners
        public event Action<FPWorldItem, XRHandedness> ItemGrabbed;
        public event Action<FPWorldItem, XRHandedness> ItemDropped;
        public event Action<FPWorldItem, XRHandedness> ItemForceDropped;
        public event Action<FPWorldItem> ItemDestroyed;
        public event Action<FPWorldItem> ItemSpawned;
        public event Action<FPWorldItem, FPSocket> ItemSocketSet;
        public event Action<FPWorldItem, FPSocket> ItemSocketRemoved;
        public event Action<FPWorldItem, XRHandedness> ItemRaySelect;
        public event Action<FPWorldItem> ItemRayUnselected;
        public event Action<FPWorldItem> ItemLabelActivated;
        public event Action<FPWorldItem> ItemCamRayActivated;
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
        /// <summary>
        /// Stubout for if we need to process the data object
        /// </summary>
        /// <param name="interactionType"></param>
        /// <param name="passedObject"></param>
        public virtual void EventActionPassedBack(XRInteractionStatus interactionType, object passedObject)
        {
            
        }
        public virtual void EventActionPassedBack(XRInteractorType interactor,XRInteractionStatus interactionType, XRHandedness hand)
        {
            switch (interactionType)
            {
                case XRInteractionStatus.None:
                    break;
                case XRInteractionStatus.Hover:
                    break;
                case XRInteractionStatus.Select:
                    if(interactor == XRInteractorType.Ray)
                    {
                        RayInteracted((int)hand);
                        break;
                    }
                    if(interactor == XRInteractorType.Grab)
                    { 
                        PickedUpItem((int)hand);
                        break;
                    }
                    break;
                case XRInteractionStatus.Unselect:
                    if (interactor == XRInteractorType.Ray)
                    {
                        RayUnselect();
                        break;
                    }
                    if (interactor == XRInteractorType.Grab)
                    {
                        DroppedItem();
                        break;
                    }
                    break;
                case XRInteractionStatus.InteractorViewAdded:
                    break;
                case XRInteractionStatus.InteractorViewRemoved:
                    break;
                case XRInteractionStatus.SelectingInteractorViewAdded:
                    break;
                case XRInteractionStatus.SelectingInteractorViewRemoved:
                    break;
            }
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
        public virtual void RayUnselect()
        {
            ItemRayUnselected?.Invoke(this);
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
        public virtual void ForceDrop()
        {
            if (this.handState != XRHandedness.NONE)
            {

                ItemForceDropped?.Invoke(this, this.handState);
            }
            else
            {
                ItemForceDropped?.Invoke(this, XRHandedness.NONE);
            }
            this.handState = XRHandedness.NONE;
            OnForceDropEvent.Invoke();
            StartCoroutine(DelayEndFrameAction());
        }
        protected IEnumerator DelayEndFrameAction()
        {
            yield return new WaitForEndOfFrame();
            OnEndFrameForceDropEvent.Invoke();
        }
        /// <summary>
        /// Called if something else spawns this item - don't want this in start/onEnable
        /// basically an external way for a spawner system to fire this off without knowing it exactly...
        /// </summary>
        public virtual void ItemSpawnedEvent()
        {
            ItemSpawned?.Invoke(this);
        }
        /// <summary>
        /// This is if we are engaging with the item via maybe another camera system of some sorts
        /// </summary>
        public virtual void ItemCameraRayEvent()
        {
            ItemCamRayActivated?.Invoke(this);
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
            //setup details on label if we have them
            for(int i = 0; i < LabelInterfaces.Count; i++)
            {
                LabelInterfaces[i].SetupLabelData(DetailedLabelData, StartingFPLanguage, StartDetailedLabelOnStart, UseSupportCombinedVocabData);
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
        #region Label Related Information
        public virtual void SetupInteractionLabelText()
        {
            if (InteractableLabel != null)
            {
                InteractableLabel.SetupLabelData(DetailedLabelData, StartingFPLanguage, false);
            } 
        }
        public virtual void ActivateInteractionLabel(bool state)
        {
            if (InteractableLabel != null)
            {
                InteractableLabel.gameObject.SetActive(state);
                InteractableLabel.DisplayVocabTranslation(StartingFPLanguage);
                ActivatedInteractionLabelEvent.Invoke();
                if (state)
                {
                    ItemLabelActivated?.Invoke(this);
                }
            }
        }
        /// <summary>
        /// Turns on the label and then will turn it off after a certain amount of time
        /// </summary>
        /// <param name="time"></param>
        public virtual void ActivateInteractionLabelTimer(float time)
        {
            if (InteractableLabel != null)
            {
                InteractableLabel.gameObject.SetActive(true);
                InteractableLabel.DisplayVocabTranslation(StartingFPLanguage);
                ActivatedInteractionLabelEvent.Invoke();
                ItemLabelActivated?.Invoke(this);
                if (FP_Timer.CCTimer != null)
                {
                    FP_Timer.CCTimer.StartTimer(time, () => { InteractableLabel.gameObject.SetActive(false); });
                }
            }
            
        }
    
        public virtual void ActivateDetailedLabel()
        {
            for(int i=0;i< LabelInterfaces.Count; i++)
            {
                LabelInterfaces[i].ShowAllRenderers(true);
            }
            ItemLabelActivated?.Invoke(this);
            DetailedLabelActivated.Invoke();
        }
        public virtual void DeactivateAllLabels()
        {
            for (int i = 0; i < LabelInterfaces.Count; i++)
            {
                LabelInterfaces[i].ForceHideRenderer();
            }
            DetailedLabelDeactivated.Invoke();
        }
        public virtual void ActivateDetailedLabelTimer(float time)
        {
            if (FP_Timer.CCTimer != null)
            {
                if(Time.time-lastTimeTimerRan> lastTimerValue)
                {
                    //timer has already deactivated by now
                    FP_Timer.CCTimer.StartTimer(time, () => { DeactivateAllLabels(); });
                    lastTimeTimerRan = Time.time;
                    lastTimerValue = time;
                    Debug.LogWarning($"Timer Activated, Last Time Timer Ran = {lastTimeTimerRan.ToString()}, Last Timer Value = {lastTimerValue}");
                    ItemLabelActivated?.Invoke(this);
                }
                else
                {
                    //already have a timer in the queue
                    Debug.LogWarning($"Already have a timer for deactivating in the queue! - should be running at {lastTimeTimerRan + lastTimerValue} - and we are currently at {Time.time}");
                }
            }
        }
        public virtual void ShowAllTheRenderers()
        {
            for (int i = 0; i < LabelInterfaces.Count; i++)
            {
                LabelInterfaces[i].ShowAllRenderers(true);
            }
        }
        public virtual void HideAllTheRenderers()
        {
            for (int i = 0; i < LabelInterfaces.Count; i++)
            {
                LabelInterfaces[i].ShowAllRenderers(false);
            }
        }
        public virtual void DetailedLabelVocabTranslationDisplay()
        {
            for(int i = 0; i < LabelInterfaces.Count; i++)
            {
                LabelInterfaces[i].DisplayVocabTranslation(StartingFPLanguage);
            }
            ItemLabelActivated?.Invoke(this);
        }
        
        #endregion
    }
}
