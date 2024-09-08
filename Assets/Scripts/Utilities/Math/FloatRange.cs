using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores min and max float values.
/// </summary>
[System.Serializable]
public struct FloatRange
{
	public float Min;
	public float Max;

	/// <summary>
	/// Initialises the float range with min and max values.
	/// </summary>
	public FloatRange(float min, float max)
	{
		Min = min;
		Max = max;
	}

	/// <summary>
	/// The center value between the min and max
	/// </summary>
	public float Center => (Max + Min) / 2;

	/// <summary>
	/// Returns random value between the min and max
	/// </summary>
	public float GetRand() => Random.Range(Min, Max);

	/// <summary>
	/// Returns interpolated value between min and max from amount (0 - 1).
	/// </summary>
	public float Lerp(float _amount) => Mathf.Lerp(Min, Max, _amount);

	/// <summary>
	/// Returns normalized position for given values position between min and max range.
	/// </summary>
	public float InverseLerp(float _value) => Mathf.InverseLerp(Min, Max, _value);

	public override string ToString()
	{
		return $"{Min:0.000} - {Max:0.000}";
	}
}