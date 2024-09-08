using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor;
#endif

public class PanelManager : Singleton<PanelManager>, IManager
{
	#region Editor variables

	// If you receieve error: "The type or namespace name `PanelID' could not be found."
	// Then: Import DefaultAutogenScripts.package, located in External/PlaySide/ dir
	public PanelID[] initialPanels;
	public PanelID FallbackReturnToPanel;

	[SerializeField] private RectTransform mainCanvasRectTransform = null;

	[Header("Popups")]
	public PopupMessageBase popupController;

	[Header("RarityColours")]
	/// <summary>
	/// Global rarity colours
	/// </summary>
	[SerializeField]
	private Color commonColour;
	[SerializeField]
	private Color rareColour;
	[SerializeField]
	private Color epicColour;

	/// <summary>
	/// Returns the correct colour for the rarity
	/// </summary>
	/// <param name="rarity"></param>
	/// <returns></returns>

	#endregion   // Editor variables

	#region Non-editor variables

	public readonly Dictionary<PanelID, Panel> panels = new Dictionary<PanelID, Panel>();
	Stack<PanelID> navHistory = new Stack<PanelID>();

	[SerializeField] private Camera uiCamera;

	public Camera UICamera => uiCamera;

	public float cameraWidth { get; private set; }

	public float leftBound { get; private set; }

	public float rightBound { get; private set; }

	public int screenWidth { get; private set; }
	public static PanelID CurrentActivePanelID = PanelID.None;
	#endregion // Non-editor variables
	#region Debug
#if !FINAL || FORCE_DEBUG
	public bool OnScreenNavigationDebugging;
	StringBuilder OnGUISB = new StringBuilder();
	private GUIStyle guiStyle;
	Rect OnGUIRect;
	void OnGUI()
	{
		if (!OnScreenNavigationDebugging)
			return;
		guiStyle = new GUIStyle("Label");
		guiStyle.fontSize = 20;

		PanelID[] history = navHistory.ToArray();
		OnGUISB.Length = 0;
		for (int i = history.Length - 1; i >= 0; --i)
		{
			if (i < history.Length - 1)
				OnGUISB.Append(" -> ");
			OnGUISB.Append(history[i].ToString());
			OnGUISB.Append("\n");
		}

		OnGUIRect = GUILayoutUtility.GetRect(new GUIContent(OnGUISB.ToString()), guiStyle);
		OnGUIRect.x = Screen.width * 0.01f;
		OnGUIRect.y = Screen.height * 0.5f;

		GUI.Label(OnGUIRect, OnGUISB.ToString(), guiStyle);
	}
#endif
	#endregion // Debug

#if UNITY_EDITOR
	[ExecuteInEditMode]
    protected override void Awake()
    {
        if (Application.isPlaying)
        {
            base.Awake();
			float height = 2f * uiCamera.orthographicSize;
			cameraWidth = height * uiCamera.aspect;
			rightBound = cameraWidth / 2;
			leftBound = -rightBound;
			screenWidth = Screen.width;
		}
        else
        {
            PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        }
    }

    private void OnPrefabStageClosing(PrefabStage obj)
    {
        Panel[] unsortedPanels = GetComponentsInChildren<Panel>(true);
        foreach (var item in unsortedPanels)
            item.gameObject.SetActive(false);
    }
#endif

	// Index panels by enum int for later access
	public IEnumerator Init()
	{
		//UIFadeScreen.Instance.Hide();
		Debug.Log(transform.name);
		Panel[] unsortedPanels = GetComponentsInChildren<Panel>(true);

		for(int i=0; i<unsortedPanels.Length; i++)
		{
			PanelID id = unsortedPanels[i].id;
			if (panels.ContainsKey(id))
				Debug.LogError("Unable to initialize panels. Multiple panels share same ID: " + panels[id].name + ", " + unsortedPanels[i].name + 
					$". Shared id: {unsortedPanels[i].id}, original gameObject: {panels[id].transform.GetTransformPath()}, duplicate gameObject: {unsortedPanels[i].transform.GetTransformPath()}");
			panels[id] = unsortedPanels[i];
		}

		foreach (var panel in panels)
			panel.Value.Init();

		InitScreens(); 

		yield break;
	}

