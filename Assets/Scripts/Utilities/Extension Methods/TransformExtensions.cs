using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contains unity transform extension methods.
/// </summary>
public static class TransformExtensions
{
    /// <summary>
    /// Performs a recursive search for a child of the transform with the given name.
    /// Returns null if no child could be find.
    /// </summary>
    public static Transform FindDeep(this Transform transform, string name)
    {
        // first search base children
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).name == name) return transform.GetChild(i);
        }

        // next search children's children
        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = FindDeep(transform.GetChild(i), name);
            if (child != null) return child;
        }

        // Return null if none found
        return null;
    }

    /// <summary>
    /// Returns a full string path of the given transform.
    /// </summary>
    public static string GetTransformPath(this Transform transform)
    {
        string path = "/" + transform.gameObject.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = "/" + transform.gameObject.name + path;
        }
        return path;
    }

    /// <summary>
    /// Recursively iterates and returns all children of this transform (depth first).
    /// </summary>
    public static IEnumerable<Transform> ChildrenDeep(this Transform transform)
	{
        foreach (Transform child in transform)
		{
            yield return child;
            foreach (Transform subChild in ChildrenDeep(child))
			{
                yield return subChild;
			}
		}
	}
}
