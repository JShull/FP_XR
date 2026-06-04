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
    using UnityEngine.Events;

    public class FPXRTester : MonoBehaviour
    {
        public float DelayTime = 3f;
        
        [Header("Test Events")]
        public bool TestOne;
        public UnityEvent TestEventOne;
        [Space]
        public bool TestTwo;
        public UnityEvent TestEventTwo;
        [Space]
        public bool TestThree;
        public UnityEvent TestEventThree;
        private FP_Timer testTimer;

        private void Start()
        {
            testTimer = FP_Timer.CCTimer;
            if(testTimer == null)
            {
                Debug.LogError("No Timer Found");
                return;
            }
            if (TestOne)
            {
                testTimer.StartTimer(DelayTime, TestEventOne.Invoke);
            }
            if (TestTwo)
            {
                testTimer.StartTimer(DelayTime * 2, TestEventTwo.Invoke);
            }
            if (TestThree)
            {
                testTimer.StartTimer(DelayTime * 3, TestEventThree.Invoke);
            }
            
        }
        
    }
}