	public IEnumerator OnLogin() { yield break; }

	/// <summary>
	/// Converts a screen position to a value that works with different UI scale
	/// </summary>
	/// <param name="screenPoint">A screen point from any of the Camera.xxxToScreenPoint or RectTransformUtility.xxxToScreenPoint</param>
	/// <returns></returns>
	public static Vector2 TransformScreenPositionToCanvasPosition(Vector2 screenPoint)
	{
        if (InstanceValid)
        {
		    float ratioX = Instance.mainCanvasRectTransform.rect.width / Screen.width;
		    float ratioY = Instance.mainCanvasRectTransform.rect.height / Screen.height;

		    screenPoint.x *= ratioX;
		    screenPoint.y *= ratioY;

		    return screenPoint;
        }
        else
        {
            Debug.LogError("Panel Manager Instance Not Valid");
            return Vector2.zero;
        }
    }

    void InitScreens()
	{
		foreach (var panelKeyValue in panels)
		{
			Panel panel = panelKeyValue.Value;
			if (panel != null)
			{
				// Check if initial panel to turn on
				for (int initialPanelNo = 0; initialPanelNo < initialPanels.Length; ++initialPanelNo)
				{
					if (panel.id == initialPanels[initialPanelNo])
					{
						panel.gameObject.SetActive(true);
						panel.Transition(Panel.onTransitionName);
						break;
					}
				}
			}
		}
	}

	// Play a Transition (In/Out/State) on a Panel
	public void Transition(PanelID panelId, string transitionName)
	{
		if(!panels.ContainsKey(panelId))
			Debug.LogWarning("Unable to transition Panel "+panelId);

		panels[panelId].Transition(transitionName);
	}

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            var activePanels = GetComponentsInChildren<Panel>(false);
            foreach (var item in activePanels)
            {
                Debug.LogError($"YOU NEED TO TURN THIS OFF: {item.gameObject.name}");
                // item.gameObject.SetActive(false);
            }
        }
    }
