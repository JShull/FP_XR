namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using UnityEngine;
    using System;

    [Serializable]
    [CreateAssetMenu(fileName = "FPKeyData", menuName = "FuzzPhyte/Utility/XR/KeyBoardKey")]
    public class FPPhysicalKeyData : FP_Data
    {
        public char TheFPKey;
        public char TheFPKeyShift;
        public FPKeyboardKey TheKeyType = FPKeyboardKey.None;
    }
}
