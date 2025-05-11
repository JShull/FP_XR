namespace FuzzPhyte.XR
{
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine;
    using FuzzPhyte.Utility;
    public class FPPhysicalButton : MonoBehaviour
    {
        public char FPKey;
        public char FPKeyShift;
        public FPKeyboardKey KeyType=FPKeyboardKey.None;
        [Tooltip("The transform we are moving")]
        public Transform FPButton;
        public Vector3 localPressDirection = Vector3.down;
        public float MaxDistance = -0.075f;
        public float ReturnSpeed = 10f;
        public float DistanceThreshold = 0.001f;
        public float PressedTimeRepeat = 1f;
        [Range(0.01f,1f)]
        public float PressedTimeResetPercentage =0.75f;
        public UnityEvent UnityPressedEvent;
        public UnityEvent UnityHeldEvent;
        public UnityEvent UnityReleasedEvent;


        public delegate void FPhysicalButtonActions(FPPhysicalButton theButton);
        public event FPhysicalButtonActions IsPressed;
        public event FPhysicalButtonActions IsReleased;
        public event FPhysicalButtonActions IsHeldDown;
        [SerializeField] protected Vector3 restPosition;
        public Vector3 RestPosition { get=>restPosition; }
        [SerializeField] protected Vector3 maxLocalPosition;
        public Vector3 PushedPosition { get => maxLocalPosition; }
        [SerializeField] protected Rigidbody rb;
        protected Coroutine moveCoroutine;
        [Header("Editor Based")]
        [SerializeField] protected float visSize=0.01f;
        [SerializeField] protected float clampedDot;
        [SerializeField] protected float maxDot;
        public virtual void Awake() 
        {
            if(KeyType== FPKeyboardKey.RegularKey || KeyType == FPKeyboardKey.NumericalKey)
            {
                this.gameObject.name += "_" + FPKey;
            }
            else
            {
                this.gameObject.name += "_" + KeyType.ToString();
            }
        }
        public virtual void Start()
        {
            if (rb == null)
            {
                Debug.LogError($"Missing RB! Checking if it's on this item...");
                rb = GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.LogError($"Oh you really are missing the RB! - ERROR");
                    return;
                }
            }
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
            //rb.constraints
            restPosition = FPButton.localPosition;
            var worldPressDirection = FPButton.TransformDirection(localPressDirection).normalized;
            maxLocalPosition = restPosition - worldPressDirection * MaxDistance;
        }
        public void Pressed()
        {
            IsPressed?.Invoke(this);
            UnityPressedEvent.Invoke();
        }
        public void Released()
        {
            IsReleased?.Invoke(this);
            UnityReleasedEvent.Invoke();
        }
        public void MoveToPosition(Vector3 targetPosition,bool pressedDown)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(LerpToPosition(targetPosition, pressedDown));
        }

        IEnumerator LerpToPosition(Vector3 target,bool down)
        {
            float runTimeDown = 0;
            while (Vector3.Distance(FPButton.localPosition, target) > DistanceThreshold)
            {
                FPButton.localPosition = Vector3.Lerp(FPButton.localPosition, target, Time.deltaTime * ReturnSpeed);
                if (down)
                {
                    if (runTimeDown >= PressedTimeRepeat)
                    {
                        IsHeldDown?.Invoke(this);
                        UnityHeldEvent.Invoke();
                        runTimeDown = PressedTimeRepeat * PressedTimeResetPercentage;
                    }
                    runTimeDown += Time.deltaTime;
                }
                yield return null;
            }
            FPButton.localPosition = target;
            while (down)
            {
                if (runTimeDown >= PressedTimeRepeat)
                {
                    IsHeldDown?.Invoke(this);
                    UnityHeldEvent.Invoke();
                    runTimeDown = PressedTimeRepeat * PressedTimeResetPercentage;
                }
                runTimeDown += Time.deltaTime;
                
                yield return null;
            }
            
        }


#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (FPButton == null)
                return;

            Vector3 restWorldPos = FPButton.parent != null
                ? FPButton.parent.TransformPoint(restPosition)
                : FPButton.position;

            Vector3 pressDir = FPButton.TransformDirection(localPressDirection.normalized);
            Vector3 maxPressPos = restWorldPos + pressDir * -MaxDistance;

            // Draw rest position (green)
            Gizmos.color = Color.green;
            var sqGLines = FP_UtilityData.DrawGizmosSquare(restWorldPos, FPButton, visSize);
            foreach (var giz in sqGLines) 
            { 
                Gizmos.DrawLine(giz.StartPos, giz.EndPos);
            }
            // Draw max press position (red)
            Gizmos.color = Color.red;
            var sqGMaxLines = FP_UtilityData.DrawGizmosSquare(maxPressPos, FPButton, visSize);
            foreach (var giz in sqGMaxLines)
            {
                Gizmos.DrawLine(giz.StartPos, giz.EndPos);
            }
            
        }
#endif
    }
}
