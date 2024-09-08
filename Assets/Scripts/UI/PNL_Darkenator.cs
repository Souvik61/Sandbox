using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PNL_Darkenator : Panel
{
    public enum MaskMode
    {
        None = -1,
        Square,
        Circle,
        RightLeaningSquare,
        Custom,
    }

    [Serializable]
    public struct MaskData
    {
        public MaskMode maskMode;
        public Sprite maskSprite;
        public Sprite highlightSprite;
        public float pixelsPerUnityMultiplier;
    }
       

    [Header("Target")]
    [SerializeField] private RectTransform targetingRect;
    [SerializeField] private Image maskImage;
    [SerializeField] private Image highlightImage;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject highlightParent;

    public static bool IsActive { get; private set; }

    private RectTransform buttonRect;
    private Action onSelect;

    private List<RectTransform> buttonRectQueue = null;
    private List<Action> onSelectQueue = null;

    [Header("Animations")]
    [SerializeField] private AnimationCurve AnimationCurve;
    [SerializeField] private float Duration = 1.0f;

    [Header("Mask Data")]
    public List<MaskData> maskDatas = new List<MaskData>();

    private MaskMode maskMode = MaskMode.None;

    private int SH_Enabled = "Enabled".ToHash();
    private bool IsEnabled => animator.IsInState(SH_Enabled);

    private Vector3 startingTargetScale;

    private Coroutine bounceCoroutine = null;

    public override void Init()
    {
        base.Init();
        SetMaskMode(MaskMode.Circle);
        IsActive = false;
        highlightParent.SetActive(false);
    }

    private void Update()
    {
        if (buttonRect)
            MatchRect();
    }

    /// <summary>
    /// Match the highlight rect to the desired size
    /// </summary>
    private void MatchRect()
    {
        if (targetingRect != null && buttonRect != null)
        {
            targetingRect.position = new Vector3(buttonRect.position.x, buttonRect.position.y, targetingRect.position.z);
            targetingRect.anchoredPosition += buttonRect.rect.center;
            targetingRect.sizeDelta = buttonRect.rect.size;
        }
    }

    /// <summary>
    /// Enable the Force select
    /// </summary>
    /// <param name="_buttonRect"></param>
    /// <param name="_OnSelect"></param>
    public Coroutine Enable(RectTransform _buttonRect, bool highlight = false, Action _OnSelect = null, MaskMode _MaskMode = MaskMode.Square, Sprite _CustomMask = null, float pixelsMultiplierOverride = -1, bool ShouldBounce = true, BounceOverrideData _BounceOverrideData = null)
    {
        SetMaskMode(_MaskMode, _CustomMask, pixelsMultiplierOverride);

        onSelect = _OnSelect;
        PanelManager.EnableScreen(PanelID.None);
        buttonRect = _buttonRect;
        MatchRect();
        IsActive = true;

        // Turn on the highlight if you want the user to press a button
        highlightParent.SetActive(highlight);

        if (buttonRect != null)
        {
            if (bounceCoroutine != null)
            {
                StopCoroutine(bounceCoroutine);
                bounceCoroutine = null;

                // Reset Targetting rect scale
                targetingRect.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        // Start coroutine to bounce the target 
        if (ShouldBounce)
        {
            if (_BounceOverrideData != null && _BounceOverrideData.AnimationOverride)
            {
                buttonRect.transform.localScale = startingTargetScale;
                bounceCoroutine = StartCoroutine(BounceCoroutine(_buttonRect, _BounceOverrideData));
            }
            else
            {
                BounceOverrideData newBounceOverrideData = new BounceOverrideData()
                {
                    Duration = Duration,
                    Curve = this.AnimationCurve
                };
                bounceCoroutine = StartCoroutine(BounceCoroutine(_buttonRect, newBounceOverrideData));
            }
        }

        // Wait For The Darkenator To Enable And Focus On That Target
        return StartCoroutine(WaitForEnable());
    }

    /// <summary>
    /// Enable the Force Select via an Array
    /// </summary>
    /// <param name="_buttonRects"></param>
    /// <param name="_onSelects"></param>
    public void Enable(RectTransform[] _buttonRects, Action[] _onSelects, MaskMode maskMode = MaskMode.Square)
    {
        if (_buttonRects.Length != _onSelects.Length)
        {
            Debug.LogError("Force select input queues not equal length, ignoring");
            return;
        }

        buttonRectQueue = new List<RectTransform>(_buttonRects);
        onSelectQueue = new List<Action>(_onSelects);
        Enable(buttonRectQueue.Dequeue(), false, onSelectQueue.Dequeue());
    }

    public Coroutine Enable(UIDarkenatorTarget _UIDarkenatorTarget, bool highlight = false, Action _OnSelect = null)
    {
        return Enable(_UIDarkenatorTarget.targetRectTransform, highlight,  _OnSelect, _UIDarkenatorTarget.MaskMode, _UIDarkenatorTarget.CustomMask, _UIDarkenatorTarget.pixelsPerUnitMultiplier, _UIDarkenatorTarget.ShouldBounce, _UIDarkenatorTarget.BounceOverrideData);
    }

    private IEnumerator WaitForEnable()
    {
        while (true)
        {
            if (PanelManager.IsScreenEnabled(PanelID.None))
                break;
            else
                yield return null;
        }
    }

    /// <summary>
    /// Disable the Force Select
    /// </summary>
    public void Disable()
    {
        if (buttonRectQueue != null && buttonRectQueue.Count > 0)
        {
            Enable(buttonRectQueue.Dequeue(), false, onSelectQueue.Dequeue());

            StopCoroutine(bounceCoroutine);
            bounceCoroutine = null;

            // Reset scale
            buttonRect.transform.localScale = startingTargetScale;

            // Reset Targetting rect scale
            targetingRect.transform.localScale = new Vector3(1, 1, 1);
            return;
        }

        onSelect?.Invoke();
        onSelect = null;
        buttonRect = null;
        IsActive = false;

        PanelManager.DisableScreen(PanelID.None);
    }

    public void SetMaskMode(MaskMode _MaskMode, Sprite _CustomMask = null, float pixelsPerUnitMultiplierOverride = -1)
    {
        // Handle Custom Mask Before Other Types, always update custom types
        if (_MaskMode == MaskMode.Custom && _CustomMask != null)
        {
            maskMode = _MaskMode;

            MaskData maskData = maskDatas.Find((x) => x.maskMode == _MaskMode);
            Sprite highlightSprite = maskData.highlightSprite;

            maskImage.sprite = _CustomMask;
            highlightImage.sprite = highlightSprite;

            if (pixelsPerUnitMultiplierOverride < 0)
            {
                maskImage.pixelsPerUnitMultiplier = maskData.pixelsPerUnityMultiplier;
                highlightImage.pixelsPerUnitMultiplier = maskData.pixelsPerUnityMultiplier;
            }
            else
            {
                maskImage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplierOverride;
                highlightImage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplierOverride;
            }

        }
        // Handle other types
        else
        {
            maskMode = _MaskMode;

            MaskData maskData = maskDatas.Find((x) => x.maskMode == _MaskMode);
            Sprite maskSprite = maskData.maskSprite;
            Sprite highlightSprite = maskData.highlightSprite;

            maskImage.sprite = maskSprite;
            highlightImage.sprite = highlightSprite;

            if (_CustomMask == null || pixelsPerUnitMultiplierOverride < 0)
            {
                maskImage.pixelsPerUnitMultiplier = maskData.pixelsPerUnityMultiplier;
                highlightImage.pixelsPerUnitMultiplier = maskData.pixelsPerUnityMultiplier;
            }
            else
            {
                maskImage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplierOverride;
                highlightImage.pixelsPerUnitMultiplier = pixelsPerUnitMultiplierOverride;
            }
        }
    }

    private IEnumerator BounceCoroutine(RectTransform _TargetTransformToBounce, BounceOverrideData _OverrideData)
    {
        startingTargetScale = _TargetTransformToBounce.transform.localScale;
        float timer = 0;
        while (true)
        {
            timer = Mathf.PingPong(Time.unscaledTime, _OverrideData.Duration);
            float evaluation = _OverrideData.Curve.Evaluate(timer / Duration);
            Vector3 newScale = startingTargetScale * evaluation;
            _TargetTransformToBounce.transform.localScale = newScale;
            targetingRect.transform.localScale = newScale;
            yield return null;
        }
    }
}

