using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNL_Test : Panel
{

	public override void Init()
	{
		base.Init();

		this.onTransitionOn.AddListener(OnTransitionOnMethod);

	}

	private void OnTransitionOnMethod()
	{
		//EventManager.FireEvent(this, EventID.SCREENS_ACTIVE_EVENTS);
	}

	// Button Presses
	public void OnButtonPress_Test()
	{
		Debug.Log("Test btn press");
	}

	

	protected override void OnDisable()
	{
		base.OnDisable();

	}

	private void Update()
	{
		
	}

	protected override void OnEnable()
	{
		base.OnEnable();

	}




}
