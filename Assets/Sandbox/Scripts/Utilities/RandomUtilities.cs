using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a set of random utility methods.
/// </summary>
public class RandomUtilities
{
    /// <summary>
    /// Returns a random value in the given range for the given seed. Resets the random
    /// number back to fully random afterwards.
    /// </summary>
    public static float SeededRandomRange(int _seed, float _min, float _max)
    {
        int nextSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(_seed);
        float value = Random.Range(_min, _max);
        Random.InitState(nextSeed);
        return value;
    }

	/// <summary>
	/// Returns a random value in the given range for the given seed. Resets the random
	/// number back to fully random afterwards.
	/// </summary>
	public static int SeededRandomRange(int _seed, int _min, int _max)
	{
		int nextSeed = Random.Range(int.MinValue, int.MaxValue);
		Random.InitState(_seed);
		int value = Random.Range(_min, _max);
		Random.InitState(nextSeed);
		return value;
	}
}
