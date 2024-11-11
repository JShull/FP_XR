namespace FuzzPhyte.XR
{
    using UnityEngine;
    [ExecuteInEditMode]
    public class FPSocket : MonoBehaviour
    {
        public int socketIndex; // Index in the socketPositions array
        protected FPWorldItem currentItem; // Reference to the item occupying this socket
        protected bool isKinematicState; // Cache the item's kinematic state
        protected bool useGravityState; // Cache the item's gravity state
        [SerializeField]protected Collider socketCollider; // Trigger collider for this socket
        [Tooltip("Whatever item comes into our socket has to have this Tag")]
        public FPXRSocketTag SocketRequirement;
        public XRInteractorState SocketStatus;
        public XRInteractorState SpaceTaken;

        public void PositionInCase(int index, Vector3 position)
        {
            socketIndex = index;
            this.transform.position = position;
        }
        private void Start()
        {
            socketCollider = GetComponent<Collider>();
            if (socketCollider == null || !socketCollider.isTrigger)
            {
                Debug.LogError($"Socket at index {socketIndex} requires a trigger collider.");
            }
            SpaceTaken = XRInteractorState.Open;
            SocketStatus = XRInteractorState.None;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(SpaceTaken != XRInteractorState.Open)
            {
                return;
            }
            if(SocketStatus == XRInteractorState.SocketBit)
            {
                //a bit owns this space, are we the returning bit?
                if(other.TryGetComponent(out FPBit bit))
                {
                    if(bit.Socket == this)
                    {
                        //we are returning to our socket via the Trigger
                        SpaceTaken = XRInteractorState.IsOccupied;
                        SocketStatus = XRInteractorState.SocketBit;
                        AttachItem(other.GetComponent<FPWorldItem>());
                        bit.ReturnedToSocket();
                        return;
                    }
                }
                return;
            }else if(currentItem == null && other.TryGetComponent(out FPWorldItem worldItem))
            {
                if(other.TryGetComponent(out FPBit bit))
                {
                    //we are a bit...
                    if(bit.SocketTag != SocketRequirement)
                    {
                        //the wrong socket requirement (still a bit)
                        SpaceTaken = XRInteractorState.Open;
                        SocketStatus = XRInteractorState.Open;
                        return;
                    }
                    //we are a bit and the right socket requirement
                    AttachItem(worldItem);
                    SocketStatus = XRInteractorState.SocketBit;
                    SpaceTaken = XRInteractorState.IsOccupied;
                    bit.SetInSocket(this);
                
                }
                else
                {
                    //not a bit but an item we can place here
                    AttachItem(worldItem);
                    SocketStatus = XRInteractorState.Open;
                    SpaceTaken = XRInteractorState.IsOccupied;
                }
            }
            
        }
        private void OnTriggerExit(Collider other)
        {
            if (currentItem != null && other.GetComponent<FPWorldItem>() == currentItem)
            {
                DetachItem();
                SpaceTaken = XRInteractorState.Open;
            }
        }

        private void AttachItem(FPWorldItem item)
        {
            currentItem = item;
            currentItem.transform.position = transform.position; // Snap item to socket position
            currentItem.transform.rotation = transform.rotation; // Align item rotation with socket
            isKinematicState = currentItem.ItemRigidBody.isKinematic; // Cache RB state
            useGravityState = currentItem.ItemRigidBody.useGravity; // Cache RB state
            currentItem.Attached(this);
            currentItem.ItemRigidBody.isKinematic = true; // Make item immovable when in socket
        }

        private void DetachItem()
        {
            if (currentItem != null)
            {
                currentItem.ItemRigidBody.isKinematic = isKinematicState; // Restore RB state
                currentItem.ItemRigidBody.useGravity = useGravityState; // Restore RB state
                currentItem.Detached();
                if(SocketStatus == XRInteractorState.SocketBit)
                {
                    //we are a bit and we are not nulling out the current item  
                }else{
                    currentItem = null;
                }
            }
        }

        public bool IsOccupied()
        {
            return currentItem != null;
        }
    }
}
