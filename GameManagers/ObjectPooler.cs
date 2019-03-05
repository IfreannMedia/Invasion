using UnityEngine;
// ---------------------------------------------------------------------------------------------------
// CLASS:       ObjectPooler
// DESCRIPTION: Can be called by other scripts to pool any object and set it to a heirarchy in scence
//              Can also re-pool objects, and instantiates as a fallback when you try to use an object 
//              from a pool with no available objects. Uses public static methods
//              I wanted an easily accessible object pooler
// ---------------------------------------------------------------------------------------------------
public class ObjectPooler : MonoBehaviour {

    /// <summary>
    /// Creates a pool of objects with desired size and name. destroyOriginal deletes the original object after the pool is created
    /// </summary>
    /// <param name="objectToPool"></param>
    /// <param name="poolSize"></param>
    /// <param name="poolName"></param>
    /// <param name="destroyOriginal"></param>
    /// <returns></returns>
    public static Transform CreatePool(GameObject objectToPool, int poolSize, string poolName, bool destroyOriginal)
    {
        if (destroyOriginal)
        {
            objectToPool.SetActive(false); // deactivate before pooling
            objectToPool.transform.parent = null; // remove any parent if there is one
            Transform poolParent = new GameObject { name = poolName }.transform;
            for (int i = 0; i < poolSize; i++)
            {
                Instantiate(objectToPool, poolParent.transform, true);
            }
            Destroy(objectToPool);
            return poolParent;

        }
        else
        {
            Transform poolParent = new GameObject { name = poolName }.transform;
            for (int i = 0; i < poolSize; i++)
            {
               GameObject currentObj = Instantiate(objectToPool, poolParent.transform, true);
                currentObj.SetActive(false);
            }
            
            return poolParent;
        }
    }
    /// <summary>
    /// Returns the transform of an object to be used at position, 
    /// returns the object in an active state
    /// Instantiates if no objects are inactive
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Transform UseObject(Transform parent, Vector3 position)
    {

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (!child.gameObject.activeInHierarchy)
            {
                child.position = position;
                child.gameObject.SetActive(true);
                return child;
            }
        }
        // Entering these lines of code means each child object in the pool is active (and presumably in use)
        // If all were enabled and in use simply instantiate a new one as fallback,
        GameObject clone = parent.GetChild(Random.Range(0, parent.childCount)).gameObject;
        Instantiate(clone, parent, true);
        clone.transform.position = position;
        clone.SetActive(true);
        return clone.transform;
    }
    // -------------------------------------------------------------------------------
    // METHOD:      UseObject
    // DESCRIPTION: Pass in the parent transform of the pool and the position you want the object returned to
    //              If the pool has run out of objects to return, a new one will be instantiated
    //              The boolean param allows us to maintain or decouple the parent
    // -------------------------------------------------------------------------------
    //TODO Tidy up this code:
    public static Transform UseObject(Transform parent, Vector3 position, bool keepParent)
    {
            
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (!child.gameObject.activeInHierarchy)
            {
                child.parent = keepParent ? parent : null;
                child.position = position;
                child.gameObject.SetActive(true);
                return child;
            }
        }
        // Entering these lines of code means each child object in the pool is active (and presumably in use)
        // If all were enabled and in use simply instantiate a new one as fallback,
        GameObject clone = parent.GetChild(Random.Range(0, parent.childCount)).gameObject;
        clone = keepParent ? Instantiate(clone, parent, true) : Instantiate(clone, null, true);
        clone.transform.position = position;
        clone.SetActive(true);
        return clone.transform;
    }

    // --------------------------------------------------------------------------------
    // METHOD:      RePool()
    // DESCRIPTION: Once you are finished using an object, use this method to repool it
    //              pass in the object and the pool
    // -------------------------------------------------------------------------------
    public static void RePool( Transform obj, Transform pool)
    {
        obj.gameObject.SetActive(false);
        obj.parent = pool;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S)) returnedPool = CreatePool(testObject, Size, Name,DestroyOriginal);

    //    if (Input.GetKeyDown(KeyCode.U)) usedObject = UseObject(returnedPool.transform, Vector3.zero, true);

    //    if (Input.GetKeyDown(KeyCode.R)) RePool(usedObject, returnedPool.transform);
    //}
}
