namespace FuzzPhyte.XR
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class FPXRControllerFeedback : MonoBehaviour
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
        /// <summary>
        /// Cached data and spawned gameobject ref
        /// </summary>
        protected Dictionary<XRButton,ButtonFeedback> spawnedData = new Dictionary<XRButton, ButtonFeedback>();
        protected Dictionary<XRButton, FPXRControllerRef> spawnedItems = new Dictionary<XRButton, FPXRControllerRef>();
        protected Dictionary<XRButton, bool> controllerButtonsLocked = new Dictionary<XRButton, bool>();
        protected Dictionary<XRButton, bool> controllerButtonsHint = new Dictionary<XRButton, bool>();
        protected Dictionary<XRButton,bool> controllerButtonsInformation = new Dictionary<XRButton, bool>();
        protected bool fullControllerLocked = false;
        [Tooltip("Controller level icon related states-information: used to check and/or manage delays/timers etc.")]
        protected bool informationActive;
        [Tooltip("Controller level icon related states-hint: used to check and/or manage delays/timers etc.")]
        protected bool hintActive;
        public bool ControllerLocked { get { return fullControllerLocked; } }
        [Tooltip("World Location Ref by Type")]
        public List<XRWorldButton> XRWorldButtons = new List<XRWorldButton>();
        
        //private XRHandedness controllerHandedness;
        [Tooltip("Temp fix/cache internal recognize a 'lock' and ignore other commands")]
        
        protected ButtonLabelState lockStateDetails;

        public virtual void Awake()
        {
            //controllerHandedness = feedbackConfig.ControllerStartHandedness;
            SpawnControllerItems();
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
        /*
        #region Testing
        [ContextMenu("Testing Primary, Select")]
        public void PrimaryControllerButtonDown()
        {
            SetButtonState(XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing Primary A, Unselect")]
        public void PrimaryControllerButtonUp()
        {
            SetButtonState(XRButton.PrimaryButton, XRInteractionStatus.Unselect);
        }
        [ContextMenu("Testing Secondary, Select")]
        public void SecondaryControllerButtonDown()
        {
            SetButtonState(XRButton.SecondaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing Secondary, Unselect")]
        public void SecondaryControllerButtonUp()
        {
            SetButtonState(XRButton.SecondaryButton, XRInteractionStatus.Unselect);
        }
        [ContextMenu("Testing Locking Primary")]
        public void PrimaryControllerLock()
        {
            SetButtonState(XRButton.PrimaryButton, XRInteractionStatus.Locked);
        }
        [ContextMenu("Unlocking Primary")]
        public void UnlockPrimaryController()
        {
            //have to unlock first
            UnlockControllerButton(XRButton.PrimaryButton);
            SetButtonState(XRButton.PrimaryButton, XRInteractionStatus.None);
        }
        [ContextMenu("Testing - error state")]
        public void PrimaryControllerError()
        {
            SetButtonState(XRButton.PrimaryButton, XRInteractionStatus.Hover);
        }
        #endregion
        */
        #region Public Accessors for events/Updates
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
        public void SetButtonState(XRButton button, XRInteractionStatus currentButtonStatus)
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
                            cachedItem.ApplyUIChanges(label.Icon, label.LabelText, label.LabelFontSetting, label.ButtonSound, UseCanvas, useDownOffset);
                            /// these conditions manage additional state via lock/hint/information
                            /// this is a catch area as we should be setting these states by function references above
                            /// In cases where we pass this into the SetButtonState we still want the action to be invoked
                            if (currentButtonStatus == XRInteractionStatus.Locked)
                            {
                                //lock us up
                                LockControllerButton(button);
                            }
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
    }
}
