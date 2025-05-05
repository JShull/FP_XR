namespace FuzzPhyte.XR
{
    using UnityEngine;

    /// <summary>
    /// Quick script to have a 'body' follow our head
    /// </summary>
    public class FPXRBodyFollow : MonoBehaviour
    {
        public Transform HeadTransform;
        [Tooltip("Offset from the HeadTransform pivot")]
        public Vector3 HeightAdjustment = new Vector3(0, -0.5f, 0);
        [Tooltip("The 'Body' to move")]
        public Transform ObjectToMove;
        [Tooltip("Amount to lerp between on our calculation")]
        [Range(0f,1f)]
        public float RotationLerpScale = 1;
        protected Quaternion nextRotation;
        protected Vector3 nextPos;
        protected bool setup;
        public virtual void Start()
        {
            if(ObjectToMove==null || HeadTransform == null)
            {
                Debug.LogError($"Missing object/head transform references");
                return;
            }
            setup = true;
        }
        public virtual void Update()
        {
            if (!setup)
            {
                return;
            }
            KeepTrack();
        }
        public virtual void LateUpdate()
        {
            if (!setup)
            {
                return;
            }
            ObjectToMove.position = nextPos;
            ObjectToMove.rotation = nextRotation;
        }
        protected virtual void KeepTrack()
        {
            nextPos = HeadTransform.position + HeightAdjustment;
            nextRotation = Quaternion.Lerp(ObjectToMove.rotation, HeadTransform.rotation, RotationLerpScale);
            var rotationNoX = nextRotation.eulerAngles;
            rotationNoX.x = 0;
            nextRotation = Quaternion.Euler(rotationNoX);
        }
    }
}
