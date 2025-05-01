namespace FuzzPhyte.XR
{
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using UnityEngine;
    /// <summary>
    /// Manage Controller data coming in from various XR/VR platforms/SDKs/APIs
    /// This class Tracks Interaction whereas FPXRControllerFeedback shows interactions
    /// Allows direct communication with all of the FPXRController settings
    /// Driven by the FPXRControllerFeedback data files
    /// Should be used to communicate to the internal system
    /// Register a listener with 'SetupItemForListeningAllEvents' and utilize the IFPXRControllerListener interface
    /// The listener is at a controller level and you will be able to parse what you want 
    /// e.g. if you want to listen at a higher level for all general controller logic events to be parsed by the events/functions
    /// tied to the IFPXControllerListener interface
    /// NOTE: Hint visuals are not gated by lock state.
    /// They are used for onboarding/tutorial flows to draw attention to buttons,
    /// even when input is not yet active or mapped.
    /// </summary>
    public class FPXRControllerEventManager : MonoBehaviour, IFPXRControllerSetup<IFPXRControllerListener>
    {
        public static FPXRControllerEventManager Instance { get; private set; }
        [Tooltip("This component is probably already on another object that's not destroyed")]
        public bool DontDestroy = false;
        #region Delegates and Events
        protected List<XRControllerEvent> controllerDelegates = new List<XRControllerEvent>();
        /// <summary>
        /// Controller Level Delegate - based on the hand and the button
        /// </summary>
        /// <param name="hand">Hand</param>
        /// <param name="button">Button</param>
        public delegate void XRControllerEvent(XRHandedness hand, XRButton button);
        private event XRControllerEvent buttonPressed;
        private event XRControllerEvent buttonReleased;
        private event XRControllerEvent buttonLocked;
        private event XRControllerEvent buttonUnlocked;
        public event XRControllerEvent ButtonPressed
        {
            add
            {
                buttonPressed += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                buttonPressed -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent ButtonReleased
        {
            add
            {
                buttonReleased += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                buttonReleased -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent ButtonLocked
        {
            add
            {
                buttonLocked += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                buttonLocked -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent ButtonUnlocked
        {
            add
            {
                buttonUnlocked += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                buttonUnlocked -= value;
                controllerDelegates.Remove(value);
            }
        }

        private event XRControllerEvent controllerLocked;
        private event XRControllerEvent controllerUnlocked;
        public event XRControllerEvent ControllerLocked
        {
            add
            {
                controllerLocked += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                controllerLocked -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent ControllerUnlocked
        {
            add
            {
                controllerUnlocked += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                controllerUnlocked -= value;
                controllerDelegates.Remove(value);
            }
        }

        private event XRControllerEvent hintButtonActive;
        private event XRControllerEvent hintButtonDeactivated;
        public event XRControllerEvent HintButtonActive
        {
            add
            {
                hintButtonActive += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                hintButtonActive -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent HintButtonDeactivated
        {
            add
            {
                hintButtonDeactivated += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                hintButtonDeactivated -= value;
                controllerDelegates.Remove(value);
            }
        }

        private event XRControllerEvent informationButtonActive;
        private event XRControllerEvent informationButtonDeactivated;
        public event XRControllerEvent InformationButtonDeactivated
        {
            add
            {
                informationButtonDeactivated += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                informationButtonDeactivated -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent InformationButtonActive
        {
            add
            {
                informationButtonActive += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                informationButtonActive -= value;
                controllerDelegates.Remove(value);
            }
        }

        private event XRControllerEvent controllerResetLeft;
        private event XRControllerEvent controllerResetRight;
        public event XRControllerEvent ControllerResetLeft
        {
            add
            {
                controllerResetLeft += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                controllerResetLeft -= value;
                controllerDelegates.Remove(value);
            }
        }
        public event XRControllerEvent ControllerResetRight
        {
            add
            {
                controllerResetRight += value;
                controllerDelegates.Add(value);
            }
            remove
            {
                controllerResetRight -= value;
                controllerDelegates.Remove(value);
            }
        }

        //raw events - for internal use only and for our hintAcknowledge listeners
        private event XRControllerEvent rawButtonPressed;
        private event XRControllerEvent rawButtonReleased;
        #endregion
        [Space]
        [Header("Controller Feedback")] 
        [SerializeField] protected FPXRControllerFeedback leftControllerFeedback;
        [SerializeField] protected FPXRControllerFeedback rightControllerFeedback;
        [Space]
        [Header("Controller Setup Options")]
        [Tooltip("If true, this system will not fire off events associated with a locked button")]
        protected bool enforceLockState = false;
        [Tooltip("Will manage our hint acknowledge listeners for cases of explaining controller buttons and being locked out")]
        protected Dictionary<(XRHandedness,XRButton),XRControllerEvent>hintAcknowledgeListeners = new Dictionary<(XRHandedness, XRButton), XRControllerEvent>();
        [Tooltip("Will hold the routine for our current hint/information sequence")]
        protected Coroutine currentSequenceRoutine;
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
#if UNITY_EDITOR
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
        [ContextMenu("Hover check scale")]
        public void SetHoverTextPrimaryButtonTest()
        {
            UpdateButtonState(XRHandedness.Left, XRButton.PrimaryButton, XRInteractionStatus.Hover);
        }
        #endregion
#endif
        #region Public Event Listener Registration
        /// <summary>
        /// Easy way to add all events/listeners to our interface referenced item
        /// </summary>
        /// <param name="listener">The listener</param>
        public void SetupItemForListeningAllEvents(IFPXRControllerListener listener)
        {
            listener.SetupControllerListener(leftControllerFeedback, rightControllerFeedback);
            ButtonPressed += listener.AnyControllerButtonPressed;
            ButtonReleased += listener.AnyControllerButtonReleased;
            ButtonLocked += listener.AnyControllerButtonLocked;
            ButtonUnlocked += listener.AnyControllerButtonUnlocked;
            ControllerLocked += listener.AnyControllerLocked;
            ControllerUnlocked += listener.AnyControllerUnlocked;
            HintButtonActive += listener.AnyControllerHintActive;
            HintButtonDeactivated += listener.AnyControllerHintDeactive;
            InformationButtonActive += listener.AnyControllerInfoActive;
            InformationButtonDeactivated += listener.AnyControllerInfoDeactive;
            ControllerResetLeft += listener.LeftControllerReset;
            ControllerResetRight += listener.RightControllerReset;
        }
        /// <summary>
        /// Easy way to remove all events from the listener interface item passed in
        /// </summary>
        /// <param name="listener">the listener</param>
        public void RemoveItemForListeningAllEvents(IFPXRControllerListener listener)
        {
            ButtonPressed -= listener.AnyControllerButtonPressed;
            ButtonReleased -= listener.AnyControllerButtonReleased;
            ButtonLocked -= listener.AnyControllerButtonLocked;
            ButtonUnlocked -= listener.AnyControllerButtonUnlocked;
            ControllerLocked -= listener.AnyControllerLocked;
            ControllerUnlocked -= listener.AnyControllerUnlocked;
            HintButtonActive -= listener.AnyControllerHintActive;
            HintButtonDeactivated -= listener.AnyControllerHintDeactive;
            InformationButtonActive -= listener.AnyControllerInfoActive;
            InformationButtonDeactivated -= listener.AnyControllerInfoDeactive;
            ControllerResetLeft -= listener.LeftControllerReset;
            ControllerResetRight -= listener.RightControllerReset;
        }
        /// <summary>
        /// Removes all delegates/listeners as we've been monitoring them
        /// </summary>
        public void RemoveAllListeners()
        {
            foreach (var handler in controllerDelegates)
            {
                ButtonPressed -= handler;
                ButtonReleased -= handler;
                ButtonLocked -= handler;
                ButtonUnlocked -= handler;
                ControllerLocked -= handler;
                ControllerUnlocked -= handler;
                HintButtonActive -= handler;
                HintButtonDeactivated -= handler;
                InformationButtonActive -= handler;
                InformationButtonDeactivated -= handler;
                ControllerResetLeft -= handler;
                ControllerResetRight -= handler;
            }
            //clear the list
            controllerDelegates.Clear();
        }
        #endregion
        #region Public Methods & Functions for Button States, Locking, Info/Hints
        /// <summary>
        /// Reset and load both controllers
        /// </summary>
        /// <param name="leftControllerData">left controller data?</param>
        /// <param name="rightControllerData">right controller data?</param>
        public virtual void ResetDataControllers(FPXRControllerFeedbackConfig leftControllerData, FPXRControllerFeedbackConfig rightControllerData)
        {
            if (leftControllerFeedback != null && rightControllerFeedback != null)
            {
                leftControllerFeedback.ResetUpdateControllerData(leftControllerData);
                controllerResetLeft?.Invoke(XRHandedness.Left, XRButton.NA);
                rightControllerFeedback.ResetUpdateControllerData(rightControllerData);
                controllerResetRight?.Invoke(XRHandedness.Right, XRButton.NA);
            }
        }
        /// <summary>
        /// Reset, load both controllers, and run the sequence
        /// </summary>
        /// <param name="leftConfig"></param>
        /// <param name="rightConfig"></param>
        /// <param name="onComplete"></param>
        public virtual void ResetControllersAndRunSequence(FPXRControllerFeedbackConfig leftConfig,FPXRControllerFeedbackConfig rightConfig,Action onComplete = null)
        {
            // Stop any active sequence first
            CancelCurrentAcknowledgmentSequence();

            // Reset feedback data
            ResetDataControllers(leftConfig, rightConfig);

            // Merge sequences from both controllers
            var combinedSteps = new List<XRButtonHintStep>();
            if (leftConfig.AcknowledgeSequence != null)
                combinedSteps.AddRange(leftConfig.AcknowledgeSequence);
            if (rightConfig.AcknowledgeSequence != null)
                combinedSteps.AddRange(rightConfig.AcknowledgeSequence);

            if (combinedSteps.Count == 0)
            {
                Debug.LogWarning("No acknowledgment sequence defined in either controller config.");
                onComplete?.Invoke();
                return;
            }

            RunButtonAcknowledgmentSequence(combinedSteps, onComplete);
        }
        /// <summary>
        /// Lock the controllers, reset them, and run the sequence
        /// </summary>
        /// <param name="leftConfig"></param>
        /// <param name="rightConfig"></param>
        /// <param name="onComplete"></param>
        public virtual void ResetControllersRunLockedTutorialSequence(FPXRControllerFeedbackConfig leftConfig,FPXRControllerFeedbackConfig rightConfig,Action onComplete = null)
        {
            LockController(XRHandedness.Left);
            LockController(XRHandedness.Right);

            ResetControllersAndRunSequence(leftConfig, rightConfig, () =>
            {
                UnlockController(XRHandedness.Left);
                UnlockController(XRHandedness.Right);
                onComplete?.Invoke();
            });
        }
        /// <summary>
        /// Reset and load LEFT Controller
        /// </summary>
        /// <param name="leftControllerData">left controller data?</param>
        public virtual void ResetDataLeftController(FPXRControllerFeedbackConfig leftControllerData)
        {
            if (leftControllerFeedback != null)
            {
                leftControllerFeedback.ResetUpdateControllerData(leftControllerData);
                controllerResetLeft?.Invoke(XRHandedness.Left, XRButton.NA);
            }
        }
        /// <summary>
        /// Reset and load LEFT controller and run the sequence
        /// </summary>
        /// <param name="leftControllerData"></param>
        /// <param name="onComplete"></param>
        public virtual void ResetDataLeftRunSequence(FPXRControllerFeedbackConfig leftControllerData, Action onComplete = null)
        {
            CancelCurrentAcknowledgmentSequence();
            ResetDataLeftController(leftControllerData);

            if (leftControllerData.AcknowledgeSequence == null || leftControllerData.AcknowledgeSequence.Count == 0)
            {
                Debug.LogWarning("No acknowledgment sequence defined in left controller config.");
                onComplete?.Invoke();
                return;
            }

            RunButtonAcknowledgmentSequence(leftControllerData.AcknowledgeSequence, onComplete);
        }
        /// <summary>
        /// Reset and load RIGHT controller
        /// </summary>
        /// <param name="rightControllerData">right controller data?</param>
        public virtual void ResetDataRightController(FPXRControllerFeedbackConfig rightControllerData)
        {
            if(rightControllerFeedback != null)
            {
                rightControllerFeedback.ResetUpdateControllerData(rightControllerData);
                controllerResetRight?.Invoke(XRHandedness.Right, XRButton.NA);
            }
        }
        /// <summary>
        /// Reset and load Right controller and run the sequence
        /// </summary>
        /// <param name="rightControllerData"></param>
        /// <param name="onComplete"></param>
        public virtual void ResetDataRightRunSequence(FPXRControllerFeedbackConfig rightControllerData, Action onComplete = null)
        {
            CancelCurrentAcknowledgmentSequence();
            ResetDataRightController(rightControllerData);

            if (rightControllerData.AcknowledgeSequence == null || rightControllerData.AcknowledgeSequence.Count == 0)
            {
                Debug.LogWarning($"No acknowledgment sequence defined in right controller config.");
                onComplete?.Invoke();
                return;
            }
            RunButtonAcknowledgmentSequence(rightControllerData.AcknowledgeSequence, onComplete);
        }

        /// <summary>
        /// Adds a temporary internal raw listener based on user needs
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="button"></param>
        /// <param name="onAcknowledged"></param>
        /// <param name="autoHide"></param>

        #region Hint Acknowledge Functions
        public virtual void WaitForButtonAcknowledge(XRHandedness hand,XRButton button,Action onAcknowledged,bool autoHide = true)
        {
            // Define the temporary listener
            XRControllerEvent listener = null;
            listener = (eventHand, eventButton) =>
            {
                if (eventHand == hand && eventButton == button)
                {
                    rawButtonPressed -= listener; // Remove listener once triggered
                    hintAcknowledgeListeners.Remove((hand, button));

                    if (autoHide)
                        ShowHideHintControllerButton(hand, button, XRInteractionStatus.Select, false);

                    onAcknowledged?.Invoke();
                }
            };

            // Store and subscribe
            hintAcknowledgeListeners[(hand, button)] = listener;
            rawButtonPressed += listener;
        }
        /// <summary>
        /// Removes the temporary listener for button acknowledge
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="button"></param>
        public virtual void CancelButtonAcknowledgeWait(XRHandedness hand, XRButton button)
        {
            var key = (hand, button);
            if (hintAcknowledgeListeners.TryGetValue(key, out var listener))
            {
                rawButtonPressed -= listener;
                hintAcknowledgeListeners.Remove(key);
            }
        }
        public virtual void RunButtonAcknowledgmentSequence(List<XRButtonHintStep>steps,Action onSequenceComplete)
        {
            if (currentSequenceRoutine != null)
            {
                Debug.LogWarning($"Button Acknowledgment sequence already running. Cancelling the previous one.");
                CancelCurrentAcknowledgmentSequence();
            }
            currentSequenceRoutine = StartCoroutine(HandleAcknowledgmentSequence(steps, onSequenceComplete));
        }
        public virtual void CancelCurrentAcknowledgmentSequence()
        {
            if (currentSequenceRoutine != null)
            {
                StopCoroutine(currentSequenceRoutine);
                currentSequenceRoutine = null;
            }
        }
        protected virtual IEnumerator HandleAcknowledgmentSequence(List<XRButtonHintStep> steps,Action onSequenceComplete)
        {
            foreach (var step in steps)
            {
                bool acknowledged = false;

                // Show the hint
                ShowHideHintControllerButton(step.Hand, step.Button, XRInteractionStatus.Select, true);

                // Set up acknowledgment
                WaitForButtonAcknowledge(step.Hand, step.Button, () =>
                {
                    acknowledged = true;

                    step.OnStepAcknowledged?.Invoke();

                    if (step.AutoHideHint)
                    {
                        ShowHideHintControllerButton(step.Hand, step.Button, XRInteractionStatus.Select, false);
                    }
                });

                // Wait until the player presses the button
                while (!acknowledged)
                    yield return null;
            }

            currentSequenceRoutine = null;
            onSequenceComplete?.Invoke();
        }
        #endregion
        /// <summary>
        /// External SDKs (e.g., Oculus, Unity XR Toolkit) invoke this to report button states.
        /// Make sure to pass Select/Unselect only here anything else will be ignored
        /// </summary>
        public virtual void UpdateButtonState(XRHandedness hand, XRButton button, XRInteractionStatus buttonState, float controllerData=1f)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            feedback?.SetButtonState(button, buttonState, controllerData);

            //fire raw events
            switch (buttonState)
            {
                case XRInteractionStatus.Select:
                    rawButtonPressed?.Invoke(hand, button);
                    break;
                case XRInteractionStatus.Unselect:
                    rawButtonReleased?.Invoke(hand, button);
                    break;
            }
            // locked enforcing and other button states/events
            if(enforceLockState && ReturnControllerLockStatus(hand,button))
            {
                //if we are locked and enforcing: don't do anything
                return;
            }
            if (buttonState == XRInteractionStatus.Locked)
            {
                //Quick reminder
                Debug.LogWarning($"If you're intention was to lock the controller - please call 'LockControllerButton'!");
            }
            
            //drives visuals and audio
            feedback?.SetButtonState(button, buttonState,controllerData);
            switch(buttonState)
            {
                case XRInteractionStatus.Select:
                    buttonPressed?.Invoke(hand, button);
                    break;
                case XRInteractionStatus.Unselect:
                    buttonReleased?.Invoke(hand, button);
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
                buttonLocked?.Invoke(hand, button);
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
                buttonUnlocked?.Invoke(hand, button);
            }
        }
        /// <summary>
        /// External request to lock Singular Controller by hand
        /// </summary>
        /// <param name="hand">The hand?</param>
        public virtual void LockController(XRHandedness hand)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null)
            {
                feedback?.LockAllButtons();
                controllerLocked?.Invoke(hand, XRButton.All);
            }
        }
        /// <summary>
        /// External request to Unlock Singular Controller by hand
        /// </summary>
        /// <param name="hand">The hand?</param>
        public virtual void UnlockController(XRHandedness hand)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            if (feedback != null)
            {
                feedback?.UnlockAllButtons();
                controllerUnlocked?.Invoke(hand, XRButton.All);
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
                    hintButtonActive?.Invoke(hand, button);
                }
                else
                {
                    hintButtonDeactivated?.Invoke(hand, button);
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
                    informationButtonActive?.Invoke(hand, button);
                }
                else
                {
                    informationButtonDeactivated?.Invoke(hand, button);
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
        /// Function to return the button lock status by hand in one call
        /// </summary>
        /// <param name="hand">Controller Hand In Question</param>
        /// <param name="button">Button in question</param>
        /// <returns></returns>
        public virtual bool ReturnControllerLockStatus(XRHandedness hand, XRButton button)
        {
            FPXRControllerFeedback feedback = GetFeedbackForHand(hand);
            return feedback !=null && feedback.ReturnButtonLockState(button);
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
        #endregion
        protected virtual FPXRControllerFeedback GetFeedbackForHand(XRHandedness hand)
        {
            return hand == XRHandedness.Left ? leftControllerFeedback : rightControllerFeedback;
        }
    }
}
