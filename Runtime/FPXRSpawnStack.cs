namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    

    public class FPXRSpawnStack : FPSpawner
    {
        public bool Testing = false;
        [SerializeField] private List<GameObject> spawnStack = new List<GameObject>(); // List of prefabs in stack order

        [SerializeField] protected int currentIndex = 0;
        [SerializeField] protected Transform BottomStackPosition;
        [SerializeField] protected Transform TopStackPosition;
        [SerializeField] protected float stackRange;
        [SerializeField] protected float stackStepSize;
        [SerializeField] protected GameObject UnderStackVisualization;
        //[SerializeField] protected float verticalScaleMeasure;
        [SerializeField] protected GameObject currentOnStackItem;
        [SerializeField] protected Transform SpawnParentRoot;
        [Space]
        public GameObject TestItem;
        #region Cached Listeners
        [SerializeField]private List<FPWorldItem> grabbedItemListenerList = new List<FPWorldItem>();
        #endregion
        protected virtual void Start()
        {
            if (SpawnParentRoot == null)
            {
                SpawnParentRoot = this.transform;
            }

            stackRange = Vector3.Distance(BottomStackPosition.position, TopStackPosition.position);
            //assumption is that the UnderStackVisualization visual fits the distance between the two points
            if (UnderStackVisualization.transform.localScale.y != 1)
            {
                Debug.LogWarning($"Understack visualization should be a scale of 1 for starting scale, update it's children visual to make it 'fit'");
            }
            //verticalScaleMeasure = stackRange / (UnderStackVisualization.transform.localScale.y);
            currentIndex = spawnStack.Count - 1;
            //start and setup the stack
           
            stackStepSize = stackRange / (currentIndex);
            AdjustSpawnPosition(TopStackPosition.position);
            currentOnStackItem = SpawnNextItem();
            AdjustSpawnPosition(ReturnPointOnLine((currentIndex) * stackStepSize));
        }
        
        public virtual void Update()
        {
            if (!Testing)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnNextItem();
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                ReturnItem(TestItem);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                //fake grabber
                Debug.Log($"Fake grab!");
                if(currentOnStackItem != null)
                {
                    //toss item off the top of the stack
                    currentOnStackItem.GetComponent<FPWorldItem>().ItemRigidBody.AddRelativeForce(Vector3.up*250);
                    currentOnStackItem.GetComponent<FPWorldItem>().PickedUpItem(0);
                }
                    
            }
        }
        public GameObject SpawnNextItem()
        {
            currentOnStackItem = Spawn();
            //check if our item is a World Item
            currentOnStackItem.transform.SetParent(SpawnParentRoot);
            if (currentOnStackItem != null)
            {
                UpdateVisualStackSize();
                AdjustSpawnPosition(ReturnPointOnLine(currentIndex * stackStepSize));
                var fpWorld = currentOnStackItem.GetComponent<FPWorldItem>();
                if (fpWorld != null)
                {
                    fpWorld.ItemSpawnedEvent();
                    fpWorld.ItemGrabbed += ListenForFirstGrab;
                    if (!grabbedItemListenerList.Contains(fpWorld))
                    {
                        grabbedItemListenerList.Add(fpWorld);
                    }
                }
            }
            return currentOnStackItem;
        }
        

        protected void ListenForFirstGrab(FPWorldItem theItem, XRHandedness theHandItsIn)
        {
            Debug.LogWarning($"An item was just grabbed and we cared about it, {theItem.gameObject.name} and it was apparently grabbed by the {theHandItsIn.ToString()} hand");
            //when we fire this off we are going to remove ourselves from it as well
            if(grabbedItemListenerList.Contains(theItem))
            {
                theItem.ItemGrabbed -= ListenForFirstGrab;
                grabbedItemListenerList.Remove(theItem);
                SpawnNextItem();
            }
        }
        public void OnDisable()
        {
            foreach (var item in grabbedItemListenerList)
            {
                item.ItemGrabbed -= ListenForFirstGrab;
            }
        }
        public void ReturnItem(GameObject theItemPassedToUs)
        {
            //returnprefab = adjusts our currentIndex up one and returns true if we can return an item
            if (theItemPassedToUs == null)
            {
                Debug.LogError($"nothing passed to us for a return...");
                return;
            }
            if (ReturnPrefab())
            {
                //Debug.LogWarning($"Current Index: {currentIndex}");
                
                AdjustSpawnPosition(ReturnPointOnLine(currentIndex * stackStepSize));
                UpdateVisualStackSize();
               
                //remove current item if we have one
                if (currentOnStackItem != null)
                {
                    Destroy(currentOnStackItem);
                    currentOnStackItem = theItemPassedToUs;
                }
                currentOnStackItem.transform.position = spawnPosition.position;
                currentOnStackItem.transform.rotation = Quaternion.identity;
                currentIndex--;
                AdjustSpawnPosition(ReturnPointOnLine((currentIndex) * stackStepSize));

            }
        }
        protected override GameObject GetNextPrefab()
        {
            //start at the top of the List which is backwards 0->n 0 on the bottom

            if (currentIndex < spawnStack.Count && currentIndex >= 0)
            {
                GameObject nextPrefab = spawnStack[currentIndex];
                currentIndex--;
                return nextPrefab;
            }
            else
            {
                Debug.Log("Stack is empty. No more items to spawn.");
                return null;
            }
        }
        protected virtual bool ReturnPrefab() 
        {
            if(currentIndex+2 < spawnStack.Count && currentIndex >= 0)
            {
                currentIndex+=2;
                return true;
            }
            else
            {
                Debug.Log("Stack is full.");
            }
            return false;
        }
        protected Vector3 ReturnPointOnLine(float distance)
        {
            Vector3 direction = TopStackPosition.position- BottomStackPosition.position;
            direction.Normalize();
            return BottomStackPosition.position + direction * distance;
        }
        protected void UpdateVisualStackSize() 
        {
            //need the ratio which is always the spawnPosition prior to the actual spawn - call before we adjust the location
            var curDist = Vector3.Distance(spawnPosition.position, BottomStackPosition.position);
            var ratio = curDist / stackRange;
            if (ratio < 0.001f)
            {
                ratio = 0;
            }
            var newScale = new Vector3(UnderStackVisualization.transform.localScale.x, ratio, UnderStackVisualization.transform.localScale.z);
            UnderStackVisualization.transform.localScale = newScale;
        }
        protected void OnDrawGizmos()
        {
            if (BottomStackPosition != null && TopStackPosition != null&&spawnPosition!=null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(BottomStackPosition.position, TopStackPosition.position);
                Gizmos.DrawLine(spawnPosition.position, spawnPosition.position + Vector3.right * 0.1f);
                Gizmos.DrawLine(spawnPosition.position, spawnPosition.position + Vector3.left * 0.1f);
#if UNITY_EDITOR
                UnityEditor.Handles.Label(spawnPosition.position, "Spawn Position");
#endif
            }
        }
    }
}
