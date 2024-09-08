using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains various extension methods for colors.
/// </summary>
public static class ColorExtensions
{
	public static Color SmoothDamp(Color _current, Color _target, ref Color _velocity, float _smoothTime)
	{
		return new Color(Mathf.SmoothDamp(_current.r, _target.r, ref _velocity.r, _smoothTime),
			Mathf.SmoothDamp(_current.g, _target.g, ref _velocity.g, _smoothTime),
			Mathf.SmoothDamp(_current.b, _target.b, ref _velocity.b, _smoothTime),
			Mathf.SmoothDamp(_current.a, _target.a, ref _velocity.a, _smoothTime));
	}
}
