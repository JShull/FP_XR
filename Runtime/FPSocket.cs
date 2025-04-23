namespace FuzzPhyte.XR
{
    using UnityEngine;
    using UnityEngine.Events;

    [ExecuteInEditMode]
    public class FPSocket : MonoBehaviour
    {
        public int socketIndex; // Index in the socketPositions array
        [SerializeField]protected FPWorldItem currentItem; // Reference to the item occupying this socket
        public FPWorldItem CurrentItem { get { return currentItem; } }
        [SerializeField]FPXRCase parentCase; // Reference to the parent case
        [SerializeField]protected Bounds SocketSize; //later if we need to shrink something to fit our socket we know what that might look like
        [SerializeField]protected Collider socketCollider; // Trigger collider for this socket
        [Tooltip("Whatever item comes into our socket has to have this Tag")]
        public FPXRSocketTag SocketRequirement;
        public XRInteractorState SocketStatus;
        public XRInteractorState SpaceTaken;
        public bool SetupOnStart=true;
        [Space]
        [Header("Unity Events")]
        public UnityEvent OnSocketGivenItem;
        public UnityEvent OnSocketRemovedItem;
        /// <summary>
        /// Main setup point for our FPSocket
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        /// <param name="theCase"></param>
        public virtual void PositionInCase(int index, Vector3 position, FPXRCase theCase=null)
        {
            socketIndex = index;
            this.transform.position = position;
            parentCase = theCase;
        }
        protected virtual void Start()
        {
            if (SetupOnStart)
            {
                socketCollider = GetComponent<Collider>();
                if (socketCollider == null || !socketCollider.isTrigger)
                {
                    Debug.LogError($"Socket at index {socketIndex} requires a trigger collider.");
                }
                SpaceTaken = XRInteractorState.Open;
                SocketStatus = XRInteractorState.None;
            }
        } 
        public virtual bool GivenItem(FPWorldItem item)
        {
            //Are we open?
            if(SpaceTaken != XRInteractorState.Open)
            {
                return false;
            }
            //do we have a world item on us, we have to
            var worldItem = item;
            
            var bitItem = item.GetComponent<FPBit>();
            //Has something been stored here that's a bit?
            if(SocketStatus == XRInteractorState.SocketBit)
            {
                //a bit owns this space, are we the returning bit?
                if(bitItem!=null)
                {
                    if(bitItem.Socket == this)
                    {
                        //we are returning to our socket via the Trigger
                        SpaceTaken = XRInteractorState.IsOccupied;
                        LinkItem(worldItem);
                        bitItem.ReturnedToSocket();
                        return true;
                    }
                }
                return false;
            }
            //is our socket status open?
            if(SocketStatus==XRInteractorState.Open)
            {
                //double check currentItem (I don't think I will need this check in the future)
                if(currentItem == null)
                {
                    if(bitItem != null)
                    {
                        //we are a bit...
                        if(bitItem.SocketTag != SocketRequirement)
                        {
                            //the wrong socket requirement (still a bit-cannot store here)
                            return false;
                        }
                        //we are a bit and the right socket requirement
                        SocketStatus = XRInteractorState.SocketBit;
                        SpaceTaken = XRInteractorState.IsOccupied;
                        LinkItem(worldItem);
                        bitItem.SetInSocket(this);
                        
                        return true;
                    }
                    else
                    {
                        //not a bit but an item we can place here
                        SpaceTaken = XRInteractorState.IsOccupied;
                        LinkItem(worldItem);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        public virtual bool RemoveItem(FPWorldItem item){
            //we do have something here
            if(SpaceTaken == XRInteractorState.IsOccupied)
            {
                var worldItem = item;
                //our item is leaving
                if (currentItem != null && worldItem == currentItem)
                {
                    //are we a bit?
                    var bitItem = worldItem.GetComponent<FPBit>();
                    if(SocketStatus==XRInteractorState.SocketBit)
                    {
                        if(bitItem!=null)
                        {
                            bitItem.PulledFromSocket(XRHandedness.NONE);
                            Debug.LogWarning($"Need to pass a correct hand here");
                        }
                    }
                    SpaceTaken = XRInteractorState.Open;
                    UnLinkItem();
                    return true;
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// Used for visual updates
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerEnter(Collider other)
        {

        }
        /// <summary>
        /// Used for visual updates
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerExit(Collider other)
        {
            
        }
 
        protected virtual void LinkItem(FPWorldItem item)
        {
            //pop from hand?
            currentItem = item;
            currentItem.LinkSocket(this);
            OnSocketGivenItem.Invoke();
        }

        protected virtual void UnLinkItem(bool isBit = false)
        {
            if (currentItem != null)
            {
                if(!isBit)
                {
                    currentItem.UnlinkSocket(this);
                    currentItem = null;
                }else
                {
                    currentItem = null;
                }
                OnSocketRemovedItem.Invoke();
            }
        }

        public bool IsOccupied()
        {
            return currentItem != null;
        }
    }

}

/*
Let OVR deal with the logic of sockets
My system needs to know what's 
*/