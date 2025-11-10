namespace FuzzPhyte.XR
{
    using UnityEngine;
    using System;
    using UnityEngine.Events;
    using FuzzPhyte.Utility;

    /// <summary>
    /// can extend this class for other event types
    /// this is mainly just looking for trigger enter / collision enter events
    /// </summary>
    public class FPXRPhysicsEvents : MonoBehaviour
    {
        [Header("Rigidbody Reference")]
        [SerializeField] protected Rigidbody _rigidbody;

        [Header("Impact Thresholds")]
        [SerializeField] private float lowToMediumThreshold = 2f;
        [SerializeField] private float mediumToHighThreshold = 5f;
        [SerializeField] private float minimumThreshold = 0.1f;

        [Header("Cooldown Between Impacts")]
        [SerializeField] private float timeBetweenCollisions = 0.2f;
        private float _lastImpactTime = 0f;

        [Header("Unity Events")]
        public UnityEvent OnLowImpact;
        public UnityEvent OnMediumImpact;
        public UnityEvent OnHighImpact;

        public event Action<Collision, FPImpactType> OnImpactEvent;
        public event Action<Collider, FPImpactType> OnTriggerEvent;
        protected bool triggerAction;
        protected FPXRPhysicsListener fPXRPhysicsListener;
        public bool UseStart = true;
        public virtual void Start()
        {
            if (UseStart)
            {
                Setup();
            }
        }
        
        public virtual void OnDestroy()
        {
            if (fPXRPhysicsListener != null)
            {
                fPXRPhysicsListener.OnCollisionEnterEvent -= FPXRCollisionEnter;
                fPXRPhysicsListener.OnTriggerEnterEvent -= FPXRTriggerEnter;
            }
        }
        /// <summary>
        /// Inject Rigidbody Reference, you should optionally consider running setup here
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="runSetup"></param>
        public virtual void InjectRigidbody(Rigidbody rigidbody,bool runSetup=false)
        {
            _rigidbody = rigidbody;
            if (runSetup)
            {
                Setup();
            } 
        }
        protected virtual void Setup()
        {
            if (_rigidbody == null)
            {
                Debug.LogError($"Missing Rigidbody reference on {gameObject.name}. Please assign a Rigidbody in the inspector.");
                return;
            }
            if (_rigidbody.gameObject.GetComponent<FPXRPhysicsListener>() == null)
            {
                //assign one
                fPXRPhysicsListener = _rigidbody.gameObject.AddComponent<FPXRPhysicsListener>();
            }
            else
            {
                fPXRPhysicsListener = _rigidbody.gameObject.GetComponent<FPXRPhysicsListener>();
            }
            fPXRPhysicsListener.UseCollisionEnterEvent = true;
            fPXRPhysicsListener.UseTriggerEnterEvent = true;
            fPXRPhysicsListener.OnCollisionEnterEvent += FPXRCollisionEnter;
            fPXRPhysicsListener.OnTriggerEnterEvent += FPXRTriggerEnter;
        }
        /// <summary>
        /// For Trigger Events
        /// </summary>
        /// <param name="collider"></param>
        protected virtual void FPXRTriggerEnter(Collider collider)
        {
            float timeSinceLastImpact = Time.time - _lastImpactTime;
            if (timeSinceLastImpact < timeBetweenCollisions) return;
            float magnitude = collider.attachedRigidbody.linearVelocity.magnitude;
            var impactDetails = MagnitudeCheck(magnitude);
            if (impactDetails.Item1)
            {
                OnTriggerEvent?.Invoke(collider, impactDetails.Item2);
            }

        }
        /// <summary>
        /// For Collision Events
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void FPXRCollisionEnter(Collision collision)
        {
            float timeSinceLastImpact = Time.time - _lastImpactTime;
            if (timeSinceLastImpact < timeBetweenCollisions) return;

            float magnitude = collision.relativeVelocity.magnitude;
            var impactDetails = MagnitudeCheck(magnitude);

            if (impactDetails.Item1)
            {
                OnImpactEvent?.Invoke(collision, impactDetails.Item2);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void FPXRCollisionExit(Collision collision)
        {
            // Handle collision exit if needed
        }
        /// <summary>
        /// Internal Magnitude Check
        /// </summary>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        protected (bool, FPImpactType) MagnitudeCheck(float magnitude)
        {
            if (magnitude < minimumThreshold) return (false,FPImpactType.NA);

            _lastImpactTime = Time.time;

            FPImpactType impactType;
            if (magnitude < lowToMediumThreshold)
            {
                impactType = FPImpactType.Low;
                OnLowImpact?.Invoke();
            }
            else if (magnitude < mediumToHighThreshold)
            {
                impactType = FPImpactType.Medium;
                OnMediumImpact?.Invoke();
            }
            else
            {
                impactType = FPImpactType.High;
                OnHighImpact?.Invoke();
            }
            return (true,impactType);
        }
    }
}
