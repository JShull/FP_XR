namespace FuzzPhyte.XR
{
    using UnityEngine;
    public class FPXRControllerEventManager : MonoBehaviour
    {
        public static FPXRControllerEventManager Instance { get; private set; }
        [Tooltip("This component is probably already on another object that's not destroyed")]
        public bool DontDestroy = false;

        public delegate void XRControllerEvent(XRHandedness hand, XRButton button);
        public event XRControllerEvent ButtonPressed;
        public event XRControllerEvent ButtonReleased;
        public event XRControllerEvent ButtonLocked;
        public event XRControllerEvent ButtonUnlocked;
        public event XRControllerEvent ControllerLocked;
        public event XRControllerEvent ControllerUnlocked;
        public event XRControllerEvent HintButtonActive;
        public event XRControllerEvent InformationButtonActive;
        public event XRControllerEvent HintButtonDeactivated;
        public event XRControllerEvent InformationButtonDeactivated;
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
            if (DontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
            
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
        [ContextMenu("Testing ControllerManager, Left Hand, Select Primary, Hint: ON")]
        public void HintButtonActivePrimary()
        {
            //activate Hint
            ShowHideHintControllerButton(XRHandedness.Left, XRButton.PrimaryButton,XRInteractionStatus.Select, true);
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Select Primary, Hint: OFF")]
        public void HintButtonDeactivePrimary()
        {
            //activate Hint
            ShowHideHintControllerButton(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select, false);
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Select Primary, Information: ON")]
        public void InformationButtonActivePrimary()
        {
            ShowHideInformationControllerButton(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select, true);
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Select Primary, Information: OFF")]
        public void InformationButtonDeactivePrimary()
        {
            ShowHideInformationControllerButton(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select, false);
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Select Secondary, Information: ON")]
        public void InformationButtonActiveSecondary()
        {
            ShowHideInformationControllerButton(XRHandedness.Left, XRButton.SecondaryButton, XRInteractionStatus.Select, true);
            UpdateButtonState(XRHandedness.Left, XRButton.SecondaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing ControllerManager, Left Hand, Select Secondary, Information: OFF")]
        public void InformationButtonDeactiveSecondary()
        {
            ShowHideInformationControllerButton(XRHandedness.Left, XRButton.SecondaryButton, XRInteractionStatus.Select, false);
            UpdateButtonState(XRHandedness.Left, XRButton.SecondaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Set Primary Button Render Above Secondary")]
        public void SetRenderTextIconOrderPrimaryOverSecondary()
        {
            SetTextRenderOrder(XRHandedness.Left, XRButton.PrimaryButton, 10);
            SetPrimaryIconRenderOrder(XRHandedness.Left, XRButton.PrimaryButton, 9);
            SetSecondaryIconRenderOrder(XRHandedness.Left, XRButton.PrimaryButton, 8);
            SetTextRenderOrder(XRHandedness.Left, XRButton.SecondaryButton, 7);
            SetPrimaryIconRenderOrder(XRHandedness.Left, XRButton.SecondaryButton, 6);
            SetSecondaryIconRenderOrder(XRHandedness.Left, XRButton.SecondaryButton, 5);
            InformationButtonActivePrimary();
            InformationButtonActiveSecondary();
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
            //drive unlock
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
        /// Method to deactivate/activate hint
        /// </summary>
        /// <param name="hand">Current hand?</param>
        /// <param name="button">Button?</param>
        /// <param name="hintActive">Hint On?</param>
        public virtual void ShowHideHintControllerButton(XRHandedness hand, XRButton button, XRInteractionStatus buttonState,bool hintActive)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            //drive hint
            if (feedback != null)
            {
                feedback?.HintControllerButton(button, buttonState,hintActive);
                if (hintActive)
                {
                    HintButtonActive?.Invoke(hand, button);
                }
                else
                {
                    HintButtonDeactivated?.Invoke(hand, button);
                }
            }
        }
        /// <summary>
        /// Method to deactivate/activate information
        /// </summary>
        /// <param name="hand">Current hand?</param>
        /// <param name="button">Button?</param>
        /// <param name="informationActive">Information on?</param>
        public virtual void ShowHideInformationControllerButton(XRHandedness hand, XRButton button, XRInteractionStatus buttonState,bool informationActive)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            //drive hint
            if (feedback != null)
            {
                feedback?.InformationControllerButton(button, buttonState,informationActive);
                if (informationActive)
                {
                    InformationButtonActive?.Invoke(hand, button);
                }
                else
                {
                    InformationButtonDeactivated?.Invoke(hand, button);
                }
            }
        }
        /// <summary>
        /// Sets Render Order of our Text
        /// </summary>
        /// <param name="hand">Hand?</param>
        /// <param name="button">Button?</param>
        /// <param name="renderOrder">Render Order?</param>
        public virtual void SetTextRenderOrder(XRHandedness hand, XRButton button, int renderOrder)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null) 
            { 
                feedback.SetButtonTextRenderOrder(button, renderOrder);
            }
        }
        /// <summary>
        /// Sets Render Order of our Primary Icon
        /// </summary>
        /// <param name="hand">Hand?</param>
        /// <param name="button">Button?</param>
        /// <param name="renderOrder">Render order for icon?</param>
        public virtual void SetPrimaryIconRenderOrder(XRHandedness hand, XRButton button, int renderOrder)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null)
            {
                feedback.SetButtonIconRenderOrder(button, renderOrder);
            }
        }
        /// <summary>
        /// Sets Render Order of our Secondary Icon
        /// </summary>
        /// <param name="hand">Hand?</param>
        /// <param name="button">Button?</param>
        /// <param name="renderOrder">Render order for secondary icon?</param>
        public virtual void SetSecondaryIconRenderOrder(XRHandedness hand, XRButton button, int renderOrder)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null)
            {
                feedback.SetButtonSecondaryIconRenderOrder(button, renderOrder);
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
