namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class FPXRContainer<GRT, G, GI, PEW> : MonoBehaviour where GRT : MonoBehaviour where G : MonoBehaviour where GI : MonoBehaviour where PEW : MonoBehaviour
    {
        [Tooltip("Status of our Container")]
        public SequenceStatus ContainerStatus;
        public RotaterStatus ContainerRotaterStatus = RotaterStatus.Closed;
        public GRT ContainerRotateManager;
        public float ContainerRotateManagerMinAngle;
        public float ContainerRotateManagerMaxAngle;
        public XRAxis ContainerRotateManagerAxis;
        //public GameObject ContainerRotateManager;
        //public List<RequirementD> UnlockRequirements = new List<RequirementD>();
        public List<ContainerRequirementD> LatchRequirements = new List<ContainerRequirementD>();
        public List<FPXRLatch<GRT, G, GI, PEW>> Latches = new List<FPXRLatch<GRT, G, GI, PEW>>();
        [SerializeField]
        protected float angleCheck;
        #region Delegates for Container Events
        public delegate void ContainerDelegate();
        public ContainerDelegate OnContainerUnlock;
        public ContainerDelegate OnContainerLock;
        //public ContainerDelegate OnContainerSingleRequirementMet;

        #endregion
        //need status of container, if open or closed
        public UnityEvent OnUnlockedEvent;
        public UnityEvent OnLockedEvent;
        public UnityEvent PartiallyLockedEvent;
        [Tooltip("Single Fire Event on First Open")]
        public UnityEvent OnFirstOpenContainer;
        protected bool firstOpen = false;
        public UnityEvent OnClosedContainer;
        public virtual void OnEnable()
        {
            for (int i = 0; i < Latches.Count; i++)
            {
                Latches[i].OnLatchStateOpened += LatchStateOpened;
                Latches[i].OnLatchStateClosed += LatchStateClosed;
            }
        }
        public virtual void OnDisable()
        {
            for (int i = 0; i < Latches.Count; i++)
            {
                Latches[i].OnLatchStateOpened -= LatchStateOpened;
                Latches[i].OnLatchStateClosed -= LatchStateClosed;
            }
        }
        /// <summary>
        /// Check if we have met all the requirements to unlock this container
        /// Look at the UnlockRequirements list and check if all requirements are met
        /// </summary>
        public virtual void LatchStateOpened(ContainerRequirementD req)
        {
            //are all latches open?
            int latchesOpen = 0;
            for (int i = 0; i < LatchRequirements.Count; i++)
            {
                var aLatch = LatchRequirements[i];
                if (aLatch.RequirementTag == req.RequirementTag)
                {
                    //just need to change the state of the latch
                    aLatch.LatchState = req.LatchState;
                    LatchRequirements[i] = aLatch;
                    //OnContainerSingleRequirementMet?.Invoke();
                }
                //now check if the latch is open and increase our count
                if (aLatch.LatchState == LatchState.Open)
                {
                    latchesOpen++;
                }
            }
            if (latchesOpen == LatchRequirements.Count)
            {
                //all are open!
                //are we currently locked? if we are then we can unlock
                if (ContainerStatus == SequenceStatus.Locked)
                {
                    ContainerStatus = SequenceStatus.Unlocked;
                    OnContainerUnlock?.Invoke();
                    OnUnlockedEvent.Invoke();
                }
            }
        }
        public virtual void LatchStateClosed(ContainerRequirementD req)
        {
            //are all latches closed?
            int latchesClosed = 0;
            for (int i = 0; i < LatchRequirements.Count; i++)
            {
                var aLatch = LatchRequirements[i];
                if (aLatch.RequirementTag == req.RequirementTag)
                {
                    //just need to change the state of the latch
                    aLatch.LatchState = req.LatchState;
                    LatchRequirements[i] = aLatch;
                    //OnContainerSingleRequirementMet?.Invoke();
                }
                //now check if the latch is open and increase our count
                if (aLatch.LatchState == LatchState.Closed)
                {
                    latchesClosed++;
                }
            }
            if (latchesClosed == LatchRequirements.Count)
            {
                //all are open!
                //are we currently locked? if we are then we can unlock
                if (ContainerStatus == SequenceStatus.Unlocked)
                {
                    ContainerStatus = SequenceStatus.Locked;
                    OnContainerLock?.Invoke();
                    OnLockedEvent.Invoke();
                }
            }
            else
            {
                //do we have one of our latches closed?
                if (latchesClosed > 0)
                {
                    if (ContainerStatus == SequenceStatus.Unlocked)
                    {
                        //ContainerStatus = SequenceStatus.Locked;
                        OnContainerLock?.Invoke();
                        PartiallyLockedEvent.Invoke();
                    }
                }
            }
        }
        public virtual void UpdateContainerManagerAxis(int someEnum)
        {
            //0  = up, 1 = right, 2 = forward
            ContainerRotateManagerAxis = (XRAxis)someEnum;
        }
        public virtual void UpdateContainerManagerMinMaxAngleValues(float min, float max)
        {
            ContainerRotateManagerMinAngle= min;
            ContainerRotateManagerMaxAngle = max;
        }
        /// <summary>
        /// Use the Event Wrapper to call this function but only if we are moving the lid and unlocked
        /// </summary>
        public virtual void MoveContainerCheckLatches()
        {
            Debug.LogWarning($"Moving Container!!!");
            angleCheck = 0;
            switch (ContainerRotateManagerAxis)
            {
                case XRAxis.Up:
                    angleCheck = ContainerRotateManager.transform.localEulerAngles.y;
                    break;
                case XRAxis.Forward:
                    angleCheck = ContainerRotateManager.transform.localEulerAngles.z;
                    break;
                case XRAxis.Right:
                    angleCheck = ContainerRotateManager.transform.localEulerAngles.x;
                    break;
            }
            //states
            var minValue = ContainerRotateManagerMinAngle;
            var minValueOffset = minValue * 0.1f;
            if (angleCheck > minValue + minValueOffset)
            {
                //open
                ContainerRotaterStatus = RotaterStatus.Open;
                //make sure our latches stay disabled
                for (int i = 0; i < Latches.Count; i++)
                {
                    Latches[i].LatchManager.enabled = false;
                    Latches[i].LatchGrabber.enabled = false;
                    Latches[i].LatchWrapper.enabled = false;
                    Latches[i].LatchInteractor.enabled = false;
                }
                if (!firstOpen)
                {
                    OnFirstOpenContainer.Invoke();
                    firstOpen = true;
                }
            }
            else
            {
                //closed
                ContainerRotaterStatus = RotaterStatus.Closed;
                //make sure our latches are enabled
                for (int i = 0; i < Latches.Count; i++)
                {
                    Latches[i].LatchManager.enabled = true;
                    Latches[i].LatchGrabber.enabled = true;
                    Latches[i].LatchWrapper.enabled = true;
                    Latches[i].LatchInteractor.enabled = true;
                }
                OnClosedContainer.Invoke();

            }
        }
    }
}
