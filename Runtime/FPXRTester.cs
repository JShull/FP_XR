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
