namespace FuzzPhyte.XR
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Splines;
    public class FPSimpleSplineFollow : MonoBehaviour
    {
        [Header("Spline Settings")]
        public GameObject npc;
        public SplineContainer splineContainer;
        public int splineIndex = 0;
        [Tooltip("The count of segments to go through on the spline")]
        public int splineRange = 1;
        public bool AllRange = false;
        [Header("Movement Settings")]
        public float speed = 0.1f; // Fraction of spline per second
        public Vector3 offsetFromPath = Vector3.zero;

        protected SplinePath path;
        private float t = 0f;

        void Start()
        {
            if (splineContainer == null || splineContainer.Splines.Count == 0)
            {
                Debug.LogError("SplineContainer is missing or has no splines.");
                enabled = false;
                return;
            }

            if (splineIndex >= splineContainer.Splines.Count)
            {
                Debug.LogWarning($"Spline index {splineIndex} is out of range. Using 0 instead.");
                splineIndex = 0;
            }

            var spline = splineContainer.Splines[splineIndex];
            var transformMatrix = splineContainer.transform.localToWorldMatrix;
            var numKnots = spline.Knots.ToList().Count;
            if(AllRange)
            {
                splineRange = numKnots - 1;
                Debug.LogWarning($"Using all range on count, entire spline in play: {splineRange}");
            }
            path = new SplinePath(new[]
            {
                new SplineSlice<Spline>(spline, new SplineRange(0, splineRange), transformMatrix)
            });
        }

        void Update()
        {
            // Move along the path
            Vector3 position = path.EvaluatePosition(t);
            npc.transform.position = position + offsetFromPath;

            t += speed * Time.deltaTime;
            if (t > 1f) t = 0f;
        }
    }
}
