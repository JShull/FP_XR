namespace FuzzPhyte.XR
{
    using UnityEngine;
    public class FPXRControllerEventManager : MonoBehaviour
    {
        public static FPXRControllerEventManager Instance { get; private set; }
        

        public delegate void XRControllerEvent(XRHandedness hand, XRButton button);
        public event XRControllerEvent ButtonPressed;
        public event XRControllerEvent ButtonReleased;
        public event XRControllerEvent ButtonLocked;
        public event XRControllerEvent ButtonUnlocked;
        public event XRControllerEvent ControllerLocked;
        public event XRControllerEvent ControllerUnlocked;
        //
        //protected Dictionary<(XRHandedness, XRButton), bool> buttonStates = new Dictionary<(XRHandedness, XRButton), bool>();
        //protected HashSet<XRButton> lockedButtons = new HashSet<XRButton>();
        [Space]
        [Header("Controller Feedback")] 
        [SerializeField] protected FPXRControllerFeedback leftControllerFeedback;
        [SerializeField] protected FPXRControllerFeedback rightControllerFeedback;
        protected virtual void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        #region Testing
        [ContextMenu("Testing ControllerManager, Left Hand, Primary Button, Select")]
        public void LeftControllerPrimaryButtonSelectAction()
        {
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Primary Button, Unselect")]
        public void LeftControllerPrimaryButtonUnselectAction()
        {
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Unselect);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Primary Button, Locked")]
        public void LockControllerPrimaryButton()
        {
            LockControllerButton(XRHandedness.Left, XRButton.PrimaryButton);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Primary Button, Unlocked")]
        public void UnlockControllerPrimaryButton()
        {
            UnlockControllerButton(XRHandedness.Left, XRButton.PrimaryButton);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, All Locked")]
        public void LockLeftController()
        {
            LockController(XRHandedness.Left);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, All Unlocked")]
        public void UnlockLeftController()
        {
            UnlockController(XRHandedness.Left);
        }
        #endregion
        /// <summary>
        /// External SDKs (e.g., Oculus, Unity XR Toolkit) invoke this to report button states.
        /// Make sure to pass Select/Unselect only here anything else will be ignored
        /// </summary>
        public virtual void UpdateButtonState(XRHandedness hand, XRButton button, XRInteractionStatus buttonState)
        {
            if(buttonState== XRInteractionStatus.Locked)
            {
                //John = quick reminder
                Debug.LogWarning($"You should be locking and calling the Lock Controller Button Function!");
            }
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            //drives visuals and audio
            feedback?.SetButtonState(button, buttonState);
            switch(buttonState)
            {
                case XRInteractionStatus.Select:
                    ButtonPressed?.Invoke(hand, button);
                    break;
                case XRInteractionStatus.Unselect:
                    ButtonReleased?.Invoke(hand, button);
                    break;
            }
        }
        /// <summary>
        /// External Request for locking a controller button
        /// </summary>
        /// <param name="hand">the controller hand</param>
        /// <param name="button">button to lock</param>
        public virtual void LockControllerButton(XRHandedness hand, XRButton button)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            //drive lock
            if (feedback != null) 
            {
                feedback?.LockControllerButton(button);
                feedback?.SetButtonState(button,XRInteractionStatus.Select);
                ButtonLocked?.Invoke(hand, button);
            }
        }
        /// <summary>
        /// External Request for unlocking a controller button
        /// </summary>
        /// <param name="hand">the controller hand</param>
        /// <param name="button">button to unlock</param>
        public virtual void UnlockControllerButton(XRHandedness hand, XRButton button)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            //drive lock
            if (feedback != null)
            {
                feedback?.UnlockControllerButton(button);
                ButtonUnlocked?.Invoke(hand, button);
            }
        }
        public virtual void LockController(XRHandedness hand)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null)
            {
                feedback?.LockAllButtons();
                ControllerLocked?.Invoke(hand, XRButton.NA);
            }
        }
        public virtual void UnlockController(XRHandedness hand)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null)
            {
                feedback?.UnlockAllButtons();
                ControllerUnlocked?.Invoke(hand, XRButton.NA);
            }
        }
        /// <summary>
        /// Will return if the entire controller is locked
        /// </summary>
        /// <param name="hand">Controller hand</param>
        /// <returns></returns>
        public bool ReturnControllerLockStatus(XRHandedness hand)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            return feedback.ControllerLocked;
        }
        /// <summary>
        /// Return left controller button locked state
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool ReturnLeftControllerButtonLockStatus(XRButton button)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(XRHandedness.Left);
            return feedback.ReturnButtonLockState(button);
        }
        /// <summary>
        /// Return right controller button locked state
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool ReturnRightControllerButtonLockStatus(XRButton button)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(XRHandedness.Right);
            return feedback.ReturnButtonLockState(button);
        }
        protected virtual FPXRControllerFeedback GetFeedbackForHand(XRHandedness hand)
        {
            return hand == XRHandedness.Left ? leftControllerFeedback : rightControllerFeedback;
        }
        
    }
}
