namespace FuzzPhyte.XR
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// Base Interactive Menu
    /// </summary>
    public class FPXRMenu : MonoBehaviour
    {
        public List<FPXRMenuIcon> AllMenuObjects = new List<FPXRMenuIcon>();
        [Space]
        [Header("Pivot Related")]
        public Transform LeftHandPivot;
        public Transform RightHandPivot;
        public Transform MenuPivot;
        public Transform UnMenuParent;
        public XRHandedness CurrentHand;
        #region Core Menu
        public virtual void SetMenuRightHand()
        {
            MenuPivot.SetParent(RightHandPivot);
            MenuPivot.localPosition = Vector3.zero;
            MenuPivot.localRotation = Quaternion.identity;
            CurrentHand = XRHandedness.Right;
        }
        public virtual void SetMenuLeftHand()
        {
            MenuPivot.SetParent(LeftHandPivot);
            MenuPivot.localPosition = Vector3.zero;
            MenuPivot.localRotation = Quaternion.identity;
            CurrentHand = XRHandedness.Left;
        }
        public virtual void ResetAllMenuItems()
        {
            for (int i = 0; i < AllMenuObjects.Count; i++)
            {
                var anIcon = AllMenuObjects[i];
                if (anIcon != null)
                {
                    if (anIcon.gameObject.activeInHierarchy)
                    {
                        OverrideCloseItem(anIcon);
                    }
                }
            }
        }
        public Vector3 ReturnItemMovePos()
        {
            switch (CurrentHand)
            {
                case XRHandedness.Left:
                    return RightHandPivot.position;
                case XRHandedness.Right:
                    return LeftHandPivot.position;
                default:
                    return MenuPivot.position;
            }
        }
        public void OverrideCloseItem(FPXRMenuIcon menuObjectBack)
        {
            //menuObjectBack.OVRGrabInteractable.enabled = true;
            menuObjectBack.TheMenuItem.MoveObjectBackInMenu();
        }
        #endregion
    }
}
