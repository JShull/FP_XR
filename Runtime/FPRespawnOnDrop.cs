namespace FuzzPhyte.XR
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    public class FPRespawnOnDrop : MonoBehaviour
    {
        /// <summary>
        /// Respawn will happen when the transform moves below this World Y position.
        /// </summary>
        [SerializeField]
        [Tooltip("Respawn will happen when the transform moves below this World Y position.")]
        protected float _yThresholdForRespawn;

        /// <summary>
        /// UnityEvent triggered when a respawn occurs.
        /// </summary>
        [SerializeField]
        [Tooltip("UnityEvent triggered when a respawn occurs.")]
        protected UnityEvent _whenRespawned = new UnityEvent();

        /// <summary>
        /// If the transform has an associated rigidbody, make it kinematic during this
        /// number of frames after a respawn, in order to avoid ghost collisions.
        /// </summary>
        [SerializeField]
        [Tooltip("If the transform has an associated rigidbody, make it kinematic during this number of frames after a respawn, in order to avoid ghost collisions.")]
        protected int _sleepFrames = 0;

        public UnityEvent WhenRespawned => _whenRespawned;

        // cached starting transform
        protected Vector3 _initialPosition;
        protected Quaternion _initialRotation;
        protected Vector3 _initialScale;
        protected Transform snappedLocation;

        //private TwoGrabFreeTransformer[] _freeTransformers;
        protected Rigidbody _rigidBody;
        protected int _sleepCountDown;

        protected virtual void OnEnable()
        {
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _initialScale = transform.localScale;
            //OVR
            //_freeTransformers = GetComponents<TwoGrabFreeTransformer>();
            _rigidBody = GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            if (transform.position.y < _yThresholdForRespawn)
            {
                Respawn();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_sleepCountDown > 0)
            {
                if (--_sleepCountDown == 0)
                {
                    _rigidBody.isKinematic = false;
                }
            }
        }
        /// <summary>
        /// If we snapped to a location
        /// </summary>
        /// <param name="snapLocation"></param>
        public virtual void SnappedToLocation(Transform snapLocation)
        {
            snappedLocation = snapLocation;
        }
        public virtual void Respawn()
        {
            if (snappedLocation != null)
            {
                transform.position = snappedLocation.position;
                transform.rotation = snappedLocation.rotation;
                transform.localScale = _initialScale;
                if (_sleepFrames > 0)
                {
                    _sleepCountDown = _sleepFrames;
                }

            }
            else
            {
                transform.position = _initialPosition;
                transform.rotation = _initialRotation;
                transform.localScale = _initialScale;

                if (_rigidBody)
                {
                    _rigidBody.linearVelocity = Vector3.zero;
                    _rigidBody.angularVelocity = Vector3.zero;

                    if (!_rigidBody.isKinematic && _sleepFrames > 0)
                    {
                        _sleepCountDown = _sleepFrames;
                        _rigidBody.isKinematic = true;
                    }
                }
                //OVR FreeTransfomer Code would go here
                MarkeBaseScale();


            }

            _whenRespawned.Invoke();
        }
        /// <summary>
        /// ovveride this method to mark the base scale of the free transformer
        /// </summary>
        public virtual void MarkeBaseScale()
        {
            /*
            foreach (var freeTransformer in _freeTransformers)
            {
                freeTransformer.MarkAsBaseScale();
            }
            */
        }
    }
    public class OVRFPRespawnOnDrop: MonoBehaviour
    {
        //FAKE
    }
}
