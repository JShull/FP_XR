namespace FuzzPhyte.XR
{
    using UnityEngine;
    using System;

    [RequireComponent(typeof(Rigidbody))]
    public class FPXRPhysicsListener : MonoBehaviour
    {
        [Tooltip("List of tags this bin accepts")]
        public string[] acceptedTags;
        [Tooltip("Physics Collisions")]
        public event Action<Collision> OnCollisionEnterEvent;
        public event Action<Collision> OnCollisionStayEvent;
        public event Action<Collision> OnCollisionExitEvent;

        [Tooltip("Physics Triggers")]
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerStayEvent;
        public event Action<Collider> OnTriggerExitEvent;

        public bool UseCollisionEnterEvent;
        public bool UseCollisionStayEvent;
        public bool UseCollisionExitEvent;
        public bool UseTriggerEnterEvent;
        public bool UseTriggerStayEvent;
        public bool UseTriggerExitEvent;

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (!UseCollisionEnterEvent) return;
            if (IsTagAccepted(collision.gameObject.tag))
            {
                OnCollisionEnterEvent?.Invoke(collision);
            }
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            if(!UseCollisionStayEvent) return;
            if (IsTagAccepted(collision.gameObject.tag))
            {
                OnCollisionStayEvent?.Invoke(collision);
            } 
        }

        protected virtual void OnCollisionExit(Collision collision)
        {
            if(!UseCollisionExitEvent) return;
            if (IsTagAccepted(collision.gameObject.tag))
            {
                OnCollisionExitEvent?.Invoke(collision);
            }
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!UseTriggerEnterEvent) return;
            if (IsTagAccepted(other.gameObject.tag))
            {
                OnTriggerEnterEvent?.Invoke(other);
            } 
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!UseTriggerStayEvent) return;
            if (IsTagAccepted(other.gameObject.tag))
            {
                OnTriggerStayEvent?.Invoke(other);
            }  
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            if (!UseTriggerExitEvent) return;
            if (IsTagAccepted(other.gameObject.tag))
            {
                OnTriggerExitEvent?.Invoke(other);
            }  
        }
        protected bool IsTagAccepted(string tag)
        {
            if(acceptedTags.Length == 0)
            {
                return true;
            }
            foreach (var acceptedTag in acceptedTags)
            {
                if (tag == acceptedTag)
                    return true;
            }
            return false;
        }
    }
}
