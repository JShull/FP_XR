namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using FuzzPhyte.Utility.Meta;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Base class for other XR related sub libraries like OVR to derive from...
    /// This comes in and should pickup where we previously made OVRTool...
    /// </summary>
    public abstract class FPXRTool : MonoBehaviour
    {
        #region FP Related
        public FP_Tag FPTag;
        public FP_Data FPData;
        #endregion
        [SerializeField] protected Rigidbody mainRigidbody;
        protected float lastUseTime;
        [SerializeField] protected AnimationCurve strengthCurve;
        protected float dampedUseStrength;
        [SerializeField]
        protected float triggerSpeed;
        [Space]
        public bool UseDelayStartEvent;
        public float DelayBeforeUse;
        [Tooltip("Debugging purposes on exposure")]
        [SerializeField] protected bool toolInUse;
        [SerializeField] protected bool toolInHand;
        public bool ToolInUse { get { return toolInUse; } }
        public bool ToolInHand { get { return toolInHand; } }
        
        #region Generic Tool Events
        public UnityEvent BeginUseEvent;
        public UnityEvent EndUseEvent;
        public UnityEvent DuringUseDelayStartEvent;
        public UnityEvent ToolAutomaticallyReturnedMenu;
        #endregion
        #region Various Functions needed for derived classes
        public virtual void BeginUse()
        {
            lastUseTime = Time.realtimeSinceStartup;
            BeginUseEvent.Invoke();
            toolInUse = true;
            toolInHand = true; //just making sure
            if (UseDelayStartEvent)
            {
                StartCoroutine(DelayDuringUse(DelayBeforeUse));
            }
        }
        public virtual void EndUse()
        {
            EndUseEvent.Invoke();
            toolInUse = false;
        }
        public virtual float ComputeUseStrength(float strength)
        {
            float delta = Time.realtimeSinceStartup - lastUseTime;
            lastUseTime = Time.realtimeSinceStartup;
            if (strength > dampedUseStrength)
            {
                dampedUseStrength = Mathf.Lerp(dampedUseStrength, strength, triggerSpeed * delta);
            }
            else
            {
                dampedUseStrength = strength;
            }
            float progress = strengthCurve.Evaluate(dampedUseStrength);
            return progress;
        }
        #endregion
        protected virtual IEnumerator DelayDuringUse(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            if (toolInUse)
            {
                DuringUseDelayStartEvent.Invoke();
            }
        }
        /// <summary>
        /// Turn off Kinematic properties from an external source like a menu
        /// </summary>
        public virtual void ResetKinematics(bool kinematicProperty)
        {
            if (mainRigidbody != null)
            {
                mainRigidbody.isKinematic = kinematicProperty;
            }
        }
        /// <summary>
         /// Turn off Kinematic properties from an external source like a menu/OVR Interactor
         /// </summary>
        public virtual void ResetKinematicsDelay(bool kinematicProperty)
        {
            StartCoroutine(DelayKinematic(kinematicProperty));
        }
        protected virtual IEnumerator DelayKinematic(bool value)
        {
            yield return new WaitForFixedUpdate();
            if (mainRigidbody != null)
            {
                mainRigidbody.isKinematic = value;
            }
        }
        public virtual void ToolPickedUp()
        {
            toolInHand = true;
        }
        public virtual void ToolDropped()
        {
            toolInHand = false;
        }
        public virtual void ToolAutoBackInMenu()
        {
            ToolAutomaticallyReturnedMenu.Invoke();
        }
    }
}
