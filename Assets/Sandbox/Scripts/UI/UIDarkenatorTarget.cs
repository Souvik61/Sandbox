using System.Collections;
using System.Collections.Generic;
using System.Windows;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BounceOverrideData
{
    public bool AnimationOverride = false;
    public AnimationCurve Curve;
    public float Duration;
}

[RequireComponent(typeof(GUID))]
public class UIDarkenatorTarget : MonoBehaviour
{
    [SerializeField, ReadOnly] public int GUID;
    [SerializeField] public PNL_Darkenator.MaskMode MaskMode = PNL_Darkenator.MaskMode.Square;
    [Space(10)]
    [SerializeField, ReadOnly] public RectTransform targetRectTransform = null;
    [SerializeField, ReadOnly] public Button Button = null;
    [SerializeField, ReadOnly] public GUID DarkenatorTarget_GUID = null;

    [Header("Animation")]
    [SerializeField, HideInInspector] public bool ShouldBounce = true;

    // Show these in the custom editor
    [HideInInspector] public BounceOverrideData BounceOverrideData = new BounceOverrideData();
    // Custom Mask Sprite
    [HideInInspector] public Sprite CustomMask = null;
    [HideInInspector] public float pixelsPerUnitMultiplier = -1;

    protected void Reset()
    {
        if (DarkenatorTarget_GUID != null)
            DarkenatorTarget_GUID.Init();
        Init();
    }
    protected void OnValidate()
    {
        Init();
        GUID = DarkenatorTarget_GUID.HashCode;
    }

    protected virtual void Init()
    {
        Button = GetComponent<Button>();
        targetRectTransform = GetComponent<RectTransform>();
        DarkenatorTarget_GUID = GetComponent<GUID>();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIDarkenatorTarget))]
public class UIDarkenatorTarget_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UIDarkenatorTarget uIDarkenatorTarget = target as UIDarkenatorTarget;
        if (uIDarkenatorTarget != null)
        {
            if (uIDarkenatorTarget.MaskMode == PNL_Darkenator.MaskMode.Custom)
            {
                // Show a sprite object field to recieve a sprite
                Undo.RecordObject(uIDarkenatorTarget, "Setting Sprite");
                uIDarkenatorTarget.CustomMask = EditorGUILayout.ObjectField("Custom Mask", uIDarkenatorTarget.CustomMask, typeof(Sprite), false) as Sprite;
                uIDarkenatorTarget.pixelsPerUnitMultiplier = EditorGUILayout.FloatField("Pixels Per Unit Multiplier", uIDarkenatorTarget.pixelsPerUnitMultiplier);
            }
            else
            {
                // Make sure you reset it as to ensure no memory loss
                uIDarkenatorTarget.CustomMask = null;
            }

            EditorGUILayout.LabelField("Animation");

            EditorGUI.BeginChangeCheck();
            {
                uIDarkenatorTarget.ShouldBounce = EditorGUILayout.Toggle("Should Bounce", uIDarkenatorTarget.ShouldBounce);

                if (uIDarkenatorTarget.ShouldBounce)
                {
                    uIDarkenatorTarget.BounceOverrideData.AnimationOverride = EditorGUILayout.Toggle("Override Animation", uIDarkenatorTarget.BounceOverrideData.AnimationOverride);
                    if (uIDarkenatorTarget.BounceOverrideData.AnimationOverride)
                    {
                        uIDarkenatorTarget.BounceOverrideData.Curve = EditorGUILayout.CurveField("Animation Override Curve", uIDarkenatorTarget.BounceOverrideData.Curve);
                        uIDarkenatorTarget.BounceOverrideData.Duration = EditorGUILayout.FloatField("Duration Override", uIDarkenatorTarget.BounceOverrideData.Duration);
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(uIDarkenatorTarget);

            if (GUILayout.Button($"Make This Main Darkenator For Target {uIDarkenatorTarget.GUID}"))
            {
                // Get all objects of type Darkenator Target in the scene and destroy them all
                var AllTargets = GetAllObjectsOnlyInScene<UIDarkenatorTarget>();
                if (AllTargets != null && AllTargets.Count > 0)
                {
                    // Make a sub dialog box that is are you sure you want destroy all the other darkenator targets at these objects
                    string message = "";
                    foreach (var item in AllTargets)
                    {
                        if (item != uIDarkenatorTarget && item.GUID == uIDarkenatorTarget.GUID)
                            message += item.name + " : " + item.GUID + Environment.NewLine;
                    }

                    if (EditorUtility.DisplayDialog("Are you sure you want to destroy all these?", message, "Delete them all", "Cancel"))
                    {
                        foreach (var item in AllTargets)
                        {
                            if (item != uIDarkenatorTarget && item.GUID == uIDarkenatorTarget.GUID)
                                DestroyImmediate(item);
                        }
                    }
                }
            }
        }
    }

    private static List<T> GetAllObjectsOnlyInScene<T>() where T : MonoBehaviour
    {
        List<T> objectsInScene = new List<T>();

        foreach (T go in Resources.FindObjectsOfTypeAll(typeof(T)) as T[])
        {
            if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(go);
        }

        return objectsInScene;
    }
}
#endif