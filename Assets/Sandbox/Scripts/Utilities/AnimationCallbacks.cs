using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides triggered callbacks when animators hit a certain state.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationCallbacks : MonoBehaviour
{
	public event System.Action AnimatorEnteredTestState;

	[SerializeField]
	private string testState;

	private int state_hash;
	private Animator animator;
	private bool callbackFired;

	public void ClearCallbacks()
	{
		AnimatorEnteredTestState = null;
	}

	private void Start()
	{
		animator = GetComponent<Animator>();
		state_hash = Animator.StringToHash(testState);
	}

	private void OnEnable()
	{
		callbackFired = false;
	}

	private void Update()
	{
		if (!callbackFired && !animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == state_hash)
		{
			callbackFired = true;
			AnimatorEnteredTestState?.Invoke();
		}
	}
}
