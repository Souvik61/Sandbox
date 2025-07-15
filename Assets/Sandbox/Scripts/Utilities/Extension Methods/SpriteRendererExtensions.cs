using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension methods for sprite renderers.
/// </summary>
public static class SpriteRendererExtensions
{
	/// <summary>
	/// Sets the alpha value of the sprite.
	/// </summary>
	public static void SetAlpha(this SpriteRenderer _spriteRenderer, float _alpha)
	{
		Color col = _spriteRenderer.color;
		col.a = _alpha;
		_spriteRenderer.color = col;
	}

	/// <summary>
	/// Sets the color of the sprite while preserving its current alpha value.
	/// </summary>
	public static void SetColorOnly(this SpriteRenderer _spriteRenderer, Color _color)
	{
		_color.a = _spriteRenderer.color.a;
		_spriteRenderer.color = _color;
	}
}
