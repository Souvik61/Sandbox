using UnityEngine;
using System.Collections;

public class PanelSwitchNotifier : MonoBehaviour
{
	[Header("DEPRECATED - Use PanelNotifier instead.")]
	[SerializeField] PanelID	PanelToSwitchTo;

	void Awake()
	{
		Debug.LogWarning("DEPRECATED - Use PanelNotifier instead.", this);
	}

	public void Switch()
	{
		Debug.LogWarning("DEPRECATED - Use PanelNotifier instead.", this);
	}

	public void BackButtonPressed()
	{
		Debug.LogWarning("DEPRECATED - Use PanelNotifier instead.", this);
	}

	public void BackButtonPressed(GameObject _panelToDismiss)
	{
		Debug.LogWarning("DEPRECATED - Use PanelNotifier instead.", this);
	}


}
