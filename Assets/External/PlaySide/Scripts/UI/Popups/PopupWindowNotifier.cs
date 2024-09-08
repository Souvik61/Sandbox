using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindowNotifier : MonoBehaviour
{
	public PopupWindowContainer.PopupIDs popupID;

	public void ShowPopup()
	{
		PanelManager.Instance.popupController.PopupOrQueue(popupID);
	}	
}
