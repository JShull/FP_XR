namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Utility;
    public class FPPhysicalButtonCollision : MonoBehaviour
    {
        public FPPhysicalButton FPButton;
        public LayerMask TriggeredLayers;
        public FPToolState ButtonState; //TriggerEnter = Activated, TriggerStay with Distance Confirm = ActiveUse, Exit = Ending
        //[SerializeField] protected bool isPressed;
        //[SerializeField] protected bool isDistanceStaying;
        [SerializeField] protected Collider whoActivatedMe;
        [Tooltip("Delay between frames for collision checks")]
        
        [SerializeField] protected int frameCountDelay = 50;
        protected int checkFrameCount = 0;
        public bool UsePhysicsMode {get => usePhysicsMode; set => usePhysicsMode = value; }
        protected bool usePhysicsMode = true;
        public virtual void OnEnable()
        {
            ButtonState = FPToolState.Ending;
        }
        public virtual void OnTriggerEnter(Collider other)
        {
            if (!usePhysicsMode)
            {
                return;
            }
            if (AllowedCollision(other) == false)
            {
                return; // Not in the allowed layers
            }
            
            // Check if the object's layer is in the allowed LayerMask
            
            if (ButtonState == FPToolState.Ending)
            {
                ButtonState = FPToolState.Activated;
                whoActivatedMe = other;
                FPButton.MoveToPosition(FPButton.PushedPosition,true);
                //FPButton.Pressed();
                //isPressed = true;
            }
        }
        public virtual void OnTriggerStay(Collider other)
        {
            if (!usePhysicsMode)
            {
                return;
            }

            if (ButtonState==FPToolState.Activated && other == whoActivatedMe)
            {
                //look for distance confirmation
                if (FPButton.CheckDistanceForPressed())
                {
                    //already hit distance confirmation
                    if (ButtonState==FPToolState.ActiveUse)
                    {
                        return;
                    }
                    if (checkFrameCount < Time.frameCount)
                    {
                        checkFrameCount = Time.frameCount + frameCountDelay;
                    }
                    else
                    {
                        return;
                    }
                    ButtonState = FPToolState.ActiveUse;
                    FPButton.Pressed();
                    //isDistanceStaying = true;
                }
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
            //if ButtonState could be activeUse or Activated
            if ((ButtonState == FPToolState.ActiveUse|| ButtonState==FPToolState.Activated)&&other==whoActivatedMe)
            {
                FPButton.MoveToPosition(FPButton.RestPosition,false);
                if (ButtonState == FPToolState.ActiveUse)
                {
                    FPButton.Released();
                }
                ButtonState = FPToolState.Ending;
                whoActivatedMe = null;
                //isPressed = false;
                //isDistanceStaying = false;
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
            ButtonState = FPToolState.ActiveUse;
            //isPressed = true;
            FPButton.MoveToPosition(FPButton.PushedPosition, true);
            FPButton.Pressed();
        }
        public virtual void ManualKeyTriggerReleased()
        {
            ButtonState = FPToolState.Ending;
            //isPressed = false;
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
