using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains basic easing functions for values between 0 - 1.
/// </summary>
public static class Easing
{
    /// <summary>
    /// Smoothly eases t in between 0 - 1.
    /// </summary>
    public static float EaseInQuad(float t)
    {
        return t * t;
    }

    /// <summary>
    /// Smoothly eases t in between 0 - 1.
    /// </summary>
    public static float EaseInQuint(float t)
    {
        return t * t * t;
    }


    public static float EaseInOutQuint(float t)
    {
        return t < 0.5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
    }
    /// <summary>
    /// Smoothly eases t out between 0 - 1.
    /// </summary>
    public static float EaseOutQuad(float t)
    {
        return -t * (t - 2);
    }

    /// <summary>
    /// Smoothly eases 't' in and out between 0 - 1.
    /// </summary>
    public static float InOutQuad(float t)
    {
        t *= 2;
        if (t < 1) return 0.5f * t * t;
        t--;
        return -0.5f * (t * (t - 2) - 1);
    }

    /// <summary>
    /// Smoothly eases t out between 0 - 1 as a paramatic equation
    /// </summary>
    public static float ParametricInOut(float t)
    {
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }
}
