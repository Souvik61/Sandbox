using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindowNotifierWithPopulate : MonoBehaviour
{
	[SerializeField] public PopupMessageBase.PopupInfo popupInfo;

	public void ShowPopup()
	{
		if (Application.platform == RuntimePlatform.Android && (popupInfo.messageOverrideAndroid != LocIDs.None))
			popupInfo.messageLabelsID = popupInfo.messageOverrideAndroid;

		PanelManager.Instance.popupController.PopupOrQueue(popupInfo);
	}
}
