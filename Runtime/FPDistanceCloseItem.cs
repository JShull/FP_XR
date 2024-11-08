namespace FuzzPhyte.XR
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    
    public class FPDistanceCloseItem : MonoBehaviour
    {
        public GameObject RealWorldObject;
        public float MaxDistanceFromPivotPoint = 3f;
        [Tooltip("Only Items that arent moving on behalf of something else")]
        public List<GameObject> SubRealObjects = new List<GameObject>();
        protected List<Vector3> SubRealStartingLocalPositions = new List<Vector3>();
        public Transform VRPivotPoint;
        [SerializeField] private FPXRTool toolRelated;
        public UnityEvent AdditionalEventBeforeClose;

        public void Start()
        {
            if (VRPivotPoint == null)
            {
                Debug.LogError($"This won't work without a transform to track, disabling myself");
                this.enabled = false;
                return;
            }
            if (RealWorldObject != null && toolRelated == null)
            {
                if (RealWorldObject.GetComponent<FPXRTool>() != null)
                {
                    toolRelated = RealWorldObject.GetComponent<FPXRTool>();
                }
            }
            for (int i = 0; i < SubRealObjects.Count; i++)
            {
                SubRealStartingLocalPositions.Add(SubRealObjects[i].transform.localPosition);
            }
        }

        public void StartDistanceChecker()
        {
            StartCoroutine(CloseLoopCheck());
        }
        /// <summary>
        /// Coroutine to keep tabs on proximity of where we are relative our item
        /// </summary>
        /// <returns></returns>
        IEnumerator CloseLoopCheck()
        {
            var curDistance = Vector3.Distance(RealWorldObject.transform.position, VRPivotPoint.position);
            while (curDistance < MaxDistanceFromPivotPoint)
            {
                //MenuRealObject.transform.position = Vector3.MoveTowards(MenuRealObject.transform.position, VRPivotPoint.position, 0.1f);
                if (SubRealObjects.Count > 0)
                {
                    curDistance = Vector3.Distance(SubRealObjects[0].transform.position, VRPivotPoint.position);
                }
                else
                {
                    curDistance = Vector3.Distance(RealWorldObject.transform.position, VRPivotPoint.position);
                }
                yield return null;
            }
            if (toolRelated != null)
            {
                //confirm it's not in your hand - stretching out
                while (toolRelated.ToolInHand)
                {
                    yield return null;
                }
                //autoreturn tool activated
                toolRelated.ToolAutoBackInMenu();
            }
            yield return StartCoroutine(DelayOneFrameBeforeDeactivation());
        }
        IEnumerator DelayOneFrameBeforeDeactivation()
        {
            yield return new WaitForFixedUpdate();
            //reset children location
            for (int i = 0; i < SubRealObjects.Count; i++)
            {
                SubRealObjects[i].transform.localPosition = SubRealStartingLocalPositions[i];
                //SubRealObjects[i].transform.localRotation = Quaternion.identity;
            }
            if (toolRelated != null)
            {
                //makes kinematics true so the next time we pull the tool it doesn't have Physics/gravity running on it
                toolRelated.ResetKinematics(true);
            }
            AdditionalEventBeforeClose.Invoke();
            RealWorldObject.SetActive(false);
        }
    }

    public class OVRFPDistanceCloseItem : MonoBehaviour
    {
        //FAKE
    }
}
