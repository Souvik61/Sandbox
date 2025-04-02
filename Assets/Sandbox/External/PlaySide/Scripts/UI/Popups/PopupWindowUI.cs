using UnityEngine;
using System.Collections;
using System.Linq;

public class PopupWindowUI : PopupMessageBase
{
	PopupWindowContainer[] popups;

	#region PopupMessageBase Overrides

	/// <summary> Shows & returns the specified popup container </summary>
	/// <param name="_popupId"> Popup's name </param>
	/// <returns> The PopupWindowContainer </returns>
	protected override PopupWindowContainer ShowContainer(PopupWindowContainer.PopupIDs _popupId)
	{
		PopupWindowContainer container = null;

		if (popups == null)
			popups = GetComponentsInChildren<PopupWindowContainer>(true);

		for (int i = 0; i < popups.Length; ++i)
		{
			PopupWindowContainer popup = popups[i];
			if (popup.popupId == _popupId)
			{
				popup.gameObject.SetActive(true);
				container = popup;
			}
			else
				popup.gameObject.SetActive(false);
		}

		return container;
	}

    /// <summary> Shows the Popup </summary>
    protected override void Show()
	{
		PanelManager.EnableScreen(PopupPanelId);
		// AudioController.Play("UI_Popup");
	}

	/// <summary> Hides the Popup </summary>
	protected override void Hide()
	{
		PanelManager.DisableScreen(PopupPanelId);
		// AudioController.Play("UI_Back");
	}

	/// <summary> Is the popup currently visible? </summary>
	/// <returns> True if showing, false if hidden </returns>
	protected override bool IsShowing()
	{
		return PanelManager.IsScreenEnabled(PopupPanelId);
	}

	#endregion	// PopupMessageBase Overrides

	public void TooglePopup(PopupWindowContainer.PopupIDs popupIDs,bool val)
    {
		PopupWindowContainer  popupWindowContainer = popups.FirstOrDefault(x => x.popupId == popupIDs);
		popupWindowContainer.gameObject.SetActive(val);

	}
}
