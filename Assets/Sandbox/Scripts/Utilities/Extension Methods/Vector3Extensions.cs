using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains extension methods for Vector3 class.
/// </summary>
public static class Vector3Extensions
{
	/// <summary>
	/// Returns the 2d distance between the two 3d vectors, ignoring any distance on the y-axis.
	/// </summary>
	public static float Distance2D(Vector3 _a, Vector3 _b)
	{
		return Vector2.Distance(_a.To2D(), _b.To2D());
	}

	/// <summary>
	/// Converts the given vector to a 2d vector using the x and z components (discarding the y component).
	/// </summary>
	public static Vector2 To2D(this Vector3 _vec)
	{
		return new Vector2(_vec.x, _vec.z);
	}
}
