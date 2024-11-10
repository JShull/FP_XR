namespace FuzzPhyte.XR
{
    using UnityEngine;
    using UnityEngine.Events;
    using FuzzPhyte.SGraph;
    using System.Collections;
    public class FPXRLatch<GRT,G,GI,PEW> : MonoBehaviour where GRT:MonoBehaviour where G: MonoBehaviour where GI: MonoBehaviour where PEW: MonoBehaviour
    {
        public ContainerRequirementD LatchRequirementStatus;
        public LatchState CurrentLatchState = LatchState.Closed;
        //[SerializeField]
        //private bool latched = true;
        #region Delegates for Latch Events
        public delegate void LatchDelegate(RequirementD aReq, bool aStat);
        public delegate void LatchStateDelegate(ContainerRequirementD aState);
        public LatchStateDelegate OnLatchStateOpened;
        public LatchStateDelegate OnLatchStateClosed;
        #endregion
        //open
        public UnityEvent OnLatchedFalse;
        //closed
        public UnityEvent OnLatchedTrue;
        //public GameObject LatchManager;
        public XRAxis LatchManagerAxis;
        public GRT LatchManager;
        public G LatchGrabber;
        public GI LatchInteractor;
        public PEW LatchWrapper;
        [Space]
        [Header("Latch Settings")]
        public float MinAngleLatch = 0;
        public float MaxAngleLatch = 90;
        public float AngleOpenRequirement = 45;
        [SerializeField]
        protected float lastLatchAngle = 0;
        protected bool latchGrabbed;

        /*
        public virtual void LatchGrabbedPointer(PointerEvent evt)
        {
            Debug.LogWarning($"LatchGrabbed Pointer! {evt.Type.ToString()}");

            Debug.LogWarning($"Latch Data? {evt.Data.ToString()}");
        }
        */

        //Called via Grabber/interactable
        public virtual void LatchGrabbed()
        {
            latchGrabbed = true;
            Debug.LogWarning($"Latch Grabbed!!");
            StartCoroutine(ContinuouslyCheckLatch());
        }
        //Called via Grabber/interactable
        public virtual void LatchLetGo()
        {
            latchGrabbed = false;
            Debug.LogWarning($"Latch Let go!!");
        }
        protected virtual IEnumerator ContinuouslyCheckLatch()
        {
            while (latchGrabbed)
            {
                CheckLatchOpen();
                yield return new WaitForEndOfFrame();
            }
        }
        /// <summary>
        /// Update the Latch Manager Axis
        /// </summary>
        /// <param name="someEnum">0=up, 1=right, 2=forward</param>
        public void UpdateLatchManagerAxis(int someEnum)
        {
            //0  = up, 1 = right, 2 = forward
            LatchManagerAxis = (XRAxis)someEnum;
        }
        protected virtual void CheckLatchOpen()
        {
            //Debug.LogWarning($"Checking Latch Open {LatchManager.RotationAxis.ToString()}");
            switch (LatchManagerAxis)
            {
                case XRAxis.Up:
                    lastLatchAngle = this.transform.localEulerAngles.y;
                    break;
                case XRAxis.Right:
                    lastLatchAngle = this.transform.localEulerAngles.x;
                    break;
                case XRAxis.Forward:
                    lastLatchAngle = this.transform.localEulerAngles.z;
                    break;
            }
            if (lastLatchAngle >= AngleOpenRequirement)
            {
                //we have exceed the angle requirement
                //are we currently closed? lets open it up
                if (CurrentLatchState == LatchState.Closed)
                {
                    CurrentLatchState = LatchState.Open;
                    LatchRequirementStatus.LatchState = CurrentLatchState;
                    OnLatchedFalse.Invoke();
                    OnLatchStateOpened?.Invoke(LatchRequirementStatus);
                }
            }
            else
            {
                if (CurrentLatchState == LatchState.Open)
                {
                    CurrentLatchState = LatchState.Closed;
                    LatchRequirementStatus.LatchState = CurrentLatchState;
                    OnLatchedTrue.Invoke();
                    OnLatchStateClosed?.Invoke(LatchRequirementStatus);
                }
            }
        }
    }
}
