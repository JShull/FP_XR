namespace FuzzPhyte.XR
{
    using UnityEngine;

    /// <summary>
    /// Check on items going where they shouldn't
    /// Assumes that this is directly on some sort of collider
    /// </summary>
    public class FPWorldCheck : MonoBehaviour
    {
        public bool UseTrigger=true;
        public bool UseCollider=false;
        public virtual void OnTriggerEnter(Collider other)
        {
            if (!UseTrigger) { return; }
            if (other.gameObject.GetComponent<FPWorldItem>())
            {
                Debug.Log($"World Item {other.gameObject.name} fell out of bounds... reset!");
                other.gameObject.GetComponent<FPWorldItem>().ResetLocation();
            }
        }
        public virtual void OnCollisionEnter(Collision collision)
        {
            if (!UseCollider) { return; }
            if (collision.gameObject.GetComponent<FPWorldItem>())
            {
                Debug.Log($"World Item {collision.gameObject.name} fell out of bounds... reset!");
                collision.gameObject.GetComponent<FPWorldItem>().ResetLocation();
            }
        }
    }
}
