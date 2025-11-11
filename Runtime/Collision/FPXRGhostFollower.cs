namespace FuzzPhyte.XR
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class FPXRGhostFollower : MonoBehaviour
    {
        [Header("Follow Target")]
        public Transform Target;

        [Header("Tuning")]
        [Tooltip("Max position delta per FixedUpdate before we 'snap' (avoids tunnel/lag if tracking jumps).")]
        public float maxTeleportDistance = 0.25f;

        [Tooltip("Optional positional smoothing (0 = off, 1 = heavy smoothing).")]
        [Range(0f, 1f)] public float positionSmoothing = 0.15f;

        [Tooltip("Optional rotation smoothing (0 = off, 1 = heavy smoothing).")]
        [Range(0f, 1f)] public float rotationSmoothing = 0.1f;

        [Tooltip("Clamp for linear speed to avoid crazy values on tracking spikes.")]
        public float maxLinearSpeed = 35f;

        [Tooltip("Clamp for angular speed (radians/sec).")]
        public float maxAngularSpeed = 75f;

        public Rigidbody rb;
        protected Vector3 _lastTargetPos;
        protected Quaternion _lastTargetRot;
        protected bool _haveHistory;

        public virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            // Rigidbody recommendations for a velocity-driven follower:
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public virtual void OnEnable()
        {
            _haveHistory = false;
        }

        public virtual void FixedUpdate()
        {
            if (Target == null) return;

            float dt = Time.fixedDeltaTime;
            Vector3 targetPos = Target.position;
            Quaternion targetRot = Target.rotation;

            if (!_haveHistory)
            {
                _lastTargetPos = targetPos;
                _lastTargetRot = targetRot;
                rb.position = targetPos;
                rb.rotation = targetRot;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                _haveHistory = true;
                return;
            }

            // Compute desired linear velocity from target delta.
            Vector3 desiredVel = (targetPos - rb.position) / dt;

            // If tracking jumped too far, snap to avoid huge impulses.
            float distToTargetNow = Vector3.Distance(rb.position, targetPos);
            if (distToTargetNow > maxTeleportDistance)
            {
                rb.position = targetPos;
                rb.rotation = targetRot;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                _lastTargetPos = targetPos;
                _lastTargetRot = targetRot;
                return;
            }

            // Optional smoothing (reduce jitter/aliasing between Update/FixedUpdate rates).
            if (positionSmoothing > 0f)
            {
                Vector3 directVel = (targetPos - _lastTargetPos) / dt; // 'raw' controller motion
                desiredVel = Vector3.Lerp(directVel, desiredVel, positionSmoothing);
            }

            // Clamp crazy spikes
            if (desiredVel.sqrMagnitude > maxLinearSpeed * maxLinearSpeed)
            {
                desiredVel = desiredVel.normalized * maxLinearSpeed;
            }

            // Angular velocity from quaternion delta
            Quaternion deltaRot = targetRot * Quaternion.Inverse(rb.rotation);
            deltaRot.ToAngleAxis(out float angleDeg, out Vector3 axis);
            if (float.IsNaN(axis.x))
            {
                axis = Vector3.zero;
                angleDeg = 0f;
            }
            // Map angle to [-180, 180]
            if (angleDeg > 180f) angleDeg -= 360f;
            Vector3 desiredAngVel = axis * (angleDeg * Mathf.Deg2Rad) / dt;

            // Optional smoothing for rotation
            if (rotationSmoothing > 0f)
            {
                // Estimate 'raw' ang vel from last target delta as well
                Quaternion rawDelta = targetRot * Quaternion.Inverse(_lastTargetRot);
                rawDelta.ToAngleAxis(out float rawDeg, out Vector3 rawAxis);
                if (rawDeg > 180f) rawDeg -= 360f;
                Vector3 rawAngVel = rawAxis * (rawDeg * Mathf.Deg2Rad) / dt;
                desiredAngVel = Vector3.Lerp(rawAngVel, desiredAngVel, rotationSmoothing);
            }

            // Clamp angular spikes
            float angMag = desiredAngVel.magnitude;
            if (angMag > maxAngularSpeed)
                desiredAngVel = desiredAngVel * (maxAngularSpeed / Mathf.Max(angMag, 1e-4f));

            // Drive the body using velocities so collisions have meaningful relativeVelocity.
            rb.linearVelocity = desiredVel;
            rb.angularVelocity = desiredAngVel;

            _lastTargetPos = targetPos;
            _lastTargetRot = targetRot;
        }
    }
}
