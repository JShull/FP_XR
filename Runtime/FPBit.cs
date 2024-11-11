namespace FuzzPhyte.XR
{
    using UnityEngine;
    /// <summary>
    /// Sits at the same component level as FPWorldItem
    /// Responsible really for just tracking special items that can return to a socket and lock a socket down
    /// mainly responsible for visuals of that item by state
    /// Also responsible for scale of an item by state
    /// </summary>
    public class FPBit : MonoBehaviour
    {
        public FPXRSocketTag SocketTag;
        [SerializeField]protected XRHandedness handedness;
        public Vector3 InSocketScale;
        public Vector3 OutSocketScale;
        public GameObject MainInteractor;
        //visuals for the bit by socket state
        public GameObject MainVisualBit;
        public GameObject InSocketVisualOpen; //up and ready to be grabbed out of the socket
        public GameObject InSocketVisualClosed; //in the socket and sort of not ready to be grabbed
        public FPSocket Socket { get; private set; }

        /// <summary>
        /// First time going into a socket
        /// </summary>
        /// <param name="socket"></param>
        public virtual void SetInSocket(FPSocket socket)
        {
            Socket = socket;
            ReturnedToSocket();
        }
        /// <summary>
        /// We are being put into a socket
        /// </summary>
        public virtual void ReturnedToSocket()
        {
            handedness = XRHandedness.NONE;
            MainVisualBit.SetActive(false);
            InSocketVisualOpen.SetActive(true);
            InSocketVisualClosed.SetActive(false);
            MainInteractor.transform.localScale = InSocketScale;
        }
        /// <summary>
        /// We are being pulled from the socket
        /// Probably occurring on a Trigger Exit event
        /// </summary>
        /// <param name="handThatGrabbed"></param>
        public virtual void PulledFromSocket(XRHandedness handThatGrabbed)
        {
            handedness= handThatGrabbed;
            MainVisualBit.SetActive(true);
            InSocketVisualOpen.SetActive(false);
            InSocketVisualClosed.SetActive(false);
            MainInteractor.transform.localScale = OutSocketScale;
        }
        /// <summary>
        /// Called probably in a loop to close something external
        /// Don't want to do this if we are in a hand somewhere
        /// </summary>
        public virtual void SocketClosed()
        {
            if(handedness == XRHandedness.NONE)
            {
                MainInteractor.transform.localScale = InSocketScale;
                MainVisualBit.SetActive(false);
                InSocketVisualOpen.SetActive(false);
                InSocketVisualClosed.SetActive(true);
            }
        }
        /// <summary>
        /// When we need to remove it from the socket entirely
        /// </summary>
        protected virtual void FullyLeftSocket()
        {
            MainInteractor.transform.localScale = OutSocketScale;
            MainVisualBit.SetActive(true);
            InSocketVisualOpen.SetActive(false);
            InSocketVisualClosed.SetActive(false);
            Socket = null;
        }
    }
}
