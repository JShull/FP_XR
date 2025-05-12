namespace FuzzPhyte.XR
{
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine;
    using FuzzPhyte.Utility;
    using TMPro;
    public class FPPhysicalButton : MonoBehaviour,IFPOnStartSetup
    {
        [Header("Key Parameters")]
        public FPPhysicalKeyData KeyData;
        protected char fPKey;
        public char FPKey { get => fPKey; }
        protected char fPKeyShift;
        public char FPKeyShift { get => fPKeyShift; }
        protected FPKeyboardKey keyType=FPKeyboardKey.None;
        public FPKeyboardKey KeyType { get=>keyType; }
        [Tooltip("The transform we are moving")]
        public Transform FPButton;
        public Collider FPCollider;
        public Transform FPVisual;
        public TextMeshPro FPTextVisual;
        public TextMeshPro FPTextVisualSecondary;
        [SerializeField]protected Vector3 thePhysicalKeySize = new Vector3(0.05f,0.05f,0.05f);
        [Space]
        [SerializeField] protected bool onStartSetup = true;
        protected bool setupComplete = false;
        public bool KeyActive = false;
        [SerializeField] protected Vector3 localPressDirection = Vector3.down;
        [SerializeField] protected float MaxDistance = -0.075f;
        [SerializeField] protected float ReturnSpeed = 10f;
        [SerializeField] protected float DistanceThreshold = 0.001f;
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
        public bool SetupStart { get => onStartSetup; set => onStartSetup=value; }

        [SerializeField] protected Rigidbody rb;
        protected Coroutine moveCoroutine;
        [Header("Editor Based")]
        [SerializeField] protected float visSize=0.01f;
        [SerializeField] protected float clampedDot;
        [SerializeField] protected float maxDot;
        public virtual void Awake() 
        {
            
        }
        public virtual void Start()
        {
            if (SetupStart && KeyData!=null)
            {
                fPKey = KeyData.TheFPKey;
                fPKeyShift = KeyData.TheFPKeyShift;
                keyType = KeyData.TheKeyType;
                if (keyType == FPKeyboardKey.RegularKey)
                {
                    FPTextVisual.text = fPKeyShift.ToString();
                }
                if (keyType == FPKeyboardKey.NumericalKey)
                {
                    FPTextVisual.text = fPKey.ToString();
                    FPTextVisualSecondary.text = fPKeyShift.ToString();
                }
                if (keyType == FPKeyboardKey.PunctuationKey)
                {
                    FPTextVisual.text = fPKey.ToString();
                    FPTextVisualSecondary.text = fPKeyShift.ToString();
                }

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
                setupComplete = true;
                KeyActive = true;
                if (keyType == FPKeyboardKey.RegularKey || keyType == FPKeyboardKey.NumericalKey)
                {
                    this.gameObject.name += "_" + fPKey;
                    FPTextVisual.gameObject.name += "_" + fPKey;
                }
                else 
                {
                    if(KeyType== FPKeyboardKey.PunctuationKey)
                    {
                        this.gameObject.name += "_" + fPKey + "_" + fPKeyShift;
                    }
                    else
                    {
                        this.gameObject.name += "_" + keyType.ToString();
                    }
                        
                }
            }
            else
            {
                //FPButton.gameObject.SetActive(false);
            }
            
        }
        public virtual void SetupKey(
            Vector3 keySize, FPKeyboardKey passedKeyType, char keyValue, 
            char keyShiftValue, float distanceT, float maxDT, 
            float pressSpeed, bool useCollision=true)
        {
            MaxDistance = maxDT;
            ReturnSpeed= pressSpeed;
            DistanceThreshold= distanceT;
            fPKey = keyValue;
            fPKeyShift = keyShiftValue;
            keyType = passedKeyType;
            thePhysicalKeySize = keySize;
            if (keyType == FPKeyboardKey.RegularKey)
            {
                FPTextVisual.text = fPKeyShift.ToString();
            }
            if(keyType == FPKeyboardKey.NumericalKey)
            {
                FPTextVisual.text = fPKey.ToString();
            }
            if (keyType == FPKeyboardKey.PunctuationKey)
            {
                FPTextVisual.text = fPKey.ToString();
                FPTextVisualSecondary.text = fPKeyShift.ToString();
            }
            if (FPCollider != null)
            {
                var bcCollider = FPCollider as BoxCollider;
                if (bcCollider != null)
                {
                    bcCollider.size = keySize;
                    bcCollider.center = new Vector3(0, keySize.y * -0.5f, 0);
                }
                var collisionSystem = FPCollider.gameObject.GetComponent<FPPhysicalButton>();
                if (collisionSystem!=null)
                {
                    collisionSystem.enabled = useCollision;
                }
            }
            if (FPVisual!=null)
            {
                FPVisual.localScale = keySize;
                FPVisual.localPosition = new Vector3(0, keySize.y * -0.5f, 0);
            }
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
            if (keyType == FPKeyboardKey.RegularKey || keyType == FPKeyboardKey.NumericalKey)
            {
                this.gameObject.name += "_" + fPKey;
                FPTextVisual.gameObject.name += "_" + fPKey;
            }
            else
            {
                if (KeyType == FPKeyboardKey.PunctuationKey)
                {
                    this.gameObject.name += "_" + fPKey + "_" + fPKeyShift;
                }
                else
                {
                    this.gameObject.name += "_" + keyType.ToString();
                }

            }
            //FPButton.gameObject.SetActive(true);
            setupComplete = true;
            KeyActive = true;
        }
        public void Pressed()
        {
            if (!setupComplete ||!KeyActive)
            {
                return;
            }
            IsPressed?.Invoke(this);
            UnityPressedEvent.Invoke();
        }
        public void Released()
        {
            if (!setupComplete|| !KeyActive)
            {
                return;
            }
            IsReleased?.Invoke(this);
            UnityReleasedEvent.Invoke();
        }
        public void MoveToPosition(Vector3 targetPosition,bool pressedDown)
        {
            if (!setupComplete||!KeyActive)
            {
                return;
            }
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
            if (setupComplete)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color= Color.red;
            }
            Gizmos.DrawWireCube(restWorldPos+new Vector3(0,thePhysicalKeySize.y*-0.5f,0), thePhysicalKeySize*1.05f);
            
        }
#endif
    }
}
