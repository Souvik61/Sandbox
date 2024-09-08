using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds data of a quadratic equation.
/// </summary>
[System.Serializable]
public struct Quadratic
{
	/// <summary>
	/// Returns lerped quadratic between the given two.
	/// </summary>
	public static Quadratic Lerp(Quadratic _a, Quadratic _b, float _time)
	{
		return new Quadratic
		{
			a = Mathf.Lerp(_a.a, _b.a, _time),
			b = Mathf.Lerp(_a.b, _b.b, _time),
			c = Mathf.Lerp(_a.c, _b.c, _time)
		};
	}

	public float a;
	public float b;
	public float c;

	/// <summary>
	/// Evaluate the quadratic equation for the given value of x.
	/// </summary>
	public float Evaluate(float _x)
	{
		return (a * (_x * _x)) + (b * _x) + c;
	}

	/// <summary>
	/// Returns the two possible values for x for the given value of y.
	/// </summary>
	public Vector2 Solve(float _y)
	{
		float sqrt = Mathf.Sqrt((b * b) - (4 * a * (c - _y)));
		float denonminator = 2 * a;
		return new Vector2((-b - sqrt) / denonminator,
			(-b + sqrt) / denonminator);
	}

	/// <summary>
	/// Returns the coordinates of the turning point of this quadratic.
	/// </summary>
	public Vector2 TurningPoint()
	{
		float x = -b / (2 * a);
		return new Vector2(x, Evaluate(x));
	}

	public override string ToString()
	{
		return $"{a:0.000}x^2 + {b:0.000}x + {c:0.000}";
	}
}
