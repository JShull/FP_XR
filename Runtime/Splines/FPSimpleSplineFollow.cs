namespace FuzzPhyte.XR
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Splines;
    using Unity.Mathematics;
    public class FPSimpleSplineFollow : MonoBehaviour
    {
        [Header("Spline Settings")]
        public GameObject TheTarget;
        public SplineContainer SplineContainer;
        public int splineIndex = 0;
        [Tooltip("The count of segments to go through on the spline")]
        public int SplineRange = 1;
        public bool AllRange = false;
        public bool OnStartActive = true;
        [Header("Movement Settings")]
        public float speed = 0.1f; // Fraction of spline per second
        public Vector3 offsetFromPath = Vector3.zero;
        protected Vector3 previousPosition;
        protected Vector3 nextPosition;
        protected float previousT;
        protected float nextT;
        protected float fixedDeltaTimer = 0f;
        protected bool isActive = false;
        protected SplinePath path;
        protected float t = 0f;

        protected virtual void Start()
        {
            if (SplineContainer == null || SplineContainer.Splines.Count == 0)
            {
                Debug.LogError("SplineContainer is missing or has no splines.");
                enabled = false;
                return;
            }

            if (splineIndex >= SplineContainer.Splines.Count)
            {
                Debug.LogWarning($"Spline index {splineIndex} is out of range. Using 0 instead.");
                splineIndex = 0;
            }

            var spline = SplineContainer.Splines[splineIndex];
            var transformMatrix = SplineContainer.transform.localToWorldMatrix;
            var numKnots = spline.Knots.ToList().Count;
            if(AllRange)
            {
                SplineRange = numKnots;
                Debug.LogWarning($"Using all range on count, entire spline in play: {SplineRange}");
            }
            path = new SplinePath(new[]
            {
                new SplineSlice<Spline>(spline, new SplineRange(0, SplineRange), transformMatrix)
            });
            if (OnStartActive)
            {
                isActive = true;
            }
        }

        protected virtual void Update()
        {
            if (!isActive)
            {
                return;
            }

            // Accumulate time to get interpolation factor
            fixedDeltaTimer += Time.deltaTime;
            float interp = Mathf.Clamp01(fixedDeltaTimer / Time.fixedDeltaTime);
            TheTarget.transform.position = Vector3.Lerp(previousPosition, nextPosition, interp);
        }
        protected virtual void FixedUpdate()
        {
            if(!isActive)
            {
                return;
            }
            // Store current frame values
            previousT = t;
            previousPosition = path.EvaluatePosition(previousT) + (float3)offsetFromPath;

            // Advance t
            t += speed * Time.fixedDeltaTime;
            if (t > 1f)
            {
                t = 0f;
            }
            // Calculate next frame values
            nextT = t;
            nextPosition = path.EvaluatePosition(nextT) + (float3)offsetFromPath;

            // Reset timer
            fixedDeltaTimer = 0f;
        }
        public virtual void StopMotion()
        {
            isActive = false;
        }
        public virtual void StartMotion()
        {
            isActive = true;
        }
    }
}
