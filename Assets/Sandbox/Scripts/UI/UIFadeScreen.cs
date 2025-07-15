using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeScreen : Singleton<UIFadeScreen>
{
    private const float DEFAULT_FADETIME = 0.5f;
    private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            return canvasGroup;
        }
    }

	public Coroutine CurrentFadeCoroutine {
		get
		{
			return currentFadeCoroutine;
		}
		set
		{
			if (currentFadeCoroutine != null)
				Instance.StopCoroutine(currentFadeCoroutine);

			currentFadeCoroutine = value; }
		}

	public enum FadingState
    {
        FadingDown,
        FadingUp,
        FadedUp,
        FadedDown,
    }

	private Coroutine currentFadeCoroutine = null;
    public FadingState currentState = FadingState.FadedUp;

    protected override void Awake()
    {
        base.Awake();
    }

    public static Coroutine FadeDown(float fadeTime = DEFAULT_FADETIME, Action callback = null)
    {
        if (InstanceValid)
        {
            if (Instance.currentState == FadingState.FadedUp)
            {
                return Instance.CurrentFadeCoroutine = Instance.StartCoroutine(Instance.FadeDownCoroutine(fadeTime, callback));
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public static Coroutine FadeUp(float fadeTime = DEFAULT_FADETIME)
    {
		if (InstanceValid)
		{
			if (Instance.currentState == FadingState.FadedDown)
				return Instance.CurrentFadeCoroutine = Instance.StartCoroutine(Instance.FadeUpCoroutine(fadeTime));
			else
				return null;
		}
		else
		{
			return null;
		}
    }
	public static Coroutine FadeUp_Force(float fadeTime = DEFAULT_FADETIME)
	{
        if (InstanceValid)
        {
            if (Instance.CurrentFadeCoroutine != null) Instance.StopCoroutine(Instance.CurrentFadeCoroutine);
            return Instance.CurrentFadeCoroutine = Instance.StartCoroutine(Instance.FadeUpCoroutine(fadeTime));
        }

        return null;
	}

    public static Coroutine FadeDown_Force(float fadeTime = DEFAULT_FADETIME, Action callback = null)
    {
        if (InstanceValid)
        {
            if (Instance.CurrentFadeCoroutine != null) Instance.StopCoroutine(Instance.CurrentFadeCoroutine);
            return Instance.CurrentFadeCoroutine = Instance.StartCoroutine(Instance.FadeDownCoroutine(fadeTime, callback));
        }

        return null;
    }

    private IEnumerator FadeDownCoroutine(float fadeTime, Action callback)
    {
        float timer = 0;
        currentState = FadingState.FadingDown;
        while (CanvasGroup.alpha != 1)
        {
            CanvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.Clamp01(timer / fadeTime));
            timer += Time.deltaTime;
            yield return null;
        }
        currentState = FadingState.FadedDown;
        Show();
		currentFadeCoroutine = null;
        callback?.Invoke();
	}
    private IEnumerator FadeUpCoroutine(float fadeTime)
    {
        float timer = 0;
        currentState = FadingState.FadingUp;
        while (CanvasGroup.alpha != 0)
        {
            CanvasGroup.alpha = Mathf.Lerp(1, 0, Mathf.Clamp01(timer / fadeTime));
            timer += Time.deltaTime;
            yield return null;
        }
        currentState = FadingState.FadedUp;
        Hide();
		currentFadeCoroutine = null;
	}

	public void Show()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
    }
    public void Hide()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }
}