namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Utility.Meta;
    public class FPXRTag : MonoBehaviour
    {
        public FP_Tag WorldTag;
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
