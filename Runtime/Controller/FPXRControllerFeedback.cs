namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UIElements;

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
        protected Dictionary<XRButton, bool> controllerLocked = new Dictionary<XRButton, bool>();
        [Tooltip("World Location Ref by Type")]
        public List<XRWorldButton> XRWorldButtons = new List<XRWorldButton>();
        
        private XRHandedness controllerHandedness;
        [Tooltip("Temp fix/cache internal recognize a 'lock' and ignore other commands")]
        
        protected ButtonLabelState lockStateDetails;

        public virtual void Awake()
        {
            controllerHandedness = feedbackConfig.ControllerStartHandedness;
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
                                }
                            }
                            if(aState.XRState == XRInteractionStatus.Locked)
                            {
                                lockStateDetails = aState;
                            }
                        }
                        if(controllerRefData != null)
                        {
                            spawnedData.Add(matchButton, curFeedback);
                            spawnedItems.Add(matchButton, controllerRefData);
                            controllerLocked.Add(matchButton, false);
                        }
                        break;
                    }
                }
            }

        }

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
        public virtual void LockController(XRButton button)
        {
            if (controllerLocked.ContainsKey(button)) 
            { 
                controllerLocked[button] = true;
            }
        }
        public virtual void UnlockControllerButton(XRButton button)
        {
            if (controllerLocked.ContainsKey(button))
            {
                controllerLocked[button] = false;
            }

        }
        public virtual void SetButtonState(XRButton button, XRInteractionStatus currentButtonStatus)
        {
            //if state is locked we need to set it and then not visually update anything until unlocked, keep firing locked events 
            //this finds feedback based on original data files
            //we want to transfer this over to my cached data

            if (spawnedData.ContainsKey(button)) 
            { 
                ButtonFeedback cachedFeedback = spawnedData[button];
                //world ref item
                FPXRControllerRef cachedItem = spawnedItems[button];
                ButtonLabelState? cachedLabelDetails = cachedFeedback.ButtonInteractionStates.Find(l => l.XRState == currentButtonStatus);
               
                if (cachedLabelDetails.HasValue)
                {
                    var label = cachedLabelDetails.Value;
                    bool lockState = controllerLocked[button];
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
                                LockController(button);
                            }
                        }
                    }
                    else 
                    { 
                        Debug.LogError($"Missing the state or didn't find a match, looking for {currentButtonStatus}!");
                    }
                }
            }
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

        
    }
}
