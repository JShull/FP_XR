namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using UnityEngine;
    using System.Collections.Generic;
    [CreateAssetMenu(fileName = "FPXRControllerFeedbackConfig", menuName = "FuzzPhyte/Utility/XR/Controller Feedback Config")]
    public class FPXRControllerFeedbackConfig: FP_Data
    {
        public XRHandedness ControllerStartHandedness;
        
        public List<ButtonFeedback> feedbacks;
        [TextArea(2, 4)]
        public string DescriptionNotes;
    }
}
