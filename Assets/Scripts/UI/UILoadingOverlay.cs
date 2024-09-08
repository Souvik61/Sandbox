using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the loading overlay UI Panel.
/// </summary>
public class UILoadingOverlay : Panel
{
	private static Coroutine waitRoutine;

	/// <summary>
	/// Shows the loading overlay until the given yield-able completes.
	/// </summary>
	public static void ShowLoadingOverlay(CustomYieldInstruction _wait)
	{
		PanelManager.EnableScreen(PanelID.None);
		if (waitRoutine != null)
			CoroutineExtensions.StopGlobalCoroutine(waitRoutine);

		waitRoutine = CoroutineExtensions.StartGlobalCoroutine(WaitThenClose(_wait));
	}

	/// <summary>
	/// Shows the loading overlay until the given yield-able completes.
	/// </summary>
	public static void ShowInlineLoadingOverlay(CustomYieldInstruction _wait)
	{
		if (waitRoutine != null)
			CoroutineExtensions.StopGlobalCoroutine(waitRoutine);

		//Equestrian.Events.EventManager.FireEvent(PanelManager.Instance, Equestrian.Events.EventID.GAMEEVENT_INLINE_LOADER, new object[] { true });

		waitRoutine = CoroutineExtensions.StartGlobalCoroutine(WaitThenHideInlineLoader(_wait));
	}

	/// <summary>
	/// Shows the loading overlay until the given yield-able completes plus the additional wait completes.
	/// </summary>
	public static void ShowLoadingOverlay(CustomYieldInstruction _wait, float _additionalWait)
	{
		PanelManager.EnableScreen(PanelID.None);
		if (waitRoutine != null)
			CoroutineExtensions.StopGlobalCoroutine(waitRoutine);

		waitRoutine = CoroutineExtensions.StartGlobalCoroutine(WaitThenClose(_wait, _additionalWait));
	}


	/// <summary>
	/// Waits for the given yield-able to complete, then closes the panel.
	/// </summary>
	private static IEnumerator WaitThenClose(CustomYieldInstruction _wait)
	{
		yield return _wait;
		PanelManager.DisableScreen(PanelID.None);
		waitRoutine = null;
	}

	/// <summary>
	/// Waits for the given yield-able to complete, then waits an additional x seconds then closes the panel.
	/// </summary>
	private static IEnumerator WaitThenClose(CustomYieldInstruction _wait, float _additionalWait)
	{
		yield return _wait;
		yield return new WaitForSeconds(_additionalWait);
		PanelManager.DisableScreen(PanelID.None);
		waitRoutine = null;
	}


	/// <summary>
	/// Waits for the given yield-able to complete, then closes the panel.
	/// </summary>
	private static IEnumerator WaitThenHideInlineLoader(CustomYieldInstruction _wait)
	{
		yield return _wait;
		//Equestrian.Events.EventManager.FireEvent(PanelManager.Instance, Equestrian.Events.EventID.GAMEEVENT_INLINE_LOADER, new object[] { false });
		waitRoutine = null;
	}
}
