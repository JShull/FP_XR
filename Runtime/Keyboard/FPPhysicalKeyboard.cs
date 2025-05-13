namespace FuzzPhyte.XR
{
    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections;
    using FuzzPhyte.Utility;
    using TMPro;
   
    public class FPPhysicalKeyboard : MonoBehaviour
    {
        [Header("Keyboard Parent")]
        public GameObject KeyboardParent;
        
        [SerializeField] Vector3 startScale = Vector3.one;
        [SerializeField] Vector3 endScale = new Vector3(0.1f,0.1f,0.1f);
        public FP_ScaleLerp KeyboardScaler;
        public TMP_InputField ActiveInputField;
        [Tooltip("Input Fields you want us to monitor")]
        [SerializeField] protected List<TMP_InputField> allInputFields = new List<TMP_InputField>();
        [SerializeField] protected List<FPPhysicalButton> theKeys = new List<FPPhysicalButton>();
        public bool CapsOn;
        public bool ShiftDown;
        [Space]
        [Tooltip("Will only use raycast approach, will deactivate all physics Trigger/stay/exit based items/components")]
        public bool UseRaycastONLY = false;
        public bool ReplaceLayerMaskingForButtons = false;
        [SerializeField] protected LayerMask triggerLayersKeyboard;
        [TextArea(2,4)]
        [SerializeField] protected string typedField;
        public string TypedField { get => typedField;
            set
            {
                if (typedField != value)
                {
                    typedField = value;
                    if (ActiveInputField != null)
                    {
                        ActiveInputField.text = typedField;
                        ActiveInputField.caretPosition = ActiveInputField.text.Length;
                    }
                }
            }
        }
        [SerializeField] protected bool displaying;
        [Space]
        [Header("Events")]
        public UnityEvent CapKeyOn;
        public UnityEvent CapKeyOff;
        public Transform CapsRelativeLocation;
        public Vector3 KeyCapSize = new Vector3(0.05f, 0.05f,0.05f);
        public float KeyCapSpaceInteriorScale = 0.1f;
        [Tooltip("We jump this audio component around based on key information")]
        public AudioSource KeyButtonSoundEmitter;
        public AudioSource KeyButtonSoundEmitterBackup;
        public AudioClip KeyButtonSoundPressed;
        public AudioClip KeyButtonSoundReleased;
        public AudioClip KeyButtonSoundHover;
        public AudioClip KeyButtonSoundUnHover;
        public AudioClip CapsActive;
        public AudioClip CapsInactive;
        public virtual void OnEnable()
        {
            if (theKeys.Count > 0)
            {
                for (int i = 0; i < theKeys.Count; i++)
                {
                    var aKey = theKeys[i];
                    aKey.IsPressed += KeyPressed;
                    aKey.IsReleased += KeyReleased;
                    aKey.IsHeldDown += KeyHeldDown;
                    aKey.IsHovering += KeyHover;
                    aKey.IsUnhovering += KeyUnhover;
                    if (UseRaycastONLY)
                    {
                        RemovePhysicsFromButtons(aKey);
                    }
                    if (ReplaceLayerMaskingForButtons)
                    {
                        ReplaceLayerMaskForButtons(aKey);
                    }
                }
                Debug.LogWarning($"On Enable: Keys Registered!");
            }
            if (allInputFields.Count == 0)
            {
                Debug.LogError($"You should reference some input fields, or the keyboard might never work right!");
            }
            foreach (var input in allInputFields)
            {
                input.onSelect.AddListener(delegate { OnAnyTMPInputFieldSelected(input); });
                input.onDeselect.AddListener(delegate { OnAnyTMPInputFieldDeselect(input); });
            }
        }
        public virtual void Start()
        {
            //loop through all children and find FPPhysicalButtons
            if (theKeys.Count == 0)
            {
                //check children?
                for (int i = 0; i < transform.childCount; i++)
                {
                    var aChild = transform.GetChild(i);
                    if (aChild.gameObject.GetComponent<FPPhysicalButton>())
                    {
                        var aKey = aChild.gameObject.GetComponent<FPPhysicalButton>();
                        aKey.IsPressed += KeyPressed;
                        aKey.IsReleased += KeyReleased;
                        aKey.IsHeldDown += KeyHeldDown;
                        aKey.IsHovering += KeyHover;
                        aKey.IsUnhovering += KeyUnhover;
                        theKeys.Add(aKey);
                        if (UseRaycastONLY)
                        {
                            RemovePhysicsFromButtons(aKey);
                        }
                        if (ReplaceLayerMaskingForButtons)
                        {
                            ReplaceLayerMaskForButtons(aKey);
                        }
                    }
                }
            }
            Debug.LogWarning($"On Start: Keys Should already be registered");
            if (KeyboardScaler != null)
            {
                KeyboardScaler.TargetObject = KeyboardParent.transform;
                KeyboardScaler.StartScale = endScale;
                KeyboardScaler.EndScale = startScale;
                KeyboardScaler.ResetMotion();
                KeyboardScaler.TargetObject.localScale = endScale;
            }
            //default is to start off
            foreach (var aKey in theKeys)
            {
                aKey.KeyActive = false;
            }
            KeyboardParent.SetActive(false);
            displaying = false;
        }

        public virtual void OnDisable()
        {
            if (theKeys.Count > 0)
            {
                for (int i = 0; i < theKeys.Count; i++)
                {
                    var aKey = theKeys[i];
                    aKey.IsPressed -= KeyPressed;
                    aKey.IsReleased -= KeyReleased;
                    aKey.IsHeldDown -= KeyHeldDown;
                    aKey.IsHovering -= KeyHover;
                    aKey.IsUnhovering -= KeyUnhover;
                }
            }
           
            for (int j = 0; j < allInputFields.Count; j++)
            {
                var inputField = allInputFields[j];
                inputField.onSelect.RemoveListener(delegate { OnAnyTMPInputFieldSelected(inputField); });
                inputField.onDeselect.RemoveListener(delegate { OnAnyTMPInputFieldDeselect(inputField); });
            }
        }
        protected void RemovePhysicsFromButtons(FPPhysicalButton aKey)
        {
            if (aKey.FPCollider.GetComponent<FPPhysicalButtonCollision>() != null)
            {
                var ButtonCollision = aKey.FPCollider.GetComponent<FPPhysicalButtonCollision>();
                ButtonCollision.UsePhysicsMode = false;
            }
        }
        protected void ReplaceLayerMaskForButtons(FPPhysicalButton aKey)
        {
            if (aKey.FPCollider.GetComponent<FPPhysicalButtonCollision>() != null)
            {
                var ButtonCollision = aKey.FPCollider.GetComponent<FPPhysicalButtonCollision>();
                ButtonCollision.TriggeredLayers = triggerLayersKeyboard;
            }
        }
        protected virtual void OnAnyTMPInputFieldSelected(TMP_InputField inputField)
        {
            if (ActiveInputField != null)
            {
                //we had an active inputfield already do we just clear that input out?
                ActiveInputField = null;
                //clear typed field
                typedField = "";
            }
            ActiveInputField = inputField;
            if (!displaying)
            {
                DisplayKeyboard(true);
            }
            if (ActiveInputField.text.Length > 0)
            {
                //update my typeField to match what's in the ActiveInputField
                typedField= ActiveInputField.text;
                ActiveInputField.caretPosition = ActiveInputField.text.Length;
            }
        }
        protected virtual void OnAnyTMPInputFieldDeselect(TMP_InputField inputField)
        {
            if (ActiveInputField == inputField)
            {
                //we had an active inputfield already do we just clear that input out?
                ActiveInputField = null;
                typedField = "";
            }
        }
        
        protected void KeyPressed(FPPhysicalButton aKey)
        {
            string keyValue = "";
            KeyAudioEvent(KeyButtonSoundPressed, aKey.transform.position);
            switch (aKey.KeyType)
            {
                case Utility.FPKeyboardKey.Backspace:
                    if (typedField.Length > 0)
                    {
                        TypedField = typedField.Substring(0, (typedField.Length - 1));
                    }
                    return;
                case Utility.FPKeyboardKey.Space:
                    TypedField += " ";
                    return;
                case Utility.FPKeyboardKey.Tab:
                    TypedField += "    ";
                    return;
                case Utility.FPKeyboardKey.Shift:
                    ShiftDown = true;
                    return;
                case Utility.FPKeyboardKey.Enter:
                    keyValue = Environment.NewLine;
                    TypedField += keyValue;
                    return;
                case Utility.FPKeyboardKey.CapsLock:
                    CapsOn = !CapsOn;
                    if (CapsOn)
                    {
                        CapKeyOn.Invoke();
                        KeyAudioEvent(CapsActive, CapsRelativeLocation.position);
                    }
                    else
                    {
                        CapKeyOff.Invoke();
                        KeyAudioEvent(CapsInactive, CapsRelativeLocation.position);
                    }
                    return;
                case Utility.FPKeyboardKey.Esc:
                    DisplayKeyboard(false);
                    return;
            }
            if(CapsOn || ShiftDown)
            {
                keyValue = aKey.FPKeyShift.ToString();
            }
            else
            {
                keyValue = aKey.FPKey.ToString();
            }
            TypedField += keyValue;
        }
        protected void KeyHeldDown(FPPhysicalButton aKey)
        {
            if(aKey.KeyType == Utility.FPKeyboardKey.CapsLock)
            {
                //cant really do much while holding caps as it just stays as if it were pressed
                return;
            }
            KeyPressed(aKey);
        }
        protected virtual void KeyHover(FPPhysicalButton aKey)
        {
            KeyAudioEvent(KeyButtonSoundHover, aKey.transform.position);
        }
        protected virtual void KeyUnhover(FPPhysicalButton aKey)
        {
            KeyAudioEvent(KeyButtonSoundUnHover, aKey.transform.position);
        }
        protected void KeyReleased(FPPhysicalButton aKey) 
        {
            KeyAudioEvent(KeyButtonSoundReleased, aKey.transform.position);
            switch (aKey.KeyType)
            {
                case Utility.FPKeyboardKey.Shift:
                    ShiftDown = false;
                    break;
            }
        }
        protected virtual void KeyAudioEvent(AudioClip aClip, Vector3 worldLocation)
        {
            if (KeyButtonSoundEmitter != null)
            {
                if (KeyButtonSoundEmitter.isPlaying)
                {
                    //try other one
                    if (KeyButtonSoundEmitterBackup != null)
                    {
                        KeyButtonSoundEmitterBackup.transform.position = worldLocation;
                        KeyButtonSoundEmitterBackup.clip = aClip;
                        KeyButtonSoundEmitterBackup.Play();
                    }
                }
                else
                {
                    KeyButtonSoundEmitter.transform.position = worldLocation;
                    KeyButtonSoundEmitter.clip = aClip;
                    KeyButtonSoundEmitter.Play();
                }
            }
        }
        public void DisplayKeyboard(bool display)
        {
            if (displaying == display)
            {
                //already displaying just need to refocus the object to the correct field
                Debug.LogWarning($"Keyboard is already in the same state");
                return;
            }
            if (KeyboardScaler != null)
            {
                if (display)
                {

                    KeyboardScaler.StartScale = endScale;
                    KeyboardScaler.EndScale = startScale;
                    KeyboardScaler.ResetMotion();
                    KeyboardParent.SetActive(true);
                    KeyboardScaler.OnEndMotion.RemoveAllListeners();
                    KeyboardScaler.OnEndMotion.AddListener(OnEndScaleUp);
                    KeyboardScaler.StartMotion();
                }
                else
                {
                    KeyboardScaler.StartScale = startScale;
                    KeyboardScaler.EndScale = endScale;
                    KeyboardScaler.ResetMotion();
                    foreach (var aKey in theKeys)
                    {
                        aKey.KeyActive = false;
                    }
                    KeyboardScaler.OnEndMotion.RemoveAllListeners();
                    KeyboardScaler.OnEndMotion.AddListener(OnEndScaleDown);
                    KeyboardScaler.StartMotion();
                }
            }
            
            displaying = display;
        }
        public void OnEndScaleUp()
        {
            foreach(var aKey in theKeys)
            {
                aKey.KeyActive = true;
            }
        }
        public void OnEndScaleDown()
        {
            KeyboardParent.SetActive(false);
        }
#if UNITY_EDITOR
        [ContextMenu("Test Show Keyboard")]
        public void TestShowKeyboard()
        {
            DisplayKeyboard(true);
        }
        [ContextMenu("Test Hide Keyboard")]
        public void TestHideKeyboard()
        {
            DisplayKeyboard(false);
        }
        [ContextMenu("Test Typing Keyboard Sync")]
        public void TypeOutRandomLetter()
        {
            var randomIndex = UnityEngine.Random.Range(0, theKeys.Count);
            var randomKey = theKeys[randomIndex];
            if(randomKey.KeyType== FPKeyboardKey.RegularKey||randomKey.KeyType==FPKeyboardKey.NumericalKey||randomKey.KeyType==FPKeyboardKey.PunctuationKey)
            {
                KeyPressed(randomKey);
            }
        }
#endif
    }
}
