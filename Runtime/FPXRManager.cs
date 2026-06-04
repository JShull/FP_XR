// Copyright (c) 2026 John B. Shull
// FuzzPhyte LLC is a company associated with John B. Shull
// This file is part of FP_XR Package.
//
// Public license: GNU GPLv3-or-later.
// Commercial/proprietary use requires a separate license from John B. Shull.
//
// See LICENSE.md COMMERCIAL-LICENSE.md, and NOTICE.md.

using UnityEngine;

namespace FuzzPhyte.XR
{
    public class FPXRManager : MonoBehaviour
    {
        [SerializeField]protected GameObject referenceToLeftHand;
        public GameObject LeftHand { get { return referenceToLeftHand; } }
        [SerializeField]protected GameObject referenceToRightHand;
        public GameObject RightHand { get { return referenceToRightHand; } }
        public static FPXRManager Instance { get; private set; }
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
        public void SetupLeftHand(GameObject passedHand)
        {
            this.referenceToLeftHand = passedHand;
        }
        public void SetupRightHand(GameObject passedHand)
        {
            this.referenceToRightHand = passedHand;
        }
    }
}
