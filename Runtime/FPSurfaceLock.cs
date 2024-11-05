namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Ray;
    using FuzzPhyte.Utility;

    public class FPSurfaceLock
    {
        private GameObject ghostObject = null; // Static reference to the ghost object
        private Renderer ghostRenderer = null;
        private bool isCollidingActive = false;
        /// <summary>
        /// Initializes the ghost object for surface locking.
        /// </summary>
        /// <param name="objectToLock">The original GameObject to create a ghost object from.</param>
        public GameObject InitializeGhostObject(Transform parent,GameObject objectToLock)
        {
            if (ghostObject == null)
            {
                ghostObject = GameObject.Instantiate(objectToLock);
                ghostObject.name = objectToLock.name + "_Ghost";
                //assume child 0 is the renderer
                if(ghostObject.transform.childCount > 0)
                {
                    if (ghostObject.transform.GetChild(0).GetComponent<Renderer>() != null)
                    {
                        ghostRenderer = ghostObject.transform.GetChild(0).GetComponent<Renderer>();
                    }
                    else
                    {
                        Debug.LogError($"missing renderer on child 0 of {ghostObject.name}");
                    }
                }
                ghostObject.transform.SetPositionAndRotation(parent.position, parent.rotation);
                ghostObject.SetActive(false); // Hide the ghost object in the scene
            }
            return ghostObject;
        }

        /// <summary>
        /// Cleans up and destroys the ghost object.
        /// </summary>
        public void DestroyGhostObject()
        {
            if (ghostObject != null)
            {
                GameObject.Destroy(ghostObject);
                ghostObject = null;
            }
        }

        /// <summary>
        /// Locks the given object to the nearest surface detected by a raycast.
        /// </summary>
        /// <param name="boundingBox">The bounding box information of the object to be locked.</param>
        /// <param name="raycaster">The FP_Raycaster instance used to detect surfaces.</param>
        /// <param name="objectToLock">The GameObject to lock onto the surface.</param>
        /// <param name="alignment">Optional: Whether to align the object to the normal of the surface.</param>
        /// <returns>True if the object was successfully locked, false otherwise.</returns>
        public bool LockToSurface(FP_Raycaster raycaster, SO_FPRaycaster reverseRayData,GameObject objectToLock, Renderer objectRenderer,bool collisionCheck=false, bool alignment = true)
        {
            // Ensure the inputs are valid
            if (raycaster == null || objectToLock == null || ghostObject == null)
            {
                Debug.LogError("Invalid input parameters for surface locking, check raycaster, objectToLock, and your ghostObject");
                return false;
            }

            // Activate the raycaster to perform a raycast
            raycaster.ActivateRaycaster();
            raycaster.FireRaycast();
            FP_RayArgumentHit hitInfo = raycaster.ReturnCurrentHitItem;
            ghostObject.SetActive(true);
            if (hitInfo != null && hitInfo.HitObject != null)
            {
                Vector3 worldEndPoint = hitInfo.WorldEndPoint;
                // Direction from the object's center to the raycast hit point
                Vector3 directionToSurface = (worldEndPoint - objectToLock.transform.position).normalized;
                //Debug.LogWarning($"Vector Direction to Surface: {directionToSurface}");
                // Invert the direction to point back towards the object
                Vector3 inverseDirection = -directionToSurface;
                //Debug.LogWarning($"Inversed to Object: {inverseDirection}");
                Debug.DrawRay(worldEndPoint, inverseDirection, Color.cyan, 5f);
                //Raycast back to the item to get the surface point on the item
                
                if (Physics.Raycast(worldEndPoint, inverseDirection, out RaycastHit selfHit, reverseRayData.RaycastLength, reverseRayData.LayerToInteract))
                {
                   
                    // Adjust the object's position so that the detected surface point aligns with the raycast hit point
                    Vector3 surfacePointOffset = worldEndPoint - selfHit.point;
                    
                    //move ghost object to the surface point
                    Vector3 ghostStartPos = Vector3.zero;
                    Quaternion ghostStartRot = Quaternion.identity;
                    
                    ghostStartPos = objectToLock.transform.position;
                    ghostStartRot = objectToLock.transform.rotation;
                    ghostStartPos += surfacePointOffset;
                    //Debug.Log($"World Surface Point = {worldEndPoint}, - self hit point  {selfHit.point} = {surfacePointOffset} | Start Pos: {ghostStartPos}");
                   
                    if (alignment)
                    {
                        // Align the object's rotation to the surface normal
                        ghostStartRot = Quaternion.FromToRotation(Vector3.up, worldEndPoint - hitInfo.HitObject.position);
                        //ghostObject.transform.rotation = alignmentRotation;
                        //JOHN
                        //objectToLock.transform.rotation = alignmentRotation;
                    }

                    // Run a collision check
                    if (collisionCheck)
                    {
                        //need to adjust my bounding box to the new position
                        //var aNewBox = FP_UtilityData.CreateBoundingBox(objectToLock, objectRenderer);
                        var aNewBox = FP_UtilityData.CreateBoundingBox(ghostStartPos,ghostStartRot, ghostRenderer);

                        if(aNewBox==null)
                        {
                            //JOHN
                            Debug.LogError($"Failed to create a new bounding box for object: {ghostObject.name}");
                            return false;
                        }
                        else
                        {
                            //Debug.LogWarning($"a New box Center: {aNewBox.Value.Center}");
                        }
                        
                        var colliding = IsColliding(ghostStartRot, aNewBox.Value);
                        //Debug.LogWarning($"Colliding: {colliding}");
                        if (colliding)
                        {
                            //JOHN
                            //ResolveCollision(objectToLock, objectRenderer, 0.01f,50);
                            var resolvedCollision = ResolveCollision(ghostObject,ghostStartPos,ghostStartRot, ghostRenderer, 0.01f,50);
                            
                            if (resolvedCollision)
                            {
                                isCollidingActive = true;
                            }
                            else
                            {
                                isCollidingActive = false;
                            }
                            
                        }
                        else
                        {
                            isCollidingActive = false;
                        }
                        return true;
                    }
                }
                else
                {
                    Debug.LogError("Failed to find a surface point on the object to lock.");
                }
            }

            // Deactivate the raycaster if no surface was detected
            raycaster.DeactivateRaycaster();
            return false;
        }

        /// <summary>
        /// Checks if the object is colliding with any other objects.
        /// </summary>
        private bool IsColliding(Quaternion worldRotation, FP_BoundingBoxInfo boundingBox)
        {
            //Debug.LogWarning($"Checking for Collisions at {boundingBox.Center} with a size {boundingBox.Size / 2}");

            Collider[] colliders = Physics.OverlapBox(
                boundingBox.Center,
                boundingBox.Size / 2,
                worldRotation
            );

            // Return true if there are any colliders other than the object's own collider
            if (colliders.Length > 0)
            {
                return true;
            }
            /*
            foreach (Collider collider in colliders)
            {
                return true;
            }
            */
            return false;
        }

        /// <summary>
        /// Attempts to resolve the collision by moving the object in different directions.
        /// Includes a timeout mechanism to stop after a certain number of attempts.
        /// </summary>
        private bool ResolveCollision(GameObject ghostItem, Vector3 worldPositionToCheck, Quaternion worldRotation, Renderer objectRenderer, float stepSize =0.1f, int maxAttempts =20)
        {
            Vector3[] directions = {
                Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back
            };
            if(maxAttempts < 1)
            {
                Debug.LogError("Max Attempts must be greater than 0, force set to 10");
                maxAttempts = 10; 
            }
            //int maxAttempts = 10; // Maximum number of attempts to resolve the collision
            int attemptCount = 0;

            while (attemptCount < maxAttempts)
            {
                for(int i = 0; i < directions.Length; i++)
                {
                    var direction = directions[i];
                    worldPositionToCheck += direction * stepSize * (attemptCount+1); // Adjust the step size as needed
                    //reset boundingBox
                    var aNewBox = FP_UtilityData.CreateBoundingBox(worldPositionToCheck, worldRotation, objectRenderer);
                    if (aNewBox.HasValue)
                    {
                       
                        //Debug.Log($"Test Position: {aNewBox.Value.Center}");
                        if (!IsColliding(worldRotation, aNewBox.Value))
                        {
                            ghostItem.transform.position = worldPositionToCheck;
                            ghostItem.transform.rotation = worldRotation;
                            //Debug.LogWarning($"Resolved Position: {worldPositionToCheck}");
                            return true; // Collision resolved
                        }
                        worldPositionToCheck -= direction * stepSize * attemptCount; // Revert the move if still colliding
                    }
                    
                }
                
                attemptCount++;
            }
            
            Debug.LogWarning($"Unable to resolve collision for object: {worldPositionToCheck} after {maxAttempts} attempts.");
            return false;
        }
    }
}
