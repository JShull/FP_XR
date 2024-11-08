namespace FuzzPhyte.XR
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Base class for Menu Items
    /// </summary>
    public class FPXRMenuItem : MonoBehaviour
    {
        public FPXRMenu TheMenu;
        public FPXRMenuIcon MenuIcon;
        [Tooltip("Visual representation of the menu item")]
        public GameObject MenuRealObject;
        public Transform VRPivotPoint;
        public float MaxDistanceFromPivotPoint = 3f;
        [SerializeField] protected bool _firstTimeOpen;
        public UnityEvent RunFirstTimeOpenEvent;
        public float DistanceSpawnOffset = 0.1f;

        protected virtual void Start()
        {
            if (MenuRealObject != null)
            {

            }
        }
        public virtual void MoveObjectOutOfMenu(Vector3 playerForwardNormalized)
        {
            if (MenuRealObject.activeInHierarchy)
            {
                return;
            }
            if (!_firstTimeOpen)
            {
                _firstTimeOpen = true;
                RunFirstTimeOpenEvent.Invoke();
            }
            //location related
            MenuRealObject.transform.position = TheMenu.ReturnItemMovePos() + DistanceSpawnOffset * playerForwardNormalized;
            MenuRealObject.transform.rotation = Quaternion.LookRotation(playerForwardNormalized);
        }
        public virtual void MoveObjectBackInMenu()
        {
            StartCoroutine(DelayOneFrameBeforeDeactivation());
        }
        IEnumerator DelayOneFrameBeforeDeactivation()
        {
            yield return new WaitForFixedUpdate();
            //ResetLocationSubChildren();
            //if (toolRelated != null)
            //{
                ////makes kinematics true so the next time we pull the tool it doesn't have Physics/gravity running on it
              //  toolRelated.ResetKinematics(true);
            //}
            //AdditionalToolReturnEvent.Invoke();
            MenuRealObject.SetActive(false);
            //MenuIcon.OVRGrabbable.enabled = true;
            //MenuIcon.OVRGrabInteractable.enabled = true;
            //MenuIconHighlite.SetActive(true);

        }
    }
}
