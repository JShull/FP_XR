namespace FuzzPhyte.XR
{
    using UnityEngine;

    public class FPPinDropSpline
    {
        private LineRenderer _lineRenderer;
        private int pCount;
        private float arcHeight;
        public FPPinDropSpline(LineRenderer theLineRenderer, int ptCount, float arcMax)
        {
            _lineRenderer = theLineRenderer;
            pCount = ptCount;
            arcHeight = arcMax;
            _lineRenderer.positionCount = pCount;
        }

        public void UpdateSpline(Vector3 start, Vector3 end,float arcMax)
        {
            arcHeight=arcMax;
            Vector3[] splinePoints = new Vector3[pCount];
            var controlPoint = GetControlPoint(start, end);
            for (int i = 0; i < pCount; i++)
            {
                float t = i / (float)(pCount - 1);
                splinePoints[i] = CalculateBezierPoint(t, start, controlPoint, end);
            }
            _lineRenderer.SetPositions(splinePoints);
            
        }

        Vector3 GetControlPoint(Vector3 start, Vector3 end)
        {
            // Calculate the midpoint between start and end
            Vector3 midPoint = (start + end) / 2;
            // Raise the midpoint up to create the arc effect
            midPoint += Vector3.up * arcHeight;
            return midPoint;
        }

        Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // Quadratic Bezier curve formula
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
            return point;
        }
    }
}
