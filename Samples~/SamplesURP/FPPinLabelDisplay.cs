
namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FPPinLabelDisplay : MonoBehaviour
    {
        public GameObject FPObject;
        public FPPinPlacement PinComponent;
        public FPVocabTagDisplay DisplayComponent;
        protected List<IFPMotionController> Lerpers = new List<IFPMotionController>();

        public void Awake()
        {
            //get components that are using IFPLerpControllers on this transform
            Lerpers.AddRange(GetComponents<IFPMotionController>());
        }
        public void Start()
        {
            StartCoroutine(DelayStart());
        }
        IEnumerator DelayStart()
        {
            yield return new WaitForEndOfFrame();
            SetupPivotLocationForPin();
        }
        public void SetupPivotLocationForPin()
        {
            if (PinComponent && DisplayComponent)
            {
                PinComponent.SetPinTailTransformTracking(DisplayComponent.AttachmentLocation);
                if (FPObject)
                {
                    PinComponent.SetPinHeadTransformTracking(FPObject.transform);
                }
            }
            for (int i = 0; i < Lerpers.Count; i++)
            {
                Lerpers[i].SetupMotion();
            }
        }
        public void ActivateSystem()
        {
            if (PinComponent)
            {
                PinComponent.StartTrackingPin();
            }
            
            for(int i = 0; i < Lerpers.Count; i++)
            {
                Lerpers[i].StartMotion();
            }
        }
    }
}

