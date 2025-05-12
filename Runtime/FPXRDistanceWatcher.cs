namespace FuzzPhyte.XR
{
    using UnityEngine;
    using UnityEngine.Events;
    public class FPXRDistanceWatcher : MonoBehaviour
    {
        [Header("Transforms to Monitor")]
        public Transform SourceObject;
        public Transform TargetObject;

        [Header("Distance Settings")]
        public float DistanceThreshold = 5f;
        public bool UseLateUpdate = true;

        [Header("Events")]
        public UnityEvent OnDistanceExceeded;
        [Tooltip("Keep tabs to make sure we only fire off the event once")]
        [SerializeField]protected bool hasFired;

        protected virtual void OnEnable()
        {
            hasFired = false;
        }
        protected virtual void OnDisable()
        {
            hasFired = false;
        }

        protected virtual void Update()
        {
            if (!UseLateUpdate)
            {
                CheckDistance();
            }
        }

        protected virtual void LateUpdate()
        {
            if (UseLateUpdate)
            {
                CheckDistance();
            }
        }

        public virtual void CheckDistance()
        {
            if (SourceObject == null || TargetObject == null) return;

            float distance = Vector3.Distance(SourceObject.position, TargetObject.position);
            if (distance > DistanceThreshold)
            {
                if (!hasFired)
                {
                    OnDistanceExceeded?.Invoke();
                    hasFired = true;
                }
            }
            else
            {
                hasFired = false;
            }
        }
    }
}
