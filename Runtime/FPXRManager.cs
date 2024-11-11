using UnityEngine;

namespace FuzzPhyte.XR
{
    public class FPXRManager : MonoBehaviour
    {
        [SerializeField]protected GameObject referenceToLeftHand;
        public GameObject LeftHand { get { return referenceToLeftHand; } }
        [SerializeField]protected GameObject referenceToRightHand;
        public GameObject RightHand { get { return referenceToRightHand; } }
        public static FPXRManager Instance { get; private set; }
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        public void SetupLeftHand(GameObject passedHand)
        {
            this.referenceToLeftHand = passedHand;
        }
        public void SetupRightHand(GameObject passedHand)
        {
            this.referenceToRightHand = passedHand;
        }
    }
}
