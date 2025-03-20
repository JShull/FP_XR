namespace FuzzPhyte.XR
{
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Collections.Generic;


    public class FPXRControllerEventManager : MonoBehaviour
    {
        public static FPXRControllerEventManager Instance { get; private set; }
        

        public delegate void XRControllerEvent(XRHandedness hand, XRButton button);
        public event XRControllerEvent ButtonPressed;
        public event XRControllerEvent ButtonReleased;
        //
        //protected Dictionary<(XRHandedness, XRButton), bool> buttonStates = new Dictionary<(XRHandedness, XRButton), bool>();
        protected HashSet<XRButton> lockedButtons = new HashSet<XRButton>();
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
        public virtual void SetButtonLock(XRButton button, bool lockButton)
        {
            if (lockButton)
            {
                lockedButtons.Add(button);
            }
            else
            {
                lockedButtons.Remove(button);
            }
        }
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

        /// <summary>
        /// External SDKs (e.g., Oculus, Unity XR Toolkit) invoke this to report button states.
        /// </summary>
        public virtual void UpdateButtonState(XRHandedness hand, XRButton button, XRInteractionStatus buttonState)
        {
            if (lockedButtons.Contains(button))
                return;

            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            //drives visuals and audio
            feedback?.SetButtonActive(button, buttonState);
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
        protected virtual FPXRControllerFeedback GetFeedbackForHand(XRHandedness hand)
        {
            return hand == XRHandedness.Left ? leftControllerFeedback : rightControllerFeedback;
        }
    }
}
