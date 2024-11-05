namespace FuzzPhyte.XR
{
    using UnityEngine;
    using FuzzPhyte.Ray;
    using FuzzPhyte.Utility;
    using Unity.Mathematics;
    
    public static class FPSurfaceLock
    {
        /// <summary>
        /// Locks the given object to the nearest surface detected by a raycast.
        /// </summary>
        /// <param name="boundingBox">The bounding box information of the object to be locked.</param>
        /// <param name="raycaster">The FP_Raycaster instance used to detect surfaces.</param>
        /// <param name="objectToLock">The GameObject to lock onto the surface.</param>
        /// <param name="alignment">Optional: Whether to align the object to the normal of the surface.</param>
        /// <returns>True if the object was successfully locked, false otherwise.</returns>
        public static bool LockToSurface(FP_BoundingBoxInfo boundingBox,FP_Raycaster raycaster, GameObject objectToLock, Renderer objectRenderer,bool collisionCheck=false, bool alignment = true)
        {
            // Ensure the inputs are valid
            if (raycaster == null || objectToLock == null)
            {
                Debug.LogError("Invalid input parameters for surface locking.");
                return false;
            }

            // Activate the raycaster to perform a raycast
            raycaster.ActivateRaycaster();
            raycaster.FireRaycast();
            FP_RayArgumentHit hitInfo = raycaster.ReturnCurrentHitItem;

            if (hitInfo != null && hitInfo.HitObject != null)
            {
                Vector3 worldEndPoint = hitInfo.WorldEndPoint;
                // Direction from the object's center to the raycast hit point
                Vector3 directionToSurface = (worldEndPoint - objectToLock.transform.position).normalized;
                Debug.LogWarning($"Vector Direction to Surface: {directionToSurface}");
                // Invert the direction to point back towards the object
                Vector3 inverseDirection = -directionToSurface;
                Debug.LogWarning($"Inversed to Object: {inverseDirection}");
                Debug.DrawRay(worldEndPoint, inverseDirection, Color.cyan, 5f);
                // Perform a raycast from the center of objectToLock in the direction of the surface point
                if (Physics.Raycast(worldEndPoint, inverseDirection, out RaycastHit selfHit, Mathf.Infinity))
                {
                    // Adjust the object's position so that the detected surface point aligns with the raycast hit point
                    //Vector3 surfacePointOffset = hitPoint - selfHit.point;
                    
                    Debug.Log($"World Surface Point = {worldEndPoint}, world object hit surface point =  {selfHit.point}");
                    // Adjust the object's position so that the detected surface point aligns with the raycast hit point
                    Vector3 surfacePointOffset = worldEndPoint - selfHit.point;
                    objectToLock.transform.position += surfacePointOffset;

                    if (alignment)
                    {
                        // Align the object's rotation to the surface normal
                        Quaternion alignmentRotation = Quaternion.FromToRotation(Vector3.up, worldEndPoint - hitInfo.HitObject.position);
                        objectToLock.transform.rotation = alignmentRotation;
                    }

                    // Run a collision check
                    if (collisionCheck)
                    {
                        //need to adjust my bounding box to the new position
                        var aNewBox = FP_UtilityData.CreateBoundingBox(objectToLock, objectRenderer);
                        if(aNewBox!=null)
                        {
                            boundingBox = aNewBox.Value;
                            Debug.Log($"Updated Bounding Box {boundingBox.Center}");
                        }
                        else
                        {
                            Debug.LogError($"Failed to create a new bounding box for object: {objectToLock.name}");
                        }
                        var colliding = IsColliding(objectToLock, boundingBox);
                        Debug.LogWarning($"Colliding: {colliding}");
                        if (colliding)
                        {
                            ResolveCollision(objectToLock, objectRenderer, 0.01f,50);
                        }
                    }
                    
                    return true;
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
        private static bool IsColliding(GameObject objectToLock, FP_BoundingBoxInfo boundingBox)
        {
            Debug.LogWarning($"Checking for Collisions at {boundingBox.Center} with a size {boundingBox.Size / 2}");
            Collider[] colliders = Physics.OverlapBox(
                boundingBox.Center,
                boundingBox.Size / 2,
                objectToLock.transform.rotation
            );

            // Return true if there are any colliders other than the object's own collider
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject != objectToLock)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Attempts to resolve the collision by moving the object in different directions.
        /// Includes a timeout mechanism to stop after a certain number of attempts.
        /// </summary>
        private static void ResolveCollision(GameObject objectToLock, Renderer objectRenderer, float stepSize =0.1f, int maxAttempts =20)
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
                    objectToLock.transform.position += direction * stepSize * (attemptCount+1); // Adjust the step size as needed
                    //reset boundingBox
                    var aNewBox = FP_UtilityData.CreateBoundingBox(objectToLock, objectRenderer);
                    if (aNewBox.HasValue)
                    {
                       
                        Debug.Log($"Test Position: {aNewBox.Value.Center}");
                        if (!IsColliding(objectToLock, aNewBox.Value))
                        {
                            return; // Collision resolved
                        }
                        objectToLock.transform.position -= direction * stepSize * attemptCount; // Revert the move if still colliding
                    }
                    
                }
                
                attemptCount++;
            }

            Debug.LogWarning($"Unable to resolve collision for object: {objectToLock.name} after {maxAttempts} attempts.");
        }
    }
}
