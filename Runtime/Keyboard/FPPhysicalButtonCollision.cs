using UnityEngine;

namespace FuzzPhyte.XR
{
    public class FPPhysicalButtonCollision : MonoBehaviour
    {
        public FPPhysicalButton FPButton;
        public LayerMask TriggeredLayers;
        [SerializeField] protected bool isPressed;
        protected Collider whoActivatedMe;
        [Tooltip("Delay between frames for collision checks")]
        [SerializeField] protected int frameDelay = 10;
        protected int triggerEnterFrameValue = 0;
        public bool UsePhysicsMode {get => usePhysicsMode; set => usePhysicsMode = value; }
        protected bool usePhysicsMode = true;
        public virtual void OnTriggerEnter(Collider other)
        {
            if (!usePhysicsMode)
            {
                return;
            }
            if (triggerEnterFrameValue < Time.frameCount)
            {
                triggerEnterFrameValue = Time.frameCount + frameDelay;
            }
            else
            {
                return;
            }
            // Check if the object's layer is in the allowed LayerMask
            if (AllowedCollision(other) == false)
            {
                return; // Not in the allowed layers
            }
            if (!isPressed)
            {
                whoActivatedMe = other;
                FPButton.MoveToPosition(FPButton.PushedPosition,true);
                FPButton.Pressed();
                isPressed = true;
            }
        }
        public virtual void OnTriggerExit(Collider other)
        {
            if (!usePhysicsMode)
            {
                return;
            }
            if (AllowedCollision(other) == false)
            {
                return; // Not in the allowed layers
            }
            if (isPressed&&other==whoActivatedMe)
            {
                FPButton.MoveToPosition(FPButton.RestPosition,false);
                FPButton.Released();
                whoActivatedMe = null;
                isPressed = false;
            }
        }
        public virtual void OnTriggerStay(Collider other)
        {
            if(!usePhysicsMode)
            {
                return;
            }
            if (isPressed && other == whoActivatedMe)
            {

            }
        }
        protected bool AllowedCollision(Collider other)
        {
            if (((1 << other.gameObject.layer) & TriggeredLayers) == 0)
            {
                return false;
            }
            return true;
        }
        #region For UI / Raycast Needs
        /// <summary>
        /// Use case might be something like OVR Event Wrapper
        /// </summary>
        public virtual void ManualKeyTriggerPressed()
        {
            isPressed = true;
            FPButton.MoveToPosition(FPButton.PushedPosition, true);
            FPButton.Pressed();
        }
        public virtual void ManualKeyTriggerReleased()
        {
            isPressed = false;
            FPButton.MoveToPosition(FPButton.RestPosition, false);
            FPButton.Released();
        }
        public virtual void ManualKeyHover()
        {
            FPButton.Hover();
        }
        public virtual void ManualKeyHoverExit()
        {
            FPButton.UnHover();
        }
        #endregion
    }
}
