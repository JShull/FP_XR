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
        ExtraButton = 6
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
        UnHover = 8
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
    /// <summary>
    /// Based on the Button State, the label will change
    /// </summary>
    [System.Serializable]
    public struct ButtonFeedback
    {
        public XRButton Button;
        public List<ButtonLabelState> ButtonInteractionStates;
        [Tooltip("Used as a backup")]
        public Sprite ActionIcon;
        public Sprite AnimationSpriteSheet;
    }
    [Serializable]
    public struct ButtonLabelState
    {
        public XRInteractionStatus XRState;
        public AudioClip ButtonSound;
        [TextArea(2,3)]
        public string LabelText;
        [Space]
        public Sprite Icon;
        public FontSetting LabelFontSetting;
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
        public FP_Vocab VocabData;
        public FP_Theme ThemeData;
    }
    public interface IFPXRLabel
    {
        void SetupLabelData(XRDetailedLabelData data, FP_Language startingLanguage, bool startActive=true);
        string DisplayVocabTranslation(FP_Language language=FP_Language.USEnglish);
        void ShowAllRenderers(bool status);
    }
    /*
         *public class OVREventBinder : IEventBinder
        {
            private readonly InteractableUnityEventWrapper _eventWrapper;

            public OVREventBinder(InteractableUnityEventWrapper eventWrapper)
            {
                _eventWrapper = eventWrapper;
            }
            //public UnityEvent WhenHover 
            //public UnityEvent WhenUnhover 
            //public UnityEvent WhenSelect
            //public UnityEvent WhenUnselect 
            //public UnityEvent WhenInteractorViewAdded 
            //public UnityEvent WhenInteractorViewRemoved 
            //public UnityEvent WhenSelectingInteractorViewAdded 
            //public UnityEvent WhenSelectingInteractorViewRemoved 

            public void BindHover(UnityAction action) => _eventWrapper.WhenHover.AddListener(action);
            public void BindUnhover(UnityAction action) => _eventWrapper.WhenUnhover.AddListener(action);
            public void BindSelect(UnityAction action) => _eventWrapper.WhenSelect.AddListener(action);
            public void BindUnselect(UnityAction action) => _eventWrapper.WhenUnselect.AddListener(action);
            public void BindInteractorViewAdded(UnityAction action) => _eventWrapper.WhenInteractorViewAdded.AddListener(action);
            public void BindInteractorViewRemoved(UnityAction action) => _eventWrapper.WhenInteractorViewRemoved.AddListener(action);
            public void BindSelectingInteractorViewAdded(UnityAction action) => _eventWrapper.WhenSelectingInteractorViewAdded.AddListener(action);
            public void BindSelectingInteractorViewRemoved(UnityAction action) => _eventWrapper.WhenSelectingInteractorViewRemoved.AddListener(action);

            public void UnbindWhenHover(UnityAction action) => _eventWrapper.WhenHover.RemoveListener(action);
            public void UnbindWhenUnhover(UnityAction action) => _eventWrapper.WhenUnhover.RemoveListener(action);
            public void UnbindWhenSelect(UnityAction action) => _eventWrapper.WhenSelect.RemoveListener(action);
            public void UnbindWhenUnselect(UnityAction action) => _eventWrapper.WhenUnselect.RemoveListener(action);
            public void UnbindWhenInteractorViewAdded(UnityAction action) => _eventWrapper.WhenInteractorViewAdded.RemoveListener(action);
            public void UnbindWhenInteractorViewRemoved(UnityAction action) => _eventWrapper.WhenInteractorViewRemoved.RemoveListener(action);
            public void UnbindWhenSelectingInteractorViewAdded(UnityAction action) => _eventWrapper.WhenSelectingInteractorViewAdded.RemoveListener(action);
            public void UnbindWhenSelectingInteractorViewRemoved(UnityAction action) => _eventWrapper.WhenSelectingInteractorViewRemoved.RemoveListener(action);
        }
         */
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
