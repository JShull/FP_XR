namespace FuzzPhyte.XR
{
    using UnityEngine;
    public class FPXRTag : MonoBehaviour
    {
        public XRInteractorType InteractionFlag;
        public virtual bool MatchedInteractionTag(string type)
        {
            if (InteractionFlag.ToString() == type)
            {
                return true;
            }
            return false;
        }
    }
}
