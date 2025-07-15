using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains basic math utility functions.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Used to determine if a float is above zero.
    /// </summary>
    private const float ZeroOffset = 0.001f;

    /// <summary>
    /// Returns whether the given float has a significant magnitude above or below zero.
    /// </summary>
    public static bool HasMagnitude(float _value) => Mathf.Abs(_value) > ZeroOffset;

    /// <summary>
    /// Returns whether the given Vector3s are the same with a lower tolerance than Unity's built in function.
    /// </summary>
    public static bool CompareVectors(Vector3 lhs, Vector3 rhs)
    {
        return !HasMagnitude(Vector3.SqrMagnitude(lhs - rhs));
    }

    /// <summary>
    /// Returns whether the given Quaternions are the same with a lower tolerance than Unity's built in function.
    /// </summary>
    public static bool CompareQuaternions(Quaternion lhs, Quaternion rhs)
    {
        Vector3 vlhs = lhs.eulerAngles;
        Vector3 vrhs = rhs.eulerAngles;

        return !HasMagnitude(Vector3.SqrMagnitude(vlhs - vrhs));
    }
}