#endif

    public void Transition(PanelID panelId)
	{
		panels[panelId].Transition(Panel.onTransitionName);
	}

	public void SwitchToScreen(PanelID fromScreen, PanelID toScreen)
	{
		DisableScreen(fromScreen);
		EnableScreen(toScreen);
	}

	// Instantly Enable a Panel
	public static void EnableScreen(PanelID panelId)
	{
		if (InstanceValid && Instance.panels.ContainsKey(panelId)) 
		{
			Panel panel = Instance.panels[panelId];
			if (panel != null)
			{
				panel.Transition(Panel.onTransitionName);
				CurrentActivePanelID = panelId;
				//EventManager.FireEvent(Instance,EventID.UI_PANEL_CHANGE);
			}
			else
				Debug.LogError($"Could not find panel: {panelId} to turn on ");
		}
	}

	public static bool ScreenExists(PanelID panelId)
	{
		return Instance.panels.ContainsKey(panelId);
	}

	public static Coroutine EnableScreen_Coroutine(PanelID _PanelID)
	{
		return InstanceValid ? Instance.StartCoroutine(Instance.EnableScreen_IEnumerator(_PanelID)) : null;
	}

	/// <summary>
	/// Shows a generic error popup with the given message.
	/// </summary>
	/// <returns>The async result returns when the popup is closed (currently always returns true).</returns>
	public static AsyncResult<bool> ShowErrorMessage(LocIDs _message, string[] subStrings = null, bool forceShowPopup = false)
	{
		var callback = new AsyncResult<bool>();
		var popupInfo = new PopupMessageBase.PopupInfo(LocIDs.Screens_Error, null, _message, new LocIDs[] { LocIDs.Confirm_Ok },
			PopupWindowContainer.PopupIDs.Default, subStrings, new System.Action[] { () =>
				{
					callback.Complete(true);
				}
			});
		Instance.popupController.PopupOrQueue(popupInfo, forceShowPopup);
		return callback;
	}

	private IEnumerator EnableScreen_IEnumerator(PanelID _PanelID)
	{
		EnableScreen(_PanelID);
		while (!IsScreenInState(_PanelID, "OnIdle"))
			yield return null;
	}

	public static Coroutine DisableScreen_Coroutine(PanelID _PanelID)
	{
		return InstanceValid ? Instance.StartCoroutine(Instance.DisableScreen_IEnumerator(_PanelID)) : null;
	}
	private IEnumerator DisableScreen_IEnumerator(PanelID _PanelID)
	{
		DisableScreen(_PanelID);
		while (IsScreenEnabled(PanelID.None))
			yield return null;
	}

	public void EnableScreenInstant(PanelID panelId)
	{
        if (panels.ContainsKey(panelId))
            panels[panelId].Transition(Panel.onIdleTransitionName);
        else
            Debug.LogError("There was a problem enabling instantly a screen");
	}

	public void DisableScreenInstant(PanelID panelId)
	{
		panels[panelId].Transition(Panel.offIdleTransitionName);
	}

	// Instantly Disable a Panel
	public static void DisableScreen(PanelID panelId)
	{
		if (InstanceValid && panelId != PanelID.None && Instance.panels.ContainsKey(panelId))
		{
			if (!Instance.panels.ContainsKey(panelId) || Instance.panels[panelId] == null)
				throw new UnityException ("Panel '" + panelId + "' not found in the hierarchy");

			Panel panel = Instance.panels[panelId];
			if (panel.gameObject.activeInHierarchy)
				panel.Transition(Panel.offTransitionName);
		}
	}

	public static List<PanelID> ActivePanels()
    {
		List<PanelID> activePanels = new List<PanelID>();
		if (InstanceValid)
        {
			foreach(PanelID panelID in Instance.panels.Keys)
            {
                if (Instance.panels[panelID].isActiveAndEnabled)
                {
					activePanels.Add(panelID);
				}
            }
        }
		return activePanels;

	}

	public void TurnOffScreenImmediately(PanelID panelId)
	{
		panels[panelId].gameObject.SetActive(false);
	}

	// Checks if a panel is enabled
	public static bool IsScreenEnabled(PanelID _PanelId)
	{
		return InstanceValid && Instance.panels.ContainsKey(_PanelId) ? (Instance.panels[_PanelId].gameObject.activeSelf) : false;
	}

	private bool IsScreenInState(PanelID _PanelID, string _StateName)
	{
		return panels[_PanelID].anim.IsInState(_StateName);
	}

	// Instantly enable the specified panel, and disable all others
	public void EnableOnlyScreen(PanelID panelToLeaveOn)
	{
		List<PanelID> panelList = new List<PanelID>();
		panelList.Add(panelToLeaveOn);
		EnableOnlyScreens(panelList);
	}

	public void EnableOnlyScreens(List<PanelID> panelIDs)
	{
		foreach (var panel in panels)
		{
			DisableScreen(panel.Key);
		}

		for (int i = 0; i < panelIDs.Count; ++i)
			EnableScreen(panelIDs[i]);
	}

	public void DisableAllScreens(bool turnOffImmediately, params PanelID[] Exeptions)
	{
		foreach (var panel in panels)
		{
            if (!Exeptions.Contains<PanelID>(panel.Key))
            {
			    if (panel.Value.gameObject.activeSelf)
			    {
				    if (turnOffImmediately)
					    panel.Value.gameObject.SetActive(false);
				    else
					    panel.Value.Transition(Panel.offTransitionName);
			    }
            }
		}
	}

    public PanelID GetLatestInHistory()
    {
        return navHistory.First();
    }

    public void SetPanelDisableCallback(PanelID panelId, Action callback)
	{
        if (panels.ContainsKey(panelId))
        {
		    if (panels != null && panels[panelId].disableCallback != null)
			    panels[panelId].disableCallback = callback;
        }
        else
        {
            Debug.Log("Panels does not contain PanelID: " + panelId);
        }
	}

	public void CallOnNextFrame(Action callback)
	{
		StartCoroutine(Wait1FrameThenCall(callback));
	}
	IEnumerator Wait1FrameThenCall(Action callback)
	{
		yield return new WaitForEndOfFrame();

		callback();
	}

	#region Navigation history + back button functionality

	public void AddScreenToNavHistory(PanelID panelID)
	{
		if (panelID == PanelID.None)
			return;
		if ((navHistory.Count == 0) || (navHistory.Peek() != panelID))
		{
			navHistory.Push(panelID);
		}
	}

    public void ClearNavHistory()
    {
        navHistory = new Stack<PanelID>();
    }

	public void PopReturnScreenFromNavHistory()
	{
		if (navHistory.Count > 1)
		{
			var current = navHistory.Pop();
			navHistory.Pop();
			navHistory.Push(current);
		}
	}

	public void NavigateBack()
	{
		if (navHistory.Count < 2)
		{
			Debug.LogError($"[{nameof(PanelManager)}] {nameof(NavigateBack)} was called while {nameof(navHistory)} has a count of only {navHistory.Count} which is less than the required minimum of 2. How?\n{System.Environment.StackTrace}");
			return;
		}

		PanelID returningFrom = navHistory.Pop();
		PanelID returningTo = navHistory.Pop();		
		SwitchToScreen(returningFrom, returningTo);
	}

	public PanelID PeekLastScreen()
	{
		return navHistory.Peek();
	}

	public PanelID PeekReturnToScreen()
    {
		PanelID returningFrom = navHistory.Pop();

		PanelID returningTo = navHistory.Peek();

		navHistory.Push(returningFrom);

		return returningTo;
	}

	public void RemoveScreenFromNavHistory(PanelID panelId)
    {
		List<PanelID> popedPanelIds = new List<PanelID>();
		while (navHistory.Count > 0 && navHistory.Peek() != panelId)
        {
			PanelID poppedPanel = navHistory.Pop();
			popedPanelIds.Add(poppedPanel);
		}

		if (navHistory.Count > 0)
		{
			navHistory.Pop();
        }

        for (int i = popedPanelIds.Count - 1; i >= 0; i--)
        {
			navHistory.Push(popedPanelIds[i]);
		}
    }

	public PanelID PeekReturnToScreen(int _depth)
	{
		Stack<PanelID> returningFrom = new Stack<PanelID>();
		for (int i = 0; i < _depth && navHistory.Count > 1; ++i)
			returningFrom.Push(navHistory.Pop());

		PanelID returningTo = navHistory.Peek();

		while (returningFrom.Count > 0)
			navHistory.Push(returningFrom.Pop());

		return returningTo;
	}

	/// <summary> Special case functionality that goes back in history as necessary </summary>
	/// <param name="panelID"> Screen to return to </param>
	public void ReturnToScreen(PanelID panelID)
	{
		PanelID panelToShow = panelID;

		if (!navHistory.Contains (panelToShow))
			panelToShow = FallbackReturnToPanel;

		while (navHistory.Count > 0 && navHistory.Peek () != panelToShow) 
		{
			PanelID panel = navHistory.Pop ();
			if (IsScreenEnabled(panel))
				DisableScreen(panel);
		}

		EnableScreen(panelToShow);
	}

	public T GetScreen<T>(PanelID panelToReturn) where T : Panel
	{
		if (!ScreenExists(panelToReturn))
        {
			Debug.LogError($"Could not find panel with id: {panelToReturn}");
            return null;
        }

		Panel panel = panels[panelToReturn];
		if (panel is T panelTyped)
			return panelTyped;
		else
		{
			Debug.LogError($"Panel with id {panelToReturn} is not of type {nameof(T)} but is of type {panel.GetType().Name}");
			return null;
		}
	}


	//public static void SetItemDisplaySprite(UI_ItemDisplay display, Sprite sprite)
    //{
	//	if (display != null)
	//	{
	//		display.SetSprite(sprite);
	//	}
	//	else
	//	{
	//		Debug.LogError("There was no item Display to set the sprite, please assign one");
	//	}
	//}

	//public static void SetItemDisplayTexture(UI_ItemDisplay display, Texture2D texture)
	//{
	//	if (display != null)
	//	{
	//		display.SetRawTexture(texture);
	//	}
	//	else
	//	{
	//		Debug.LogError("There was no item Display to set the texture, please assign one");
	//	}
	//}
	#endregion // Navigation history + back button functionality
}