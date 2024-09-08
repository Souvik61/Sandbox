using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains extension methods for Image Ui class.
/// </summary>
public static class ImageExtensions
{
	/// <summary>
	/// Sets the alpha value of the image.
	/// </summary>
	public static void SetAlpha(this Image _image, float _alpha)
	{
		Color col = _image.color;
		col.a = _alpha;
		_image.color = col;
	}
}
