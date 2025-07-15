using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class PopupMessageBase : MonoBehaviour
{
	public const PanelID PopupPanelId = PanelID.Popups;

	public PopupWindowContainer.PopupIDs currentPopUpId { get; private set; }

	[System.Serializable]
	public class PopupInfo
	{
		#region Inspector variables

		public PopupWindowContainer.PopupIDs	popupId;								// Which container to display
		public LocIDs							titleLabelsID			= LocIDs.None;	// Title text
		public GameObject						imagePrefab;							// Image to spawn next to text (optional)
		public LocIDs							messageLabelsID			= LocIDs.None;	// Main message text
		public string[] 						messageSubstrings;

		public LocIDs[]							confirmLabelsID			= { LocIDs.None };	// Label on Confirm button
		public string[]							confirmSubstrings;

		public LocIDs cancelLabelsID			= LocIDs.None;	// Label on Cancel button

		[Header("Overrides")]
		public LocIDs							messageOverrideAndroid	= LocIDs.None;	// Main message text override when on Android
		public UnityEvent						unityConfirm;
		public UnityEvent 						unityCancel;

		#endregion

		public Action[]							confirmCallback;						// Callback for when button is pressed
		public Action							cancelCallback;                         // Callback for when button is pressed
		public Action<PopupWindowContainer>		onPreDisplayCallback;                       // Called before popup is shown to the user
		public Action<PopupWindowContainer>		onPostDisplayCallback;                       // Called after popup is shown to the user

		public Sprite backgroundImageSprite;

		public bool enableCustomCancelButton;

		public bool isGenericPopup;

		public PopupInfo(LocIDs _titleCSVLineNo, GameObject _imagePrefab, LocIDs _messageCSVLineNo, LocIDs[] _confirmButtonCSVLineNo, 
			PopupWindowContainer.PopupIDs _popupId = PopupWindowContainer.PopupIDs.Default, string[] _messageSubstrings = null,
			Action[] _confirmCallBack = null, string[] _confirmSubstrings = null, Action _cancelCallback = null, 
			Action<PopupWindowContainer> _onPreDisplayCallback = null, Action<PopupWindowContainer> _onPostDisplayCallback = null,Sprite backgroundImageSprite = null,bool enableCustomCancelButton = false,bool isGenericPopup = false) 
		{
			titleLabelsID 		= _titleCSVLineNo;
			imagePrefab			= _imagePrefab;
			messageLabelsID		= _messageCSVLineNo;
			messageSubstrings	= _messageSubstrings;
			confirmLabelsID		= _confirmButtonCSVLineNo;
			confirmSubstrings	= _confirmSubstrings;
			popupId 			= _popupId;
			confirmCallback 	= _confirmCallBack;
			cancelCallback 		= _cancelCallback;
			onPreDisplayCallback = _onPreDisplayCallback;
			onPostDisplayCallback = _onPostDisplayCallback;
			this.backgroundImageSprite = backgroundImageSprite;
			this.enableCustomCancelButton = enableCustomCancelButton;
			this.isGenericPopup = isGenericPopup;
		}
	}

	#region Non-editor variables

	protected Queue<PopupInfo>		PopupQueue = new Queue<PopupInfo>();
	Action[]						ConfirmCallback;
	Action							CancelCallback;
	UnityEvent 						UnityConfirm;
	UnityEvent 						UnityCancel;
	GameObject						spawnedImage;

	#endregion	// Non-editor variables

	#region Implement these

	protected abstract void			Show();						// Implement this
	protected abstract void			Hide();						// Implement this
	protected abstract bool			IsShowing();				// Implement this

	#endregion	// Implement these

	void Start()
	{
		PanelManager.Instance.SetPanelDisableCallback(PopupPanelId, PanelTransitionCallback);
	}
	
	void PanelTransitionCallback()
	{
		TryShowNext();
	}
	
	/// <summary> Shows & returns the specified popup container </summary>
	/// <param name="_popupId"> Popup's name </param>
	/// <returns> The PopupWindowContainer </returns>
	protected virtual PopupWindowContainer ShowContainer(PopupWindowContainer.PopupIDs _popupId)
	{
		throw new NotImplementedException();
	}

	/// <summary> Pops up, showing the specified options/layout </summary>
	/// <param name="info"> Popup contents </param>
	/// <param name="_forceShow">Close any popups that are currently showing and clear queue to show this popup.</param>
	public void PopupOrQueue(PopupInfo _info, bool _forceShow = false)
	{
		PopupQueue.Enqueue(_info);

		if (!IsShowing())
			TryShowNext();
		else if (_forceShow)
		{
			Hide();
			if (PopupQueue.Count > 1)
				PopupQueue.Clear();
			this.NextFrame(() => TryShowNext());
		}
	}

	/// <summary> Pops up, showing the specified options/layout </summary>
	/// <param name="_popupId"> Popup container's ID </param>
	/// <param name="_dismissCallback"> Dismiss callback, else uses the default one </param>
	public void PopupOrQueue(PopupWindowContainer.PopupIDs _popupId)
	{
		PopupOrQueue(new PopupInfo(LocIDs.None, null, LocIDs.None, new LocIDs[] { LocIDs.None }, _popupId));
	}

	/// <summary>
	/// Show a new popup if one is queued and a popup is not already displayed.
	/// </summary>
	public void ShowNextIfReady()
	{
		if (!IsShowing())
			TryShowNext();
	}

	/// <summary> Shows the next popup, if there is one </summary>
	/// <returns> True if there was a popup to show in the queue </returns>
	protected bool TryShowNext()
	{
		if (PopupQueue.Count > 0) 
		{
			PopupInfo info = PopupQueue.Dequeue();
			currentPopUpId = info.popupId;
			PopupWindowContainer container = ShowContainer(info.popupId);

			if (container.imageContainer != null)
			{

				container.DisableImageConatiner();

				if (info.imagePrefab != null)
				{
					if (spawnedImage != null)
						Destroy(spawnedImage);

					spawnedImage = GameObject.Instantiate(info.imagePrefab, container.imageContainer);
                    if (info.isGenericPopup)
                    {
						GenericPopupItem genericPopupItem = spawnedImage.GetComponent<GenericPopupItem>();
						if(genericPopupItem != null)
                        {
							genericPopupItem.Initialize(container as GenericPopup);
						}
					}
					container.EnableImageContainer(info.titleLabelsID);
				}
			}
			
			//if ((container.titleText != null) && (info.titleLabelsID != LocIDs.None))
			//	container.titleText.SetCSVLineNo((int)info.titleLabelsID);
			//
			//if ((container.messageText != null) && (info.messageLabelsID != LocIDs.None))
			//{
			//	container.messageText.SetCSVLineNo((int)info.messageLabelsID);
			//	container.messageText.SetSubstituteStrings(info.messageSubstrings);
			//}			
			//
			//if(container.backgroundImage != null && info.backgroundImageSprite != null)
            //{
			//	container.backgroundImage.sprite = info.backgroundImageSprite;
			//}
			//
			//for (int i = 0; i < container.confirmButtonText.Length; ++i)
			//{
			//	if (container.confirmButtonText[i] != null)
			//	{
			//		if((info.confirmLabelsID.Length > i) && (info.confirmLabelsID[i] != LocIDs.None))
			//			container.confirmButtonText[i].SetCSVLineNo((int)info.confirmLabelsID[i]);
			//		container.confirmButtonText[i].SetSubstituteStrings(info.confirmSubstrings);
			//	}
			//} 
			//
			//if ((container.cancelButtonText != null) && (info.cancelLabelsID != LocIDs.None))
			//	container.cancelButtonText.SetCSVLineNo((int)info.cancelLabelsID);

			if (info.confirmCallback != null)
			{
				ConfirmCallback = new Action[info.confirmCallback.Length];
				for (int i = 0; i < info.confirmCallback.Length; ++i)
					ConfirmCallback[i] = info.confirmCallback[i];
			}
			else
				ConfirmCallback = null;

			CancelCallback = (info.cancelCallback == null) ? DefaultConfirmCallback : info.cancelCallback;
			UnityConfirm = info.unityConfirm;
			UnityCancel = info.unityCancel;

			if (container.cancelButton)
				container.cancelButton.SetActive((info.confirmCallback != null) || (info.unityConfirm.GetPersistentEventCount() != 0));

			container.ToggleCustomCancelButton(info.enableCustomCancelButton);

			// AudioController.Play("UI_Popup");

			if(info.onPreDisplayCallback != null)
            {
				info.onPreDisplayCallback.Invoke(container);

			}
			Show();
			if (info.onPostDisplayCallback != null)
			{
				info.onPostDisplayCallback.Invoke(container);
			}

			return true;
		}

		else
			return false;
	}
	public void SetCustomTitle( string _text, PopupWindowContainer.PopupIDs popupIDs)
	{
		PopupWindowContainer container = ShowContainer(popupIDs);
		//container.titleText.SetTextDirectly( _text);
	}

	/// <summary> Called when popup is finished </summary>
	public void PopupConfirmed()
	{
		PopupConfirmed(0);
	}

	/// <summary> Called when popup is finished </summary>
	public void PopupConfirmed(int index)
	{
		if (ConfirmCallback != null && index < ConfirmCallback.Length)
		{
			if(ConfirmCallback[index] != null)
				ConfirmCallback[index]();
			if (ConfirmCallback != null && index < ConfirmCallback.Length)
				ConfirmCallback[index] = null;
		}
		CancelCallback = null;

		if (UnityConfirm != null)
		{
			UnityConfirm.Invoke();
		}
		UnityConfirm = null;
		UnityCancel = null;

		if (!TryShowNext())
		{
			Hide();
		}
	}

	/// <summary> Dismisses the window without triggering the callback </summary>
	public void PopupCancelled()
	{
		if (CancelCallback != null)
		{
			CancelCallback();
			CancelCallback = null;
		}
		ConfirmCallback = null;

		if (UnityCancel != null)
		{
			UnityCancel.Invoke();
		}
		UnityConfirm = null;
		UnityCancel = null;

		if (!TryShowNext())
		{
			Hide();
		}
	}

	/// <summary> Default callback for just dismissing the screen </summary>
	public void DefaultConfirmCallback()
	{
		PanelManager.DisableScreen(PopupPanelId);
	}
}
