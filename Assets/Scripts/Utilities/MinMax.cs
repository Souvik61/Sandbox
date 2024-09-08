using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct MinMax
{
	[SerializeField]
	private int min;
	[SerializeField]
	private int max;

	public int Min
	{
		get
		{
			return this.min;
		}
		set
		{
			this.min = value;
		}
	}

	public int Max
	{
		get
		{
			return this.max;
		}
		set
		{
			this.max = value;
		}
	}

	public int RandomValue
	{
		get
		{
			return UnityEngine.Random.Range(this.min, this.max);
		}
	}

	public MinMax(int min, int max)
	{
		this.min = min;
		this.max = max;
	}

	public int Clamp(int value)
	{
		return Mathf.Clamp(value, this.min, this.max);
	}

}

public class MinMaxSliderAttribute : PropertyAttribute
{

	public readonly int Min;
	public readonly int Max;

	public MinMaxSliderAttribute(int min, int max)
	{
		Min = min;
		Max = max;
	}

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MinMax))]
[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxDrawer : PropertyDrawer
{

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (property.serializedObject.isEditingMultipleObjects) return 0f;
		return base.GetPropertyHeight(property, label) + 16f;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.serializedObject.isEditingMultipleObjects) return;

		var minProperty = property.FindPropertyRelative("min");
		var maxProperty = property.FindPropertyRelative("max");
		var minmax = attribute as MinMaxSliderAttribute ?? new MinMaxSliderAttribute(0, 1);
		position.height -= 16f;

		label = EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		float min = minProperty.intValue;
		float max = maxProperty.intValue;

		var left = new Rect(position.x, position.y, position.width / 2 - 11f, position.height);
		Rect right = new Rect(position.x + position.width - left.width, position.y, left.width, position.height);
		Rect mid = new Rect(left.xMax, position.y, 22, position.height);
		min = Mathf.Clamp(EditorGUI.IntField(left, (int)min), minmax.Min, max);
		EditorGUI.LabelField(mid, " to ");
		max = Mathf.Clamp(EditorGUI.IntField(right, (int)max), min, minmax.Max);

		position.y += 16f;
		EditorGUI.MinMaxSlider(position, GUIContent.none, ref min, ref max, (float)minmax.Min, (float)minmax.Max);

		minProperty.intValue = (int)min;
		maxProperty.intValue = (int)max;
		EditorGUI.EndProperty();
	}

}
#endif