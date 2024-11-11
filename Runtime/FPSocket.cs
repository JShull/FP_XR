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
            SocketStatus = XRInteractorState.Open;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(SocketStatus != XRInteractorState.Open)
            {
                return;
            }
            if (currentItem == null && other.TryGetComponent(out FPWorldItem worldItem))
            {
                if(worldItem.SocketTag != SocketRequirement)
                {
                    return;
                }
                AttachItem(worldItem);
                SocketStatus = XRInteractorState.IsOccupied;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (currentItem != null && other.GetComponent<FPWorldItem>() == currentItem)
            {
                DetachItem();
                SocketStatus = XRInteractorState.Open;
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
                currentItem = null;
            }
        }

        public bool IsOccupied()
        {
            return currentItem != null;
        }
    }
}
