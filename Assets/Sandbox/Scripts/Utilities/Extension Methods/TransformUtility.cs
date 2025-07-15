#if UNITY_EDITOR

using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtility
{
    [MenuItem("CONTEXT/Transform/Copy/Position")]
    public static void CopyTransformPosition()
    {
        Transform transform = Selection.activeTransform;
        EditorGUIUtility.systemCopyBuffer = $"{transform.position.x}f, {transform.position.y}f, {transform.position.z}f";
    }

    [MenuItem("CONTEXT/Transform/Copy/Rotation - Euler")]
    public static void CopyTransformRotationEuler()
    {
        Transform transform = Selection.activeTransform;
        EditorGUIUtility.systemCopyBuffer = $"{transform.rotation.eulerAngles.x}f, {transform.rotation.eulerAngles.y}f, {transform.rotation.eulerAngles.z}f";
    }

    [MenuItem("CONTEXT/Transform/Copy/Rotation - Quaternion")]
    public static void CopyTransformRotationQuaternion()
    {
        Transform transform = Selection.activeTransform;
        EditorGUIUtility.systemCopyBuffer = $"{transform.rotation.x}f, {transform.rotation.y}f, {transform.rotation.z}f, {transform.rotation.w}f";
    }

    [MenuItem("CONTEXT/Transform/Copy/Scale")]
    public static void CopyTransformScale()
    {
        Transform transform = Selection.activeTransform;
        EditorGUIUtility.systemCopyBuffer = $"{transform.localScale.x}f, {transform.localScale.y}f, {transform.localScale.z}f";
    }
}
#endif