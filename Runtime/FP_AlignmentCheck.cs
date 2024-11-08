namespace FuzzPhyte.XR
{
    using UnityEngine;
    /// <summary>
    /// Base Alignment script for checking if another object is aligned/facing with the TransformItem.
    /// </summary>
    public class FP_AlignmentCheck : MonoBehaviour
    {
        [Header("Alignment Settings")]
        [Tooltip("Item we are working with")]
        public Transform TransformItem;
        [Space]
        [Header("Debug")]
        [SerializeField]
        protected Vector3 forwardA;
        [SerializeField]
        protected Vector3 forwardB;
        [SerializeField]
        protected float dotProduct;
        [SerializeField]
        protected Vector3 crossProduct;
        [SerializeField]
        protected bool isAlignedForward;
        [SerializeField]
        protected bool isAlignedBackward;
        [SerializeField]
        protected bool isForwardAligned;
        
        [Range(0.025f,0.95f)]
        public float marginOfError = 0.1f; // The acceptable margin of error for alignment (0 to 1)
        public Vector3 worldUp = Vector3.up; // The reference "World Up" vector
        [SerializeField]
        protected Transform lastCheckedAlignment;
        /// <summary>
        /// Checks if the object is aligned with the target within the margin of error.
        /// </summary>
        /// <returns>True if aligned within the margin, otherwise false.</returns>
        public virtual bool IsAligned(Transform targetTransform)
        {
            if (targetTransform == null || TransformItem == null)
            {
                Debug.LogError($"Target/Transform is not set.");
                return false;
            }
            lastCheckedAlignment = targetTransform;
            // Get the local forward vectors of both transforms
            forwardA = TransformItem.forward.normalized;
            forwardB = lastCheckedAlignment.forward.normalized;

            // Calculate the dot product between the two forward vectors
            dotProduct = Vector3.Dot(forwardA, forwardB);

            // Determine if the forward vectors are aligned within the margin of error
            isForwardAligned = Mathf.Abs(dotProduct) > 1f - marginOfError;
            return isForwardAligned;
        }
        public virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (lastCheckedAlignment != null&&UnityEditor.Selection.activeGameObject==this.gameObject)
            {
                // Draw forward vectors for visualization
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(TransformItem.position, TransformItem.position + TransformItem.forward * 2f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(lastCheckedAlignment.position, lastCheckedAlignment.position + lastCheckedAlignment.forward * 2f);
            }
#endif
        }
    }
}
