namespace FuzzPhyte.XR
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    /// <summary>
    /// Manage Button data UI/UX data associated with a controller
    /// Allows direct communication the UI/UX FPXRControllerRef script that is associated with each button
    /// Driven by the FPXRControllerFeedback data files
    /// If you want to register a listener with 'SetupItemForListeningAllEvents' and utilize the IFPXButtonListener interface
    /// This listener is ideal for specific button actions associated with specific left/right controller knowledge
    /// e.g. you want to listen to when the right controller primary button is down/up
    /// </summary>
    public class FPXRControllerFeedback : MonoBehaviour, IFPXRControllerSetup<IFPXRButtonListener>
    {
        [Header("Config Data")]
        [SerializeField] protected FPXRControllerFeedbackConfig feedbackConfig;
        
        [Space]
        [Header("UI References")]
        public bool UseCanvas=false;
        public GameObject ControllerButtonPrefab;
        public GameObject GripButtonPrefab;
        public GameObject TriggerButtonPrefab;
        public AudioSource WorldControllerAudioSource;
        public bool SetupOnAwake = true;
        #region Delegate and Events
        public delegate void FPXRButtonEvents(XRButton button, XRInteractionStatus buttonState);
        private event FPXRButtonEvents primeButtonDown;
        private event FPXRButtonEvents primeButtonUp;
        private event FPXRButtonEvents primeButtonLocked;
        private event FPXRButtonEvents primeButtonUnlocked;
        public event FPXRButtonEvents PrimaryButtonDown 
        {
            add 
            { 
                primeButtonDown += value;
                buttonDelegates.Add(value);
            }
            remove {
                primeButtonDown -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents PrimaryButtonUp
        {
            add 
            {
                primeButtonUp += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                primeButtonUp -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents PrimaryButtonLocked
        {
            add 
            { 
                primeButtonLocked += value;
                buttonDelegates.Add(value);
            }
            remove 
            { 
                primeButtonLocked -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents PrimaryButtonUnlocked
        {
            add
            {
                primeButtonUnlocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                primeButtonUnlocked -= value;
                buttonDelegates.Remove(value);
            }
        }
        
        private event FPXRButtonEvents secondButtonDown;
        private event FPXRButtonEvents secondButtonUp;
        private event FPXRButtonEvents secondButtonLocked;
        private event FPXRButtonEvents secondButtonUnlocked;
        public event FPXRButtonEvents SecondaryButtonDown
        {
            add
            {
                secondButtonDown += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                secondButtonDown -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents SecondaryButtonUp
        {
            add
            {
                    secondButtonUp += value;
                    buttonDelegates.Add(value);
            }
            remove
            {
                    secondButtonUp -= value;
                    buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents SecondaryButtonLocked
        {
            add
            {
                secondButtonLocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                secondButtonLocked -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents SecondaryButtonUnlocked
        {
            add
            {
                secondButtonUnlocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                secondButtonUnlocked -= value;
                buttonDelegates.Remove(value);
            }
        }

        private event FPXRButtonEvents triggerButtonDown;
        private event FPXRButtonEvents triggerButtonUp;
        private event FPXRButtonEvents triggerButtonLocked;
        private event FPXRButtonEvents triggerButtonUnlocked;
        public event FPXRButtonEvents TriggerButtonDown 
        {
            add
            {
                triggerButtonDown += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                triggerButtonDown -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents TriggerButtonUp
        {
            add
            {
                triggerButtonUp += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                triggerButtonUp -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents TriggerButtonLocked
        {
            add
            {
                triggerButtonLocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                triggerButtonLocked -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents TriggerButtonUnlocked
        {
            add
            {
                triggerButtonUnlocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                triggerButtonUnlocked -= value;
                buttonDelegates.Remove(value);
            }
        }

        private event FPXRButtonEvents gripButtonDown;
        private event FPXRButtonEvents gripButtonUp;
        private event FPXRButtonEvents gripButtonLocked;
        private event FPXRButtonEvents gripButtonUnlocked;
        public event FPXRButtonEvents GripButtonDown
        {
            add
            {
                gripButtonDown += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                gripButtonDown -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents GripButtonUp
        {
            add
            {
                gripButtonUp += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                gripButtonUp -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents GripButtonLocked
        {
            add
            {
                gripButtonLocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                gripButtonLocked -= value;
                buttonDelegates.Remove(value);
            }
        }
        public event FPXRButtonEvents GripButtonUnlocked
        {
            add
            {
                gripButtonUnlocked += value;
                buttonDelegates.Add(value);
            }
            remove
            {
                gripButtonUnlocked -= value;
                buttonDelegates.Remove(value);
            }
        }
        #endregion
        #region Cached Data Parameters
        protected Dictionary<XRButton,ButtonFeedback> spawnedData = new Dictionary<XRButton, ButtonFeedback>();
        protected Dictionary<XRButton, FPXRControllerRef> spawnedItems = new Dictionary<XRButton, FPXRControllerRef>();
        protected Dictionary<XRButton, bool> controllerButtonsLocked = new Dictionary<XRButton, bool>();
        protected Dictionary<XRButton, bool> controllerButtonsHint = new Dictionary<XRButton, bool>();
        protected Dictionary<XRButton,bool> controllerButtonsInformation = new Dictionary<XRButton, bool>();
        protected bool fullControllerLocked = false;
        [Tooltip("Temp fix/cache internal recognize a 'lock' and ignore other commands")]
        protected ButtonLabelState lockStateDetails;
        #endregion
        protected List<FPXRButtonEvents> buttonDelegates = new List<FPXRButtonEvents>();
        public bool ControllerLocked { get { return fullControllerLocked; } }
        [Tooltip("World Location Ref by Type")]
        public List<XRWorldButton> XRWorldButtons = new List<XRWorldButton>();

        public virtual void Awake()
        {
            //controllerHandedness = feedbackConfig.ControllerStartHandedness;
            if (SetupOnAwake && feedbackConfig!=null)
            {
                SpawnControllerItems();
            }
        }
        
        protected virtual void ResetController()
        {
            //need to loop through all spawned items and blast those gameobjects
            
            var controllRefKeys = spawnedItems.Keys.ToList();
            for (int i = 0; i < controllRefKeys.Count; i++)
            {
                var anItem = spawnedItems[controllRefKeys[i]];
                Destroy(anItem.gameObject);
            }
            //need to then clear data caches
            spawnedData.Clear();
            spawnedItems.Clear();
            controllerButtonsLocked.Clear();
            controllerButtonsHint.Clear();
            controllerButtonsInformation.Clear();
            fullControllerLocked = false;
        }
        /// <summary>
        /// Spawns our prefabs and populates all items based on data
        /// </summary>
        protected virtual void SpawnControllerItems()
        {
            //build out prefab based on feedbackConfig
            //align to world items based on XRWorldButtons
            //look through world references first
            for (int j = 0; j < XRWorldButtons.Count; j++)
            {
                var curButton = XRWorldButtons[j];
                //match based on feedback
                var matchButton = curButton.ButtonType;
                for (int i = 0; i < feedbackConfig.feedbacks.Count; i++)
                {
                    var curFeedback = feedbackConfig.feedbacks[i];
                    if (curFeedback.Button == matchButton)
                    {
                        // we have a match
                        // spawn prefab, assign to correct parent
                        // check our button type
                        GameObject spawnedButtonRef = null;
                        switch (matchButton)
                        {
                            case XRButton.Grip:
                                spawnedButtonRef = GameObject.Instantiate(GripButtonPrefab, curButton.ButtonLocation);
                                break;
                            case XRButton.Trigger:
                                spawnedButtonRef = GameObject.Instantiate(TriggerButtonPrefab, curButton.ButtonLocation);
                                break;
                            case XRButton.PrimaryButton:
                            case XRButton.SecondaryButton:
                                spawnedButtonRef = GameObject.Instantiate(ControllerButtonPrefab, curButton.ButtonLocation);
                                break;
                        }
                        var controllerRefData = spawnedButtonRef.GetComponent<FPXRControllerRef>();
                        spawnedButtonRef.transform.localPosition = Vector3.zero;
                        spawnedButtonRef.transform.localRotation = Quaternion.identity;
                        //update based on data using None/NA state
                        var listOfStates = curFeedback.ButtonInteractionStates;
                        //match XRState from any of the listOfStates to XRInteractionStatus.none
                        for(int x = 0; x < listOfStates.Count; x++)
                        {
                            var aState = listOfStates[x];
                            if(aState.XRState == XRInteractionStatus.None)
                            {
                                //update UI to this
                                if (controllerRefData != null)
                                {
                                    controllerRefData.SetupUI(curButton.ScaleAdjustment,aState.Icon, aState.LabelText, aState.LabelFontSetting, aState.ButtonSound, WorldControllerAudioSource);
                                    //turn off secondary icon if there is one
                                    controllerRefData.ShowORHideSecondaryVisual(false);
                                }
                            }
                            if(aState.XRState == XRInteractionStatus.Locked)
                            {
                                lockStateDetails = aState;
                            }
                        }
                        if(controllerRefData != null)
                        {
                            //setup dictionaries
                            spawnedData.Add(matchButton, curFeedback);
                            spawnedItems.Add(matchButton, controllerRefData);
                            controllerButtonsLocked.Add(matchButton, false);
                            controllerButtonsHint.Add(matchButton, false);
                            controllerButtonsInformation.Add(matchButton, false);
                        }
                        break;
                    }
                }
            }

        }

        #region Public Accessors for events
        /// <summary>
        /// This will register the interface/listener for all of the events on this feedback
        /// </summary>
        /// <param name="theListener"></param>
        public virtual void SetupItemForListeningAllEvents(IFPXRButtonListener theListener)
        {
            theListener.SetupButtonListener(this);
            PrimaryButtonDown += theListener.PrimaryButtonDown;
            PrimaryButtonUp += theListener.PrimaryButtonUp;
            PrimaryButtonLocked += theListener.PrimaryButtonUnlocked;
            PrimaryButtonUnlocked += theListener.PrimaryButtonUnlocked;

            SecondaryButtonDown += theListener.SecondaryButtonDown;
            SecondaryButtonUp += theListener.SecondaryButtonUp;
            SecondaryButtonLocked += theListener.SecondaryButtonLocked;
            SecondaryButtonUnlocked += theListener.SecondaryButtonUnlocked;

            TriggerButtonDown += theListener.TriggerButtonDown;
            TriggerButtonUp += theListener.TriggerButtonUp;
            TriggerButtonLocked += theListener.TriggerButtonLocked;
            TriggerButtonUnlocked += theListener.TriggerButtonUnlocked;

            GripButtonDown += theListener.GripButtonDown;
            GripButtonUp += theListener.GripButtonUp;
            GripButtonLocked += theListener.GripButtonLocked;
            GripButtonUnlocked += theListener.GripButtonUnlocked;
            
        }
        public virtual void RemoveItemForListeningAllEvents(IFPXRButtonListener theListener)
        {
            PrimaryButtonDown -= theListener.PrimaryButtonDown;
            PrimaryButtonUp -= theListener.PrimaryButtonUp;
            PrimaryButtonLocked -= theListener.PrimaryButtonUnlocked;
            PrimaryButtonUnlocked -= theListener.PrimaryButtonUnlocked;

            SecondaryButtonDown -= theListener.SecondaryButtonDown;
            SecondaryButtonUp -= theListener.SecondaryButtonUp;
            SecondaryButtonLocked -= theListener.SecondaryButtonLocked;
            SecondaryButtonUnlocked -= theListener.SecondaryButtonUnlocked;

            TriggerButtonDown -= theListener.TriggerButtonDown;
            TriggerButtonUp -= theListener.TriggerButtonUp;
            TriggerButtonLocked -= theListener.TriggerButtonLocked;
            TriggerButtonUnlocked -= theListener.TriggerButtonUnlocked;

            GripButtonDown -= theListener.GripButtonDown;
            GripButtonUp -= theListener.GripButtonUp;
            GripButtonLocked -= theListener.GripButtonLocked;
            GripButtonUnlocked -= theListener.GripButtonUnlocked;
        }
        public void RemoveAllListeners()
        {
            foreach(var handler in buttonDelegates)
            {
                primeButtonDown -= handler;
                primeButtonUp -= handler;
                primeButtonLocked -= handler;
                primeButtonUnlocked -= handler;
                secondButtonDown -= handler;
                secondButtonUp -= handler;
                secondButtonLocked -= handler;
                secondButtonUnlocked -= handler;
                gripButtonDown -= handler;
                gripButtonUp -= handler;
                gripButtonLocked -= handler;
                gripButtonUnlocked += handler;
                triggerButtonDown -= handler;
                triggerButtonUp -= handler;
                triggerButtonLocked -= handler;
                triggerButtonUnlocked -= handler;
            }
            //clear the list
            buttonDelegates.Clear();
        }
        #endregion
        #region Public Methods for Data and Buttons
        /// <summary>
        /// Reset the controller with new data
        /// </summary>
        /// <param name="newData"></param>
        public virtual void ResetUpdateControllerData(FPXRControllerFeedbackConfig newData)
        {
            ResetController();
            feedbackConfig = newData;
            SpawnControllerItems();
        }
        public bool ReturnButtonLockState(XRButton button)
        {
            if (controllerButtonsLocked.ContainsKey(button))
            {
                return controllerButtonsLocked[button];
            }
            return false;
        }
        /// <summary>
        /// Will lock all buttons in the dictionary
        /// </summary>
        public void LockAllButtons()
        {
            var allKeys = controllerButtonsLocked.Keys.ToList();
            for(int i = 0; i < allKeys.Count; i++)
            {
                var aKey = allKeys[i];
                LockControllerButton(aKey);
                SetButtonState(aKey,XRInteractionStatus.Locked);
            }
            fullControllerLocked = true;
        }
        /// <summary>
        /// will unlock all buttons in the dictionary
        /// </summary>
        public void UnlockAllButtons()
        {
            var allKeys = controllerButtonsLocked.Keys.ToList();
            for (int i = 0; i < allKeys.Count; i++)
            {
                var aKey = allKeys[i];
                UnlockControllerButton(aKey);
                SetButtonState(aKey, XRInteractionStatus.None);
            }
            fullControllerLocked = false;
        }
        /// <summary>
        /// Will lock a specific button
        /// </summary>
        /// <param name="button">Button to lock</param>
        public void LockControllerButton(XRButton button)
        {
            if (controllerButtonsLocked.ContainsKey(button)) 
            { 
                controllerButtonsLocked[button] = true;
                SetButtonState(button, XRInteractionStatus.Locked);
                ActivateEventsByButtonByState(button, XRInteractionStatus.Locked);
            }
        }
        /// <summary>
        /// Will unlock a specific button
        /// </summary>
        /// <param name="button"></param>
        public void UnlockControllerButton(XRButton button)
        {
            if (controllerButtonsLocked.ContainsKey(button))
            {
                controllerButtonsLocked[button] = false;
                SetButtonState(button, XRInteractionStatus.None);
                ActivateEventsByButtonByState(button, XRInteractionStatus.None);
            }
        }
        /// <summary>
        /// Activate/deactivate Additional Hint Icon
        /// </summary>
        /// <param name="button">Button to hint?</param>
        /// <param name="activateHint">ON?</param>
        public void HintControllerButton(XRButton button, XRInteractionStatus state,bool activateHint)
        {
            if (spawnedData.ContainsKey(button))
            {
                var dataReturnTuple = LookUpByButtonState(button, state);
                if (dataReturnTuple.Item3)
                {
                    var label = dataReturnTuple.Item2;
                    var cachedItem = dataReturnTuple.Item1;
                    //add to hint
                    SetHintModeActive(button, activateHint);
                    //reup UI
                    cachedItem.ApplyUISecondaryVisual(label.HintIcon);
                    cachedItem.ShowORHideSecondaryVisual(activateHint);
                }
                else
                {
                    Debug.LogError($"No Hint state for {button}");
                }
            }
            /*
            FPXRControllerRef cachedItem = spawnedItems[button];
            ButtonFeedback cachedFeedback = spawnedData[button];
            ButtonLabelState? cachedLabelDetails = cachedFeedback.ButtonInteractionStates.Find(l => l.XRState == XRInteractionStatus.Hint);
            if (cachedLabelDetails.HasValue)
            {
                var label = cachedLabelDetails.Value;
                if(label.XRState == XRInteractionStatus.Hint)
                {
                    //add to hint
                    SetHintModeActive(button, activateHint);
                    //reup UI
                    cachedItem.ApplyUISecondaryVisual(label.HintIcon);
                    cachedItem.ShowORHideSecondaryVisual(activateHint);
                }
            }
            */
        }
        /// <summary>
        /// Activate/deactivate Additional Information Icons
        /// </summary>
        /// <param name="button">Button for Information?</param>
        /// <param name="activateInformation">ON?</param>
        public void InformationControllerButton(XRButton button, XRInteractionStatus state,bool activateInformation)
        {
            if (spawnedData.ContainsKey(button))
            {
                var dataReturnTuple = LookUpByButtonState(button, state);
                if (dataReturnTuple.Item3)
                {
                    var label = dataReturnTuple.Item2;
                    var cachedItem = dataReturnTuple.Item1;
                    //add to hint
                    SetInformationModeActive(button, activateInformation);
                    //reup UI
                    cachedItem.ApplyUISecondaryVisual(label.InformationIcon);
                    cachedItem.ShowORHideSecondaryVisual(activateInformation);
                }
                else
                {
                    Debug.LogError($"No Information state for {button}");
                }
            }
        }
        
        /// <summary>
        /// primary public function to set button state
        /// called via the EventManager
        /// </summary>
        /// <param name="button"></param>
        /// <param name="currentButtonStatus"></param>
        public void SetButtonState(XRButton button, XRInteractionStatus currentButtonStatus,float vectorValue=1f)
        {
            //if state is locked we need to set it and then not visually update anything until unlocked, keep firing locked events 
            //this finds feedback based on original data files
            //we want to transfer this over to my cached data

            if (spawnedData.ContainsKey(button)) 
            {
                var dataReturnTuple = LookUpByButtonState(button, currentButtonStatus);
                if (dataReturnTuple.Item3)
                {
                    var label = dataReturnTuple.Item2;
                    var cachedItem = dataReturnTuple.Item1;
                    bool lockState = controllerButtonsLocked[button];
                    //confirm direct label match
                    if (label.XRState == currentButtonStatus)
                    {
                        //check locked state?
                        if (lockState)
                        {
                            //go to lock
                            ButtonStateLock(cachedItem);
                        }
                        else
                        {
                            
                            bool useDownOffset = false;
                            switch (currentButtonStatus)
                            {
                                case XRInteractionStatus.Select:
                                    useDownOffset = true;
                                    
                                    break;
                            }
                            if (button == XRButton.Trigger)
                            {
                                cachedItem.ApplyUIChanges(label.Icon, label.LabelText, label.LabelFontSetting, label.ButtonSound, UseCanvas, useDownOffset, vectorValue);
                            }
                            else
                            {
                                cachedItem.ApplyUIChanges(label.Icon, label.LabelText, label.LabelFontSetting, label.ButtonSound, UseCanvas, useDownOffset);
                            }
                                

                            /// these conditions manage additional state via lock/hint/information
                            /// this is a catch area as we should be setting these states by function references above
                            /// In cases where we pass this into the SetButtonState we still want the action to be invoked
                            if (currentButtonStatus == XRInteractionStatus.Locked)
                            {
                                //lock us up
                                LockControllerButton(button);
                            }
                            /// activate state
                            ActivateEventsByButtonByState(button, currentButtonStatus);
                            if (currentButtonStatus == XRInteractionStatus.Hint)
                            {
                                //add to hint
                                SetHintModeActive(button, true);
                                //reup UI
                                cachedItem.ApplyUISecondaryVisual(label.HintIcon);
                                cachedItem.ShowORHideSecondaryVisual(true);

                            }
                            if (currentButtonStatus == XRInteractionStatus.Information)
                            {
                                //add to information
                                SetInformationModeActive(button, true);
                                cachedItem.ApplyUISecondaryVisual(label.InformationIcon);
                                cachedItem.ShowORHideSecondaryVisual(true);
                                //reup UI
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"Missing the state or didn't find a match, looking for {currentButtonStatus}!");
                    }
                }
                /*
                ButtonFeedback cachedFeedback = spawnedData[button];
                //world ref item
                FPXRControllerRef cachedItem = spawnedItems[button];
                ButtonLabelState? cachedLabelDetails = cachedFeedback.ButtonInteractionStates.Find(l => l.XRState == currentButtonStatus);
                
                if (cachedLabelDetails.HasValue)
                {
                    var label = cachedLabelDetails.Value;
                    bool lockState = controllerButtonsLocked[button];
                    //confirm direct label match
                    if (label.XRState == currentButtonStatus) 
                    {
                        //check locked state?
                        if (lockState)
                        {
                            //go to lock
                            ButtonStateLock(cachedItem);
                        }
                        else
                        {
                            bool useDownOffset = false;
                            switch (currentButtonStatus)
                            {
                                case XRInteractionStatus.Select:
                                    useDownOffset = true;
                                    break;
                            }
                            cachedItem.ApplyUIChanges(label.Icon, label.LabelText, label.LabelFontSetting, label.ButtonSound, UseCanvas, useDownOffset);
                            if (currentButtonStatus == XRInteractionStatus.Locked)
                            {
                                //lock us up
                                LockControllerButton(button);
                            }
                            if(currentButtonStatus == XRInteractionStatus.Hint)
                            {
                                //add to hint
                                SetHintModeActive(button,true);
                                //reup UI
                                cachedItem.ApplyUISecondaryVisual(label.HintIcon);
                                cachedItem.ShowORHideSecondaryVisual(true);
                                    
                            }
                            if(currentButtonStatus == XRInteractionStatus.Information)
                            {
                                //add to information
                                SetInformationModeActive(button,true);
                                cachedItem.ApplyUISecondaryVisual(label.InformationIcon);
                                cachedItem.ShowORHideSecondaryVisual(true);
                                //reup UI
                            }
                        }
                    }
                    else 
                    { 
                        Debug.LogError($"Missing the state or didn't find a match, looking for {currentButtonStatus}!");
                    }
                }
                */
            }
        }
        /// <summary>
        /// Sets the text render order in sort layer returns true if it worked
        /// </summary>
        /// <param name="button">Button to modify?</param>
        /// <param name="renderOrderNum">Order position?</param>
        /// <returns></returns>
        public bool SetButtonTextRenderOrder(XRButton button, int renderOrderNum)
        {
            if (spawnedData.ContainsKey(button))
            {
                FPXRControllerRef cachedItem = spawnedItems[button];
                cachedItem.ChangeTextDrawOrder(renderOrderNum);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sets the visual primary icon order in sort layer, returns true if it worked
        /// </summary>
        /// <param name="button">Button Icon to modify?</param>
        /// <param name="renderOrderNum">Order position?</param>
        /// <returns></returns>
        public bool SetButtonIconRenderOrder(XRButton button, int renderOrderNum)
        {
            if (spawnedData.ContainsKey(button))
            {
                FPXRControllerRef cachedItem = spawnedItems[button];
                cachedItem.ChangeVisualDrawOrder(renderOrderNum);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sets the visual primary icon order in sort layer, returns true if it worked
        /// </summary>
        /// <param name="button">Button Icon to modify?</param>
        /// <param name="renderOrderNum">Order position?</param>
        /// <returns></returns>
        public bool SetButtonSecondaryIconRenderOrder(XRButton button, int renderOrderNum)
        {
            if (spawnedData.ContainsKey(button))
            {
                FPXRControllerRef cachedItem = spawnedItems[button];
                cachedItem.ChangeSecondaryVisualDrawOrder(renderOrderNum);
                return true;
            }
            return false;
        }
        #endregion
        #region Internal Functions
        /// <summary>
        /// Will manage events associated with delegate
        /// </summary>
        /// <param name="button">The button?</param>
        /// <param name="buttonState">The button state?</param>
        protected void ActivateEventsByButtonByState(XRButton button, XRInteractionStatus buttonState)
        {
            switch (button)
            {
                case XRButton.PrimaryButton:
                    switch (buttonState)
                    {
                        case XRInteractionStatus.None:
                            primeButtonUnlocked?.Invoke(button,buttonState);
                            break;
                        case XRInteractionStatus.Select:
                            primeButtonDown?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Unselect:
                            primeButtonUp?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Locked:
                            primeButtonLocked?.Invoke(button, buttonState);
                            break;
                    }
                    break;
                case XRButton.SecondaryButton:
                    switch (buttonState)
                    {
                        case XRInteractionStatus.None:
                            secondButtonUnlocked?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Select:
                            secondButtonDown?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Unselect:
                            secondButtonUp?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Locked:
                            secondButtonLocked?.Invoke(button, buttonState);
                            break;
                    }
                    break;
                case XRButton.Trigger:
                    switch (buttonState)
                    {
                        case XRInteractionStatus.None:
                            triggerButtonUnlocked?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Select:
                            triggerButtonDown?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Unselect:
                            triggerButtonUp?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Locked:
                            triggerButtonLocked?.Invoke(button, buttonState);
                            break;
                    }
                    break;
                case XRButton.Grip:
                    switch (buttonState)
                    {
                        case XRInteractionStatus.None:
                            gripButtonUnlocked?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Select:
                            gripButtonDown?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Unselect:
                            gripButtonUp?.Invoke(button, buttonState);
                            break;
                        case XRInteractionStatus.Locked:
                            gripButtonLocked?.Invoke(button, buttonState);
                            break;
                    }
                    break;

            }
        }
        /// <summary>
        /// Internal function to inspect and return needed data by button and state
        /// </summary>
        /// <param name="button">Button to look up?</param>
        /// <param name="state">State to compare it to?</param>
        /// <returns></returns>
        protected (FPXRControllerRef,ButtonLabelState,bool) LookUpByButtonState(XRButton button, XRInteractionStatus state)
        {
            FPXRControllerRef cachedItem = spawnedItems[button];
            ButtonFeedback cachedFeedback = spawnedData[button];
            ButtonLabelState? cachedLabelDetails = cachedFeedback.ButtonInteractionStates.Find(l => l.XRState == state);
            if (cachedLabelDetails.HasValue)
            {
                var label = cachedLabelDetails.Value;
                if (label.XRState == state) 
                {
                    return (cachedItem, label, true);
                }
                else
                {
                    return (cachedItem, label, false);
                }

            }
            return (null, cachedLabelDetails.Value, false);
        }
        /// <summary>
        /// Call the cached lock state we have on file
        /// </summary>
        /// <param name="cachedItem"></param>
        /// <param name="lockState"></param>
        protected virtual void ButtonStateLock(FPXRControllerRef cachedItem)
        {
            cachedItem.ApplyUIChanges(lockStateDetails.Icon, lockStateDetails.LabelText, lockStateDetails.LabelFontSetting, lockStateDetails.ButtonSound, UseCanvas);
        }
        /// <summary>
        /// Will update the dictionary to make our button state be in a dual state of Hint
        /// </summary>
        /// <param name="button">button in hint?</param>
        protected virtual void SetHintModeActive(XRButton button, bool activeHint)
        {
            if (controllerButtonsHint.ContainsKey(button))
            {
                controllerButtonsHint[button] = activeHint;
            }
        }
        /// <summary>
        /// Will update the dictionary to make our button state be in a dual state of information
        /// </summary>
        /// <param name="button">button in information?</param>
        protected virtual void SetInformationModeActive(XRButton button, bool activeInfo)
        {
            if (controllerButtonsInformation.ContainsKey(button))
            {
                controllerButtonsInformation[button] = activeInfo;
            }
        }
        #endregion
    }
}
