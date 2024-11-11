namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    

    public class FPXRSpawnStack : FPSpawner
    {
        [SerializeField] private List<GameObject> spawnStack = new List<GameObject>(); // List of prefabs in stack order

        [SerializeField] protected int currentIndex = 0;
        [SerializeField] protected Transform BottomStackPosition;
        [SerializeField] protected Transform TopStackPosition;
        protected float stackRange;
        [SerializeField] protected float stackStepSize;
        [SerializeField] protected GameObject UnderStackVisualization;
        [SerializeField] protected float verticalScaleMeasure;
        [SerializeField] protected GameObject currentOnStackItem;
        [Space]
        public GameObject TestItem;
        protected virtual void Start()
        {
            stackRange = Vector3.Distance(BottomStackPosition.position, TopStackPosition.position);
            //assumption is that the UnderStackVisualization visual fits the distance between the two points
            verticalScaleMeasure = stackRange / (UnderStackVisualization.transform.localScale.y);
            currentIndex = spawnStack.Count - 1;
            //start and setup the stack
           
            stackStepSize = stackRange / (currentIndex);
            AdjustSpawnPosition(TopStackPosition.position);
            currentOnStackItem = Spawn();
            AdjustSpawnPosition(ReturnPointOnLine((currentIndex) * stackStepSize));
        }
        
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnNextItem();
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                ReturnItem(TestItem);
            }
        }
        public void SpawnNextItem()
        {
            currentOnStackItem = Spawn();
            if(currentOnStackItem != null)
            {
                UpdateVisualStackSize();
                AdjustSpawnPosition(ReturnPointOnLine(currentIndex * stackStepSize));
                
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
            var newScale = new Vector3(UnderStackVisualization.transform.localScale.x, verticalScaleMeasure * ratio, UnderStackVisualization.transform.localScale.z);
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
