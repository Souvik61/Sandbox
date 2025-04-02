using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Recycler designed to recycle a specific prefab gameobject. Handles instantiating the prefab, and 
/// activating / deactivating the gameobject when it is used and released.
/// </summary>
public class PrefabRecycler
{
    /// <summary>
    /// The max desired capactiy of this recycler. If the number of objects exceeds this number, any inactive objects
    /// will instantly be destroyed.
    /// </summary>
    private int desiredMaxCapacity;

    /// <summary>
    /// Stack of allocated objects that are not currently activate but available when a new one is requested
    /// </summary>
    private Stack<GameObject> pendingObjects = new Stack<GameObject>();

    /// <summary>
    /// List of currently activated objects
    /// </summary>
    private List<GameObject> activeObjects = new List<GameObject>();

    private GameObject prefab;
    private Transform parent;

    /// <summary>
    /// Instantiates the prefab recycler
    /// </summary>
    /// <param name="_prefab">The prefab to instantiate</param>
    /// <param name="_parent">Optional parent that all newly created objects are instantiated under 
    /// (they are moved there each time they are activated)</param>
    /// <param name="_desiredMaxCapacity">Max desired capacity of this recycler</param>
    public PrefabRecycler(GameObject _prefab, Transform _parent = null, int _desiredMaxCapacity = int.MaxValue)
    {
        desiredMaxCapacity = _desiredMaxCapacity;
        prefab = _prefab;
        parent = _parent;
    }

    /// <summary>
    /// Returns a new, activated object for use. It may allocate a brand new object or 
    /// return a previously deactivated one if it exists.
    /// </summary>
    /// <returns>A new object to use</returns>
    public GameObject GetNew()
    {
        GameObject newObj;
        if (pendingObjects.Count == 0)
        {
            newObj = AllocateNew();
            Activate(newObj);
            activeObjects.Add(newObj);
        }
        else
        {
            newObj = pendingObjects.Pop();
            Activate(newObj);
        }

        return newObj;
    }

    /// <summary>
    /// Releases an object that is no longer needed. Will deactivate the object and may destroy it
    /// if the current object count is greater then the max desired capacity.
    /// </summary>
    /// <param name="_obj">The object to release</param>
    public void Release(GameObject _obj)
    {
        Deactivate(_obj);
        pendingObjects.Push(_obj);
        DestroyOverflowObjects();
    }

    /// <summary>
    /// Destroys all objects currently managed by this recycler.
    /// </summary>
    /// <param name="_includeAlive">Whether objects that are currently alive should also be destroyed
    /// or false if only destroy inactive objects</param>
    public void DestroyAll(bool _includeAlive)
    {
        while (pendingObjects.Count > 0)
        {
            Destroy(pendingObjects.Pop());
        }

        if (_includeAlive)
        {
            foreach (var obj in activeObjects)
            {
                Destroy(obj);
            }

            activeObjects.Clear();
        }
    }

    /// <summary>
    /// Activates an object that may have previously been deactivated
    /// </summary>
    /// <param name="_obj">The object to deactivate</param>
    private void Activate(GameObject _obj)
    {
        if (parent != null)
        {
            _obj.transform.SetParent(parent, false);
            _obj.transform.localPosition = Vector3.zero;
        }
        else
        {
            _obj.transform.position = Vector3.zero;
        }

        _obj.SetActive(true);
    }

    /// <summary>
    /// Allocates a brand new object
    /// </summary>
    /// <returns>The new object</returns>
    private GameObject AllocateNew()
    {
        var newObj = Object.Instantiate(prefab);
        newObj.transform.position = Vector3.zero;
        newObj.transform.rotation = Quaternion.identity;

        if (parent != null)
        {
            newObj.transform.SetParent(parent, false);
            newObj.transform.localPosition = Vector3.zero;
        }

        return newObj;
    }

    /// <summary>
    /// Deactivates an object when it is no longer in use, but may be re-activated in the future
    /// </summary>
    /// <param name="_obj">The object to deactivate</param>
    private void Deactivate(GameObject _obj)
    {
        _obj.SetActive(false);
        if (parent != null) _obj.transform.SetParent(parent, false);
    }

    /// <summary>
    /// Fully destroys an object when it will no longer be used
    /// </summary>
    /// <param name="_obj">The object to destroy</param>
    private void Destroy(GameObject _obj)
    {
        GameObject.Destroy(_obj);
    }

    /// <summary>
    /// If the number of objects exceeds the max desired capacity, destroys an appropriate number
    /// of deactivated objects.
    /// </summary>
    private void DestroyOverflowObjects()
    {
        if (pendingObjects.Count > desiredMaxCapacity)
        {
            int destroyCount = pendingObjects.Count - desiredMaxCapacity;
            for (int i = 0; i < destroyCount; ++i)
            {
                Destroy(pendingObjects.Pop());
            }
        }
    }
}