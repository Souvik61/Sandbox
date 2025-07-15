using UnityEngine;
using UnityEngine.UI;

public partial class PopupWindowContainer : MonoBehaviour
{
	[Header("Main")]
	public PopupIDs				popupId;					// Which popup this is
	public GameObject 			cancelButton;

	[Header("Title")]
	//public UILocLabelBase		titleText;					// Title 

	[Header("Message + (optional) Image")]
	//public UILocLabelBase messageText;				// Message
	public Transform			imageContainer;				// Container for spawning image next to text

	[Header("Buttons")]
	//public UILocLabelBase[]		confirmButtonText;			// Confirm button's label
	//public UILocLabelBase cancelButtonText;         // Cancel button's label

	public Image backgroundImage;  // background image of the panel

	public virtual void EnableImageContainer(LocIDs title)
	{
		imageContainer.gameObject.SetActive(true);
	}

	public virtual void DisableImageConatiner()
	{
		imageContainer.gameObject.SetActive(false);
	}

	public virtual void ToggleCustomCancelButton(bool isEnabled)
    {

    }
}
