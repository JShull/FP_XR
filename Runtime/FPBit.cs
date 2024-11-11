namespace FuzzPhyte.XR
{
    using UnityEngine;

    public class FPBit : MonoBehaviour
    {
        public FPXRSocketTag SocketTag;
        [SerializeField]protected XRHandedness handedness;

        //visuals for the bit by socket state
        public GameObject MainBit;
        public GameObject OnSocketBitOpen; //up and ready to be grabbed out of the socket
        public GameObject OnSocketBitClosed; //in the socket and sort of not ready to be grabbed
        public FPSocket Socket { get; private set; }

        public void SetInSocket(FPSocket socket)
        {
            Socket = socket;
            MainBit.SetActive(false);
            OnSocketBitOpen.SetActive(false);
            OnSocketBitClosed.SetActive(true);
        }
        public void ReturnedToSocket()
        {
            MainBit.SetActive(false);
            OnSocketBitOpen.SetActive(true);
            OnSocketBitClosed.SetActive(false);
        }
    }
}
