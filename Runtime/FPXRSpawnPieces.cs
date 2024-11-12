
namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
    using UnityEngine.Events;

    public class FPXRSpawnPieces : FPSpawner
    {
        public bool Testing = false;
        [SerializeField] protected GameObject VisualizationGroupObject;
        [SerializeField] protected List<GameObject> spawnPiecesPrefab = new List<GameObject>(); // List of prefabs in stack order
        [SerializeField] protected List<Transform> spawnLocations = new List<Transform>();
        [SerializeField] protected int currentIndex = 0;
        [Tooltip("Only does the spawning action one time, doesn't work with spawnReplac")]
        [SerializeField] protected bool spawnOnce = true;
        protected bool spawnedAtLeastOnce = false;
        [SerializeField] protected bool SpawnXTimes = false;
        [SerializeField] protected int runningCount = 0;
        [SerializeField] protected int maxSpawnCount = 10;
        [Tooltip("If true, will replace the spawned items with new ones, if false will add to the list of spawned items.")]
        [SerializeField] protected bool spawnReplace = false;
        [SerializeField] protected List<GameObject> cachedSpawnedItems = new List<GameObject>();
        [Space]
        public UnityEvent AdditionalSpawnEvent;
        protected override GameObject GetNextPrefab()
        {
            spawnPosition = spawnLocations[currentIndex];
            return spawnPiecesPrefab[currentIndex];
        }
        public virtual void Start()
        {
            if (VisualizationGroupObject == null)
            {
                Debug.LogError($"Missing a group object for visualization, please add a visual reference for our group to this script!");
            }
            if(spawnPiecesPrefab.Count!=spawnLocations.Count)
            {
                Debug.LogError("Spawn Pieces and Spawn Locations are not equal in count, please fix this");
            }
            if(spawnOnce && spawnReplace)
            {
                Debug.LogWarning($"Can't have both spawnOnce and spawnReplace set to true, setting spawn replace to false");
                spawnReplace = false;
            }
            if (spawnOnce&& SpawnXTimes)
            {
                Debug.LogWarning($"Can't have spawnOnce and SpawnXTimes both true, resetting SpawnXTimes");
                SpawnXTimes = false;
            }
        }
        /// <summary>
        /// Public method to spawn the pieces
        /// </summary>
        public virtual void SpawnThePieces()
        {
            if(VisualizationGroupObject != null)
            {
                VisualizationGroupObject.SetActive(false);
            }
            if (spawnReplace&&!spawnOnce)
            {
                for(int i = 0; i < cachedSpawnedItems.Count; i++)
                {
                    Destroy(cachedSpawnedItems[i]);
                }
                cachedSpawnedItems.Clear();
            }
            if (spawnOnce && !spawnedAtLeastOnce)
            {
                for (int i = 0; i < spawnPiecesPrefab.Count; i++)
                {
                    currentIndex = i;
                    var prefabItem = Spawn();
                    cachedSpawnedItems.Add(prefabItem);
                }
                AdditionalSpawnEvent.Invoke();
                spawnedAtLeastOnce = true;
            }
            if (!spawnOnce)
            {
                if (SpawnXTimes)
                {
                    runningCount++;
                    if(runningCount>maxSpawnCount)
                    {
                        return;
                    }
                }
                for (int i = 0; i < spawnPiecesPrefab.Count; i++)
                {
                    currentIndex = i;
                    var prefabItem = Spawn();
                    cachedSpawnedItems.Add(prefabItem);
                }
                AdditionalSpawnEvent.Invoke();
            }
        }

#if UNITY_EDITOR
        public virtual void Update()
        {
            if (!Testing)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnThePieces();
            }
        }
#endif
        protected void OnDrawGizmos()
        {
            if ((spawnPiecesPrefab.Count == spawnLocations.Count) && spawnPosition!=null && VisualizationGroupObject!=null)
            {
                Gizmos.color = Color.red;
                for(int i = 0; i < spawnLocations.Count; i++)
                {
                    Gizmos.DrawLine(spawnLocations[i].position, VisualizationGroupObject.transform.position);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(spawnLocations[i].position,0.1f);
                }
            }
        }
    }
}
