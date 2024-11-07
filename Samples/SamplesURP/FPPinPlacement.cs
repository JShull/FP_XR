namespace FuzzPhyte.XR
{
    using System.Collections;
    using UnityEngine;
    [RequireComponent(typeof(LineRenderer))]
    public class FPPinPlacement : MonoBehaviour
    {
        public GameObject PinPrefab;
        [Space]
        [Header("Parameters")]
        [SerializeField]protected GameObject pinHead;
        [SerializeField]protected GameObject pinTail;
        [SerializeField] protected LineRenderer lineR;
        public int NumberPointsOnLine = 50;
        public float MaxArcHeight = 5f;
        public float ThresholdDiff = 0.01f;
        public float StartWidth = 0.1f;
        public float EndWidth = 0.25f;
        protected FPPinDropSpline _pinDropSpline;
        protected Vector3 _lastPositionHead;
        protected Vector3 _lastPositionTail;
        protected float _lastArcHeight;
        protected Coroutine drawCoroutine;

        [Space]
        public bool PinPlaced;
        [SerializeField] private Transform pinHeadTracking;
        [SerializeField] private Transform pinTailTracking;

        public void Start()
        {
            lineR = GetComponent<LineRenderer>();
            lineR.startWidth = StartWidth;
            lineR.endWidth = EndWidth;
            _pinDropSpline = new FPPinDropSpline(lineR, NumberPointsOnLine, MaxArcHeight);
            pinHead = GameObject.Instantiate(PinPrefab, transform.position, Quaternion.identity);
            _lastArcHeight = MaxArcHeight;
            //tail is first child in hierarchy
            if (pinHead.transform.childCount > 1)
            {
                pinTail = pinHead.transform.GetChild(1).gameObject;
            }
            //turn it off
            pinHead.gameObject.SetActive(false);
            lineR.enabled = false;
        }
        /*
        public void Update()
        {
          
            if (PinPlaced)
            {
                pinHead.gameObject.SetActive(true);
                lineR.enabled = true;
            }
            else
            {
                pinHead.gameObject.SetActive(false);
                lineR.enabled = false;
                return;
            }

            if(Vector3.Distance(pinHead.transform.position, _lastPositionHead) > ThresholdDiff || Vector3.Distance(pinTail.transform.position, _lastPositionTail) > ThresholdDiff || Mathf.Abs(_lastArcHeight-MaxArcHeight)>ThresholdDiff)
            {
                _pinDropSpline.UpdateSpline(pinHead.transform.position, pinTail.transform.position, MaxArcHeight);
                _lastPositionHead = pinHead.transform.position;
                _lastPositionTail = pinTail.transform.position;
                _lastArcHeight = MaxArcHeight;
            }
            
        }
        */
        public void OnDisable()
        {
            if(drawCoroutine != null)
            {
                StopCoroutine(drawCoroutine);
            }
        }
        public void SetPinHeadTransformTracking(Transform pHead)
        {
            pinHeadTracking = pHead;
        }
        public void SetPinTailTransformTracking(Transform pTail)
        {
            pinTailTracking = pTail;
        }
        public void StartTrackingPin()
        {
            PinPlaced = true;
            if (drawCoroutine != null)
            {
                StopCoroutine(drawCoroutine); // Stop if already running
            }
            drawCoroutine = StartCoroutine(UpdatePinPlacements());
        }
        public void StopTrackingPin()
        {
            PinPlaced = false;
            pinHead.gameObject.SetActive(false);
            lineR.enabled = false;
        }
        protected IEnumerator UpdatePinPlacements()
        {
            pinHead.gameObject.SetActive(true);
            lineR.enabled = true;
            
            while (PinPlaced)
            {
                if (pinHeadTracking)
                {
                    SetPinHeadPosition(pinHeadTracking.position);
                }
                if (pinTailTracking)
                {
                    SetPinTailPosition(pinTailTracking.position);
                }
                if (Vector3.Distance(pinHead.transform.position, _lastPositionHead) > ThresholdDiff || Vector3.Distance(pinTail.transform.position, _lastPositionTail) > ThresholdDiff || Mathf.Abs(_lastArcHeight - MaxArcHeight) > ThresholdDiff)
                {
                    _pinDropSpline.UpdateSpline(pinHead.transform.position, pinTail.transform.position, MaxArcHeight);
                    _lastPositionHead = pinHead.transform.position;
                    _lastPositionTail = pinTail.transform.position;
                    _lastArcHeight = MaxArcHeight;
                }
                yield return new WaitForEndOfFrame();
            }
            drawCoroutine = null;
        }
        protected void SetPinHeadPosition(Vector3 position)
        {
            pinHead.transform.position = position;
        }
        protected void SetPinTailPosition(Vector3 position)
        {
            pinTail.transform.position = position;
        }
        /// <summary>
        /// Draw Gizmos to visualize the raycast in the Unity Editor
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (PinPlaced)
            {
                // Draw the ray line
                // Gizmos.color = Color.red;
                // Gizmos.DrawLine(pinSpawn.transform.position, pinTail.transform.position);

                // Draw spheres at the start and end points for clarity
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(pinHead.transform.position, 0.2f);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(pinTail.transform.position, 0.2f);
            }
        }
    }
}
