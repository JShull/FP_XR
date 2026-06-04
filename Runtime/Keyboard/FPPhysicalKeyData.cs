// Copyright (c) 2026 John B. Shull
// FuzzPhyte LLC is a company associated with John B. Shull
// This file is part of FP_XR Package.
//
// Public license: GNU GPLv3-or-later.
// Commercial/proprietary use requires a separate license from John B. Shull.
//
// See LICENSE.md COMMERCIAL-LICENSE.md, and NOTICE.md.

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
