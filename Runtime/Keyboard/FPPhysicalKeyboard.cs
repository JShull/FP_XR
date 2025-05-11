
namespace FuzzPhyte.XR
{

    using UnityEngine;
    using UnityEngine.Events;
    using System.Collections.Generic;
    using System;

    public class FPPhysicalKeyboard : MonoBehaviour
    {
        [SerializeField] protected List<FPPhysicalButton> theKeys = new List<FPPhysicalButton>();
        public bool CapsOn;
        public bool ShiftDown;
        [TextArea(2,4)]
        [SerializeField] protected string typedField;
        public UnityEvent CapKeyOn;
        public UnityEvent CapKeyOff;
        public void OnEnable()
        {
            if (theKeys.Count > 0)
            {
                for (int i = 0; i < theKeys.Count; i++)
                {
                    var aKey = theKeys[i];
                    aKey.IsPressed += KeyPressed;
                    aKey.IsReleased += KeyReleased;
                    aKey.IsHeldDown += KeyHeldDown;
                }
            }
        }
        public void Start()
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
                        theKeys.Add(aKey);
                    }
                }
            }
        }
        
        public void OnDisable()
        {
            if (theKeys.Count > 0)
            {
                for (int i = 0; i < theKeys.Count; i++)
                {
                    var aKey = theKeys[i];
                    aKey.IsPressed -= KeyPressed;
                    aKey.IsReleased -= KeyReleased;
                    aKey.IsHeldDown -= KeyHeldDown;
                }
            }
        }

        protected void KeyPressed(FPPhysicalButton aKey)
        {
            string keyValue = "";
            switch (aKey.KeyType)
            {
                case Utility.FPKeyboardKey.Backspace:
                    if (typedField.Length > 0)
                    {
                        typedField = typedField.Substring(0, (typedField.Length - 1));
                    }
                    return;
                case Utility.FPKeyboardKey.Space:
                    typedField += " ";
                    return;
                case Utility.FPKeyboardKey.Shift:
                    ShiftDown = true;
                    return;
                case Utility.FPKeyboardKey.Enter:
                    keyValue = Environment.NewLine;
                    typedField += keyValue;
                    return;
                case Utility.FPKeyboardKey.CapsLock:
                    CapsOn = !CapsOn;
                    if (CapsOn)
                    {
                        CapKeyOn.Invoke();
                    }
                    else
                    {
                        CapKeyOff.Invoke();
                    }
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
            typedField += keyValue;
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
        protected void KeyReleased(FPPhysicalButton aKey) 
        {
            switch (aKey.KeyType)
            {
                case Utility.FPKeyboardKey.Shift:
                    ShiftDown = false;
                    break;
            }
        }
    }
}
