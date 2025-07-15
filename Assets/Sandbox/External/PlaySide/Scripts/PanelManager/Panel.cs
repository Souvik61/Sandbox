using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

[RequireComponent (typeof (Animator), typeof (CanvasGroup))]
public class Panel : MonoBehaviour
{
	public const string onTransitionName = "On";
	public const string onIdleTransitionName = "OnIdle";
	public const string offTransitionName = "Off";
	public const string offIdleTransitionName = "OffIdle";

	#region Inspector variables
	// If you receieve error: "The type or namespace name `PanelID' could not be found."
	public PanelID id;

	[Header("On Enable")]
	public bool addToNavigationHistory = true;
	public GameObject[] enableObjectsOnEnable;
	public PanelID[] otherPanelsToShow;
	public PanelID[] otherPanelsToNotShow;
	[SerializeField] protected UnityEvent onTransitionOn;

	[Header("On Disable")]
	public PanelID[] otherPanelsToDisableWithThis;
	[SerializeField] protected UnityEvent onTransitionOff;
	#endregion  // Inspector variables

	public System.Action disableCallback;
	public Animator anim { get; private set; }

	CanvasGroup canvasGroup;
	bool startupComplete;

	/// <summary> Called from PanelManager's Awake() </summary>
	public virtual void Init()
	{
		CheckCacheAnim();

		startupComplete = false;

		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
				throw new UnityException("Could not find Canvas Group component on '" + gameObject.name + "'");
		}

		canvasGroup.alpha = 0.0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	protected void OnScreenShow() { }

	/// <summary> Ensures the animator is locally cached if necessary </summary>
	void CheckCacheAnim()
	{
		if (anim == null)
		{
			anim = GetComponent<Animator>();
			if (anim == null)
				throw new UnityException("Could not find Animator component on '" + gameObject.name + "'");
		}
	}

	/// <summary> Called when object/script is enabled in the hierarchy </summary>
	protected virtual void OnEnable()
	{
		for (int i = 0; i < enableObjectsOnEnable.Length; i++)
			enableObjectsOnEnable[i].SetActive(true);
	}

	/// <summary> Called when object/script is disabled in the hierarchy </summary>
	protected virtual void OnDisable()
	{
		if (disableCallback != null)
			PanelManager.Instance.CallOnNextFrame(disableCallback);
	}
/*
	/// <summary> Unused(?) </summary>
	public void RefreshAnimator()
	{
		CheckCacheAnim();
	}
*/
	/// <summary> Starts the specified transition </summary>
	/// <param name="_transitionName"> Transition to play </param>
	public void Transition(string _transitionName)
	{
		CheckCacheAnim();

		startupComplete = true;

		// Turning on?
		if (_transitionName != offTransitionName)
		{
			// Enable GameObject?
			if (!gameObject.activeSelf)
				gameObject.SetActive(true);

			// Add to "Back" button history?
			if (addToNavigationHistory)
				PanelManager.Instance.AddScreenToNavHistory(id);

			// Paired panels to show
			for (int i = 0; i < otherPanelsToShow.Length; ++i)
			{
				if (PanelManager.Instance.panels[otherPanelsToShow[i]].gameObject.activeSelf != true)
					PanelManager.EnableScreen(otherPanelsToShow[i]);
			}

			// Paired panels to not show
			for (int i = 0; i < otherPanelsToNotShow.Length; ++i)
				PanelManager.DisableScreen(otherPanelsToNotShow[i]);

			onTransitionOn.Invoke();
		}
		else
		{
			//Turning off
			for (int i = 0; i < otherPanelsToDisableWithThis.Length; i++)
			{
				PanelManager.DisableScreen(otherPanelsToDisableWithThis[i]);
			}

			onTransitionOff.Invoke();
		}

		// Start transition if active
		if (gameObject.activeSelf)
			anim.Play(_transitionName, 0, 0f);
	}

	/// <summary> Triggered from Start animation </summary>
	public void OnStartupComplete()
	{
		if (!startupComplete)
		{
			startupComplete = true;

			if (canvasGroup == null)
				canvasGroup = GetComponent<CanvasGroup>();

			canvasGroup.alpha = 0.0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			gameObject.SetActive(false);
		}
	}

	/// <summary> Turns off the specified panel </summary>
	public void DisablePanel()
	{
		if (canvasGroup == null)
			canvasGroup = GetComponent<CanvasGroup>();

		canvasGroup.alpha = 0.0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;

		gameObject.SetActive(false);
	}
	/// <summary> Transitions the panel on </summary>
	public void PerformDefaultOnTransition()
	{
		Transition(onTransitionName);
	}

	/// <summary> Transitions the panel off </summary>
	public void PerformDefaultOffTransition()
	{
		Transition(offTransitionName);
	}

	#region Navigation history + back button functionality

	/// <summary> Navigates back one screen in the history </summary>
	public virtual void NavigateBack()
	{
		PanelManager.Instance.NavigateBack();
	}

	/// <summary> Navigates back in the history until it reaches the specified screen </summary>
	/// <param name="_panelID">Panel identifier.</param>
	public void ReturnToScreen(PanelID _panelID)
	{
		PanelManager.Instance.ReturnToScreen(_panelID);
	}

	#endregion	// Navigation history + back button functionality

	#if UNITY_EDITOR
	/// <summary> Called from "Create Panel" menu </summary>
	/// <param name="_anim"> Animator </param>
	public void SetAnimator(Animator _anim)
	{
		anim = _anim;
	}
	#endif
}
