// Copyright (c) 2026 John B. Shull
// FuzzPhyte LLC is a company associated with John B. Shull
// This file is part of FP_XR Package.
//
// Public license: GNU GPLv3-or-later.
// Commercial/proprietary use requires a separate license from John B. Shull.
//
// See LICENSE.md COMMERCIAL-LICENSE.md, and NOTICE.md.

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
        [Space]
        public List<XRButtonHintStep> AcknowledgeSequence;
    }
}
