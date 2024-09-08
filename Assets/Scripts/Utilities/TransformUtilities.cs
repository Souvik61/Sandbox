using UnityEngine;
using UnityEditor;

/// <summary>
/// Provides a set of transform utility methods.
/// </summary>

[ExecuteInEditMode]
public static class TransformUtilities
{

    /// <summary>
    /// Aligns one transform's pos and rot to another
    /// </summary>
    public static void AlignTransforms(Transform followT, Transform targetT)
    {
        if (followT != null && targetT != null)
        {
            followT.position = targetT.position;
            followT.rotation = targetT.rotation;
        }
    }

    /// <summary>
    /// Aligns one transform's pos and rot to another
    /// </summary>
    public static void CopyTransform(Transform followT, Transform targetT)
    {
        if (followT != null && targetT != null)
        {
            followT.position = targetT.position;
            followT.rotation = targetT.rotation;
            followT.localScale = targetT.localScale;
        }
    }



    /// <summary>
    /// Swap two transform's pos, rot and scale
    /// </summary>
    public static void SwapTransforms(Transform followT, Transform targetT)
    {
        if (followT != null && targetT != null)
        {
            Vector3 tempTargetP = targetT.position;
            Quaternion tempTargetR = targetT.rotation;
            Vector3 tempTargetS = targetT.localScale;

            Vector3 tempFollowP = followT.position;
            Quaternion tempFollowR = followT.rotation;
            Vector3 tempFollowS = followT.localScale;

            followT.position = tempTargetP;
            followT.rotation = tempTargetR;
            followT.localScale = tempTargetS;

            targetT.position = tempFollowP;
            targetT.rotation = tempFollowR;
            targetT.localScale = tempFollowS;
        }
    }

    /// <summary>
    /// Reset a transform's pos, rot and scale
    /// </summary>
    public static void ResetTransform(Transform t)
    {
        if (t != null)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

        }
    }

    /// <summary>
    /// Child a Transform to another and reset their transform
    /// </summary>
    public static void ChildToTransformAndReset(Transform c, Transform p)
    {
        if (c != null && p != null)
        {
            c.parent = p;
            ResetTransform(c);
        }
    }
}
