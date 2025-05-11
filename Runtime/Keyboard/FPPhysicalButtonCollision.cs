using UnityEngine;

namespace FuzzPhyte.XR
{
    public class FPPhysicalButtonCollision : MonoBehaviour
    {
        public FPPhysicalButton FPButton;
        [SerializeField] protected bool isPressed;
        protected Collider whoActivatedMe;

        public void OnTriggerEnter(Collider other)
        {
            if (!isPressed)
            {
                whoActivatedMe = other;
                FPButton.MoveToPosition(FPButton.PushedPosition,true);
                FPButton.Pressed();
                isPressed = true;
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (isPressed&&other==whoActivatedMe)
            {
                FPButton.MoveToPosition(FPButton.RestPosition,false);
                FPButton.Released();
                whoActivatedMe = null;
                isPressed = false;
            }
        }
        public void OnTriggerStay(Collider other)
        {
            
        }
    }
}
