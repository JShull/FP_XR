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
