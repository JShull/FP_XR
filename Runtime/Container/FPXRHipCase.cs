namespace FuzzPhyte.XR
{
    using System.Collections.Generic;
    using UnityEngine;
    [ExecuteInEditMode]
    public class FPXRHipCase : FPXRCase
    {
        //0.358f 14.1 to 16.1
        public Transform centerPoint; // Center reference point for the arc
        [SerializeField]protected Vector3 leftPosition;
        [SerializeField]protected Vector3 rightPosition;
        public Vector3[] socketPositions; // Array of socket positions along the arc
        public Vector3[] arcPoints;

        [Range(0.25f, 0.55f)]
        public float hipWidth = 0.358f; // Distance from the center to each hip point (half the total span)

        [Range(-1f, 1f)]
        public float middleZOffset = 0; // Controls forward/backward offset for middle point

        [Range(-1f, 1f)]
        public float arcYPositionOffset = 0; // Moves the arc up or down on the local y-axis

        [Range(0f, 1f)]
        public float padding = 0.1f; // Padding between the left and right positions

        [Range(-0.2f, 0.2f)]
        public float socketZOffset = 0.05f; // Local Z offset for each socket point based on center point
        public int numSockets = 5; // Number of sockets to represent along the arc
        [SerializeField]protected Vector3 effectiveLeft;
        [SerializeField]protected Vector3 effectiveRight;
        [SerializeField]protected Vector3 middlePosition;
        [SerializeField]protected List<FPSocket> sockets = new List<FPSocket>();
        
        public void OnEnable()
        {
            Setup();
        }
        public void OnDisable()
        {
            sockets.Clear();
        }
        public void Setup()
        {
            if (centerPoint == null)
            {
                Debug.LogWarning("Please assign the center point.");
                return;
            }
            var potentialSocketCount = centerPoint.childCount;
            int childWhoAreSockets =0;
            for(int i = 0; i < potentialSocketCount; i++)
            {
                if(centerPoint.GetChild(i).GetComponent<FPSocket>() != null)
                {
                    //centerPoint.GetChild(i).GetComponent<FPSocket>().PositionInCase(i,);
                    childWhoAreSockets++;
                }
            }
            sockets.Clear();
            if(childWhoAreSockets>0)
            {
                numSockets = childWhoAreSockets;
                socketPositions = new Vector3[numSockets];
                arcPoints = new Vector3[numSockets+2];
                CalculateSocketPositions();
                for(int i=0;i<numSockets;i++)
                {
                    centerPoint.GetChild(i).GetComponent<FPSocket>().PositionInCase(i,socketPositions[i]);
                    sockets.Add(centerPoint.GetChild(i).GetComponent<FPSocket>());
                }
            }else
            {
                socketPositions = new Vector3[numSockets];
                arcPoints = new Vector3[numSockets+2];
            }
        }
        public void Update()
        {
            if (centerPoint == null)
            {
                Debug.LogWarning("Please assign the center point.");
                return;
            }
            CalculateSocketPositions();
            AdjustPositions();
        }
        private void OnDrawGizmos()
        {
            if (centerPoint == null)
            {
                Debug.LogWarning("Please assign the center point.");
                return;
            }

            // Draw left, right, and middle points
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(leftPosition, 0.01f);
            Gizmos.DrawSphere(rightPosition, 0.01f);
            Gizmos.DrawSphere(middlePosition, 0.025f);
            // Draw arc and socket points
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(leftPosition, arcPoints[1]);
            Gizmos.DrawSphere(effectiveRight, 0.005f);
            Gizmos.DrawSphere(effectiveLeft, 0.005f);
            for (int i = 0; i < socketPositions.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(arcPoints[i+1], socketPositions[i]);
                Gizmos.color = Color.cyan;
                if(i!=socketPositions.Length-1){
                    Gizmos.DrawLine(arcPoints[i+1],arcPoints[i+2]);
                }
                // Draw each socket point along the arc
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(socketPositions[i], 0.05f);
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(arcPoints[arcPoints.Length-2], rightPosition);
        }
        protected void CalculateSocketPositions()
        {
            if (centerPoint == null)
            {
                Debug.LogWarning("Please assign the center point.");
                return;
            }
            // Calculate left and right positions based on the center point and width
            leftPosition = centerPoint.position - centerPoint.right * (hipWidth * 0.5f);
            rightPosition = centerPoint.position + centerPoint.right * (hipWidth * 0.5f);
            //LeftHipVisual.position = leftPosition;
            //RightHipVisual.position = rightPosition;

            // Calculate the middle position with padding, z offset, and y offset
            middlePosition = Vector3.Lerp(leftPosition, rightPosition, 0.5f);
            Vector3 localZDirection = centerPoint.forward.normalized;
            middlePosition += localZDirection * middleZOffset*0.5f + centerPoint.up * arcYPositionOffset;

            // Adjust left and right positions inward along the arc based on padding
            effectiveLeft = CalculateArcPoint(leftPosition, middlePosition, rightPosition, padding);
            effectiveRight = CalculateArcPoint(leftPosition, middlePosition, rightPosition, 1 - padding);
            arcPoints[0] = effectiveLeft;
            arcPoints[arcPoints.Length-1] = effectiveRight;
            for (int i = 1; i <= numSockets; i++)
            {
                float t = padding + (i / (float)(numSockets + 1)) * (1 - 2 * padding); // Normalized position within effective arc
                Vector3 pointOnArc = CalculateArcPoint(leftPosition, middlePosition, rightPosition, t);
                //
                //Vector3 pointOnArc = CalculateArcPoint(leftPosition, middlePosition, rightPosition, t);
                // Calculate the local Z offset based on the vector from center point to the socket position
                Vector3 directionFromCenter = (pointOnArc - centerPoint.position).normalized;
                Vector3 socketPositionWithZOffset = pointOnArc + directionFromCenter * socketZOffset;
                socketPositions[i-1] = socketPositionWithZOffset;
                arcPoints[i] = pointOnArc;
            }
        }
        protected virtual void AdjustPositions()
        {
            if(socketPositions.Length-2 == sockets.Count)
            {
                Debug.LogError($"Socket count is not equal to the socket positions count. Socket count: {sockets.Count} Socket positions count: {socketPositions.Length}");
                return;
            }
            for(int i=0;i<sockets.Count;i++)
            {
                sockets[i].PositionInCase(i,socketPositions[i]);
            }
        }
        private Vector3 CalculateArcPoint(Vector3 left, Vector3 middle, Vector3 right, float t)
        {
            // Quadratic formula for a point on the arc
            Vector3 pointOnArc = (1 - t) * (1 - t) * left + 2 * (1 - t) * t * middle + t * t * right;
            return pointOnArc;
        }
    }
}
