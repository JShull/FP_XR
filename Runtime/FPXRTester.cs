namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using UnityEngine;
    using UnityEngine.Events;

    public class FPXRTester : MonoBehaviour
    {
        public float DelayTime = 3f;
        [Header("Test Events")]
        public UnityEvent TestEventOne;
        [Space]
        public UnityEvent TestEventTwo;
        [Space]
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
            testTimer.StartTimer(DelayTime, TestEventOne.Invoke);
            testTimer.StartTimer(DelayTime * 2, TestEventTwo.Invoke);
            testTimer.StartTimer(DelayTime * 3, TestEventThree.Invoke);
        }
        
    }
}
