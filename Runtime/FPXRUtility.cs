namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility.EDU;
    using FuzzPhyte.Utility;
    using FuzzPhyte.Utility.Meta;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;

    public static class FPXRUtility
    {
        //XR static class for various needs
    }
    [Serializable]
    public enum LatchState
    {
        None = 0,
        Open = 1,
        Closed = 2
    }
    [Serializable]
    public enum CaseStatus
    {
        None,
        Open,
        Closed
    }
    [Serializable]
    public enum XRHandedness
    {
        Left = 0,
        Right = 1,
        NONE=2
    }
    [Serializable]
    public enum XRButton
    {
        NA = 0,
        Trigger = 1,
        Grip = 2,
        PrimaryButton = 3,
        SecondaryButton = 4,
        Thumbstick = 5,
        MenuButton =6,
        ExtraButton = 9
    }
    [Serializable]
    public enum XRInteractionStatus
    {
        None = 0,
        Hover = 1,
        Select = 2,
        Unselect = 3,
        InteractorViewAdded = 4,
        InteractorViewRemoved = 5,
        SelectingInteractorViewAdded = 6,
        SelectingInteractorViewRemoved = 7,
        UnHover = 8,
        Locked = 9,
        Information = 10,
        Hint = 11,
    }
    [Serializable]
    public enum XRAxis
    {
        Right = 0,
        Up = 1,
        Forward = 2
    }
    [Serializable]
    public enum XRInteractorType
    {
        None = 0,
        Hand = 1,
        Grab = 2,
        Ray = 3,
        Poke = 4,
        Distance = 5,
        Custom = 9,
    }
    [Serializable]
    public enum XRInteractorState
    {
        None = 0,
        Locked = 1,
        Closed = 2,
        Open = 3,
        IsOccupied=4,
        SocketBit=5,
        Custom = 9
    }
    [Serializable]
    public struct ContainerRequirementD
    {
        public FP_Tag RequirementTag;
        public string RequirementName;
        public LatchState LatchState;
    }
    [Serializable]
    public struct XRWorldButton
    {
        public XRButton ButtonType;
        public Transform ButtonLocation;
        public Vector3 ScaleAdjustment;

    }
    public struct XRButtonData
    {
        public GameObject WorldItem;
        public XRWorldButton WorldButton;
    }
    /// <summary>
    /// Based on the Button State, the label will change
    /// </summary>
    [System.Serializable]
    public struct ButtonFeedback
    {
        public XRButton Button;
        [Tooltip("Always make sure to have 'None' & 'Locked'")]
        public List<ButtonLabelState> ButtonInteractionStates;
    }
    [Serializable]
    public struct ButtonLabelState
    {
        public XRInteractionStatus XRState;
        public AudioClip ButtonSound;
        [TextArea(2,3)]
        public string LabelText;
        [Space]
        [Header("Primary UI")]
        public Sprite Icon;
        public Color IconColor;
        public FontSetting LabelFontSetting;
        public float ScaleAdjust;
        [Header("Secondary UI")]
        [Tooltip("Overall Additional Icons if Needed")]
        public Sprite HintIcon;
        public Color HintIconColor;
        public Sprite InformationIcon;
        public Color InformationIconColor;
    }
    public interface IFPXREventBinder
    {
        void BindHover(UnityAction action);
        void BindUnhover(UnityAction action);
        void BindSelect(UnityAction action);
        void BindUnselect(UnityAction action);
        void BindInteractorViewAdded(UnityAction action);
        void BindInteractorViewRemoved(UnityAction action);
        void BindSelectingInteractorViewAdded(UnityAction action);
        void BindSelectingInteractorViewRemoved(UnityAction action);

        void UNBindHover(UnityAction action);
        void UNBindUnHover(UnityAction action);
        void UNBindSelect(UnityAction action);
        void UNBindUnselect(UnityAction action);
        void UNBindInteractorViewAdded(UnityAction action);
        void UNBindInteractorViewRemoved(UnityAction action);
        void UNBindSelectingInteractorViewAdded(UnityAction action);
        void UNBindSelectingInteractorViewRemoved(UnityAction action);
        
    }
    [Serializable]
    public struct XRDetailedLabelData
    {
        [Space]
        public FP_Tag TagData;
        public FPXRSocketTag SocketTag;
        public FP_Vocab VocabData;
        public List<XRVocabSupportData> SupportVocabData;
        public FP_Theme ThemeData;
    }
    [Serializable]
    public struct XRVocabSupportData
    {
        public FP_Vocab SupportData;
        public FP_VocabSupport SupportCategory;
    }
    public interface IFPXRLabel
    {
        void SetupLabelData(XRDetailedLabelData data, FP_Language startingLanguage, bool startActive=true, bool useCombinedVocab=false);
        string DisplayVocabTranslation(FP_Language language=FP_Language.USEnglish);
        bool ShowAllRenderers(bool status);
        void ForceShowRenderer();
        void ForceHideRenderer();
        GameObject ReturnGameObject();
        FPLabelTag ReturnDataObject();
    }
    /// <summary>
    /// Help manage delegate/event requirements for Controllers and Button Events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFPXRControllerSetup<T>
    {
        void RemoveAllListeners();
        void SetupItemForListeningAllEvents(T item);
        void RemoveItemForListeningAllEvents(T item);
    }
    /// <summary>
    /// Functions required to be a full blown controller listener for various actions/events as part of FPXRControllerEventManager
    /// </summary>
    public interface IFPXRControllerListener
    {
        void SetupControllerListener(FPXRControllerFeedback leftControllerRef, FPXRControllerFeedback rightControllerRef);
        void AnyControllerButtonPressed(XRHandedness hand, XRButton button);
        void AnyControllerButtonReleased(XRHandedness hand, XRButton button);
        void AnyControllerHintActive(XRHandedness hand, XRButton button);
        void AnyControllerHintDeactive(XRHandedness hand, XRButton button);
        void AnyControllerInfoActive(XRHandedness hand, XRButton button);
        void AnyControllerInfoDeactive(XRHandedness hand, XRButton button);
        void AnyControllerButtonLocked(XRHandedness hand, XRButton button);
        void AnyControllerButtonUnlocked(XRHandedness hand, XRButton button);
        void AnyControllerLocked(XRHandedness hand, XRButton button);
        void AnyControllerUnlocked(XRHandedness hand, XRButton button);
        void RightControllerReset(XRHandedness hand, XRButton button);
        void LeftControllerReset(XRHandedness hand, XRButton button);

    }
    /// <summary>
    /// Functions required to be a full blown button listener for various VR controllers as part of the FPXRControllerFeedback
    /// </summary>
    public interface IFPXRButtonListener
    {
        void SetupButtonListener(FPXRControllerFeedback controllerRef);
        void PrimaryButtonDown(XRButton button, XRInteractionStatus buttonState);
        void PrimaryButtonUp(XRButton button, XRInteractionStatus buttonState);
        void PrimaryButtonLocked(XRButton button, XRInteractionStatus buttonState);
        void PrimaryButtonUnlocked(XRButton button, XRInteractionStatus buttonState);

        void SecondaryButtonDown(XRButton button, XRInteractionStatus buttonState);
        void SecondaryButtonUp(XRButton button, XRInteractionStatus buttonState);
        void SecondaryButtonLocked(XRButton button, XRInteractionStatus buttonState);
        void SecondaryButtonUnlocked(XRButton button, XRInteractionStatus buttonState);

        void TriggerButtonDown(XRButton button, XRInteractionStatus buttonState);
        void TriggerButtonUp(XRButton button, XRInteractionStatus buttonState);
        void TriggerButtonLocked(XRButton button, XRInteractionStatus buttonState);
        void TriggerButtonUnlocked(XRButton button, XRInteractionStatus buttonState);

        void GripButtonDown(XRButton button, XRInteractionStatus buttonState);
        void GripButtonUp(XRButton button, XRInteractionStatus buttonState);
        void GripButtonLocked(XRButton button, XRInteractionStatus buttonState);
        void GripButtonUnlocked(XRButton button, XRInteractionStatus buttonState);
    }
    public interface IFPXREventWrapper
    {
        void Initialize(IFPXREventBinder eventBinder);
        void OnHover();
        void OnUnhover();
        void OnSelect();
        void OnUnselect();
        void OnInteractorViewAdded();
        void OnInteractorViewRemoved();
        void OnSelectingInteractorViewAdded();
        void OnSelectingInteractorViewRemoved();
    }
}
