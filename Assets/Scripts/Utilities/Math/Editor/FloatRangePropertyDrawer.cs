using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom property drawer to nicely disable float ranges
/// </summary>
[CustomPropertyDrawer(typeof(FloatRange))]
public class FloatRangePropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var minLabel = new Rect(position.x, position.y, 30, position.height);
		var xRect = new Rect(position.x + 30, position.y, 40, position.height);
		var maxLabel = new Rect(position.x + 75, position.y, 30, position.height);
		var yRect = new Rect(position.x + 105, position.y, 40, position.height);

		EditorGUI.LabelField(minLabel, "Min");
		EditorGUI.PropertyField(xRect, property.FindPropertyRelative("Min"), GUIContent.none);
		EditorGUI.LabelField(maxLabel, "Max");
		EditorGUI.PropertyField(yRect, property.FindPropertyRelative("Max"), GUIContent.none);

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}