using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PanelNotifier : MonoBehaviour 
{
	[Header("Switch Functionality")]
	[SerializeField] PanelID	panelToSwitchTo;

	[Header("Transition Functionality")]
	[SerializeField] public PanelID[]	panelsToEnable;
	[SerializeField] public PanelID[]	panelsToDisable;

	Panel						parentPanel;


	// Transitions all panels
	public void TransitionAll ()
	{
		for (int i = 0; i < panelsToEnable.Length; ++i)
		{
			if (PanelManager.ScreenExists(panelsToEnable[i]))
				PanelManager.EnableScreen(panelsToEnable[i]);
		}

		for (int i = 0; i < panelsToDisable.Length; ++i)
		{
			if (PanelManager.ScreenExists(panelsToDisable[i]))
				PanelManager.DisableScreen(panelsToDisable[i]);
		}
	}


	// Disable current Panel and enable target Panel
	public void Switch()
	{
		if (parentPanel == null)
		{
			parentPanel = GetComponentInParent<Panel>();
			if (parentPanel == null)
				throw new UnityException("Can't find Panel component anywhere above '" + gameObject.name + "' in the hierarchy");
		}

		PanelManager.Instance.SwitchToScreen(parentPanel.id, panelToSwitchTo);
	}
}
