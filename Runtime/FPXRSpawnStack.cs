namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    

    public class FPXRSpawnStack : FPSpawner
    {
       
        [SerializeField] private List<GameObject> spawnStack = new List<GameObject>(); // List of prefabs in stack order
        [SerializeField] private List<GameObject> randomSpawnedItems = new List<GameObject>(); //list of randomly spawned items out there in the world
        [SerializeField] protected int currentIndex = 0;
        [SerializeField] protected Transform BottomStackPosition;
        [SerializeField] protected Transform TopStackPosition;
        [SerializeField] protected float stackRange;
        [SerializeField] protected float stackStepSize;
        [SerializeField] protected GameObject UnderStackVisualization;
       
        [SerializeField] protected GameObject currentOnStackItem;
        [SerializeField] protected Transform SpawnParentRoot;
        [Space]
        public GameObject TestItem;
        [Tooltip("Will not use a stack approach but just randomly pull from the list")]
        public bool UseRandomPull = false;
        public bool SetupOnStart = true;
        [SerializeField]protected int maxRandomSpawns = 10;
        //[SerializeField] protected int currentRandomSpawns = 0;
        #region Cached Listeners
        [SerializeField]private List<FPWorldItem> grabbedItemListenerList = new List<FPWorldItem>();
        #endregion
        
        protected virtual void Start()
        {
            if (SetupOnStart)
            {
                if (SpawnParentRoot == null)
                {
                    SpawnParentRoot = this.transform;
                }
                //always setup the stack
                SetupStack();

                if (UseRandomPull)
                {
                    //do nothing
                    //SetupAndUseRandomBin();
                    Debug.LogWarning($"You will need to use SetupAndUseRandomBin()");
                }
                else
                {
                    //generate our first item and adjust our spawn position
                    currentOnStackItem = SpawnNextItem();
                    AdjustSpawnPosition(ReturnPointOnLine((currentIndex) * stackStepSize));
                }
            }
        }
        public virtual void SetupAndUseRandomBin(List<GameObject>spawnedItems,int maxSize)
        {
            ResetSystem();
            spawnStack.Clear();
            spawnStack.AddRange(spawnedItems);
            //setup stack for random based on max size
            stackRange = Vector3.Distance(BottomStackPosition.position, TopStackPosition.position);
            
            if (UnderStackVisualization.transform.localScale.y != 1)
            {
                Debug.LogWarning($"Understack visualization should be a scale of 1 for starting scale, update it's children visual to make it 'fit'");
            }
            maxRandomSpawns = maxSize;
            currentIndex = 0;
            //start and setup the stack
            stackStepSize = stackRange / maxRandomSpawns;
            AdjustSpawnPosition(TopStackPosition.position);
            UseRandomPull = true;
        }
        public virtual void SetupAndUseStackBin(List<GameObject>spawnedItems)
        {
            ResetSystem();
            spawnStack.Clear();
            spawnStack.AddRange(spawnedItems);
            SetupStack();
        }
        /// <summary>
        /// Setup logic for general stack usage
        /// </summary>
        protected virtual void SetupStack()
        {
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
        }

#if UNITY_EDITOR
        
        [ContextMenu("Test Stack Spawn Next Item")]
        public void TestStackSpawnNextItem()
        {
            SpawnNextItem();
        }
        [ContextMenu("Test Stack return an Item")]
        public void TestReturnStackItem()
        {
            ReturnItem(TestItem);
        }
        [ContextMenu("Test pull from stack system")]
        public void TestPullFromStack()
        {
            Debug.Log($"Fake grab!");
            if (currentOnStackItem != null)
            {
                //toss item off the top of the stack
                currentOnStackItem.GetComponent<FPWorldItem>().ItemRigidBody.AddRelativeForce(Vector3.up * 250);
                currentOnStackItem.GetComponent<FPWorldItem>().PickedUpItem(0);
            }
        }
        [ContextMenu("Test-Random Setup")]
        public void TestRandomMaxTenSetup()
        {
            if (!UseRandomPull)
            {
                Debug.LogError($"Not setup for random, check your bool!");
                return;
            }
            List<GameObject>tempList = new List<GameObject>();
            tempList.AddRange(spawnStack);
            SetupAndUseRandomBin(tempList, 10);
        }
        [ContextMenu("Test-Random Spawn Item")]
        public void TestRandomPull()
        {
            if (UseRandomPull)
            {
                TriggerSpawn();
            }
        }
        [ContextMenu("Test-Random Return to Random Spawn")]
        public void TestRandomReturn()
        {
            ReturnToBinItemRandom(TestItem);
        }
#endif
        /// <summary>
        /// Activate a spawn event
        /// </summary>
        public void TriggerSpawn()
        {
            if (UseRandomPull)
            {
                SpawnRandomItem();
            }
            else
            {
                SpawnNextItem();
            }
        }
        /// <summary>
        /// Internal function to spawn random item from a collection
        /// </summary>
        /// <returns></returns>
        public GameObject SpawnRandomItem()
        {
            if (!UseRandomPull)
            {
                Debug.LogWarning("Random pull is disabled, returning null.");
                return null;
            }

            if (currentIndex >= maxRandomSpawns)
            {
                Debug.Log("Max random spawns reached.");
                return null;
            }

            if (spawnStack == null || spawnStack.Count == 0)
            {
                Debug.LogWarning("Spawn stack is empty.");
                return null;
            }

            int randomIndex = Random.Range(0, spawnStack.Count);
            GameObject prefab = spawnStack[randomIndex];

            GameObject spawned = Instantiate(prefab, spawnPosition.position, Quaternion.identity);
            randomSpawnedItems.Add(spawned);
            currentIndex++;
            var stackCountHeightDiff = maxRandomSpawns - currentIndex;
            AdjustSpawnPosition(ReturnPointOnLine((stackCountHeightDiff) * stackStepSize));
            UpdateVisualStackSize();
            return spawned;
        }
        public GameObject SpawnNextItem()
        {
            currentOnStackItem = Spawn();
            //check if our item is a World Item
            
            if (currentOnStackItem != null)
            {
                currentOnStackItem.transform.SetParent(SpawnParentRoot);
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
        /// <summary>
        /// Internal function that Reset's the system
        /// </summary>
        protected virtual void ResetSystem()
        {
            //remove listeners
            foreach (var item in grabbedItemListenerList)
            {
                item.ItemGrabbed -= ListenForFirstGrab;
            }
            foreach(var gItem in randomSpawnedItems)
            {
                Destroy(gItem);
            }
            randomSpawnedItems.Clear();
            grabbedItemListenerList.Clear();
            if (currentOnStackItem != null)
            {
                //get current
                var fpAboutDestroyed = currentOnStackItem.GetComponent<FPWorldItem>();
                if (fpAboutDestroyed != null)
                {
                    if (grabbedItemListenerList.Contains(fpAboutDestroyed))
                    {
                        fpAboutDestroyed.ItemGrabbed -= ListenForFirstGrab;
                        grabbedItemListenerList.Remove(fpAboutDestroyed);
                    }
                }
                Destroy(currentOnStackItem);
            }
            //reset visually
            var newScale = new Vector3(UnderStackVisualization.transform.localScale.x, 1, UnderStackVisualization.transform.localScale.z);
            UnderStackVisualization.transform.localScale = newScale;

        }
        /// <summary>
        /// Method to give back to the stack
        /// </summary>
        /// <param name="theItemPassedToUs"></param>
        public void ReturnToBinItemRandom(GameObject theItemPassedToUs)
        {
            if (!UseRandomPull)
            {
                Debug.LogError($"You're not using random pull but you asked a random pull return function, call ReturnItem instead?");
                return;
            }
            if (theItemPassedToUs == null)
            {
                Debug.LogError($"nothing passed to us for a return...");
                return;
            }
            //we've atleast pulled one item from this
            if (currentIndex>0)
            {
                //did this thing exist in our spawn stack?
                if (randomSpawnedItems.Contains(theItemPassedToUs))
                {
                    //drop our count down 1 unit
                    currentIndex--;
                    //adjust visual discrepency
                    var stackCountHeightDiff = maxRandomSpawns - currentIndex;
                    AdjustSpawnPosition(ReturnPointOnLine((stackCountHeightDiff) * stackStepSize));
                    UpdateVisualStackSize();
                    //destroy item
                    randomSpawnedItems.Remove(theItemPassedToUs);
                    Destroy(theItemPassedToUs);
                }
            }
            
        }
        public void ReturnItem(GameObject theItemPassedToUs)
        {
            //returnprefab = adjusts our currentIndex up one and returns true if we can return an item
            if (UseRandomPull)
            {
                Debug.LogError($"Random pull doesn't support returned item to the bin...yet");
                return;
            }
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
                    //get current
                    var fpAboutDestroyed = currentOnStackItem.GetComponent<FPWorldItem>();
                    if(fpAboutDestroyed != null)
                    {
                        if (grabbedItemListenerList.Contains(fpAboutDestroyed))
                        {
                            fpAboutDestroyed.ItemGrabbed -= ListenForFirstGrab;
                            grabbedItemListenerList.Remove(fpAboutDestroyed);
                        }
                    }
                    Destroy(currentOnStackItem);
                }
                currentOnStackItem = theItemPassedToUs;
                currentOnStackItem.transform.position = spawnPosition.position;
                currentOnStackItem.transform.rotation = Quaternion.identity;
                currentIndex--;
                AdjustSpawnPosition(ReturnPointOnLine((currentIndex) * stackStepSize));
                //re-assign listener
                var fpWorld = currentOnStackItem.GetComponent<FPWorldItem>();
                if (fpWorld != null)
                {
                    fpWorld.ItemGrabbed += ListenForFirstGrab;
                    if (!grabbedItemListenerList.Contains(fpWorld))
                    {
                        grabbedItemListenerList.Add(fpWorld);
                    }
                }
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
            Debug.LogWarning($"Current Index:{currentIndex}");
            if(currentIndex+2 < spawnStack.Count && currentIndex >= 0)
            {
                currentIndex+=2;
                Debug.LogWarning($"Updated Index: {currentIndex}");
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
            if (float.IsNaN(ratio))
            {
                ratio = 0;
            }
            else
            {
                if (ratio < 0.001f)
                {
                    ratio = 0;
                }
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
