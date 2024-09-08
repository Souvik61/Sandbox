using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class AnimatorExtensions
{
	public static bool IsCurrentStateComplete(this Animator animator)
	{
		AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
		if (info.loop)
		{
			// animation can never be complete if it loops
			return false;
		}
		else
		{
			float normalizedTime = info.normalizedTime;
			return normalizedTime >= 1f;
		}
	}

	public static bool IsExitingOrExitedState(this Animator animator, string stateName, int layerIndex = 0)
	{
		bool result = !animator.IsInState(stateName, layerIndex, true, false);
		return result;
	}

	public static bool IsExitingOrExitedState(this Animator animator, int stateHash, int layerIndex = 0)
	{
		bool result = !animator.IsInState(stateHash, layerIndex, true, false);
		return result;
	}

	public static bool IsInTransitionFromState(this Animator animator, string stateName, int layerIndex = 0)
	{
		bool isCurrentState = animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
		return isCurrentState && animator.IsInTransition(layerIndex);
	}

	public static bool IsInTransitionFromState(this Animator animator, int stateHash, int layerIndex = 0)
	{
		bool isCurrentState = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == stateHash;
		return isCurrentState && animator.IsInTransition(layerIndex);
	}

	public static bool IsInTransitionToState(this Animator animator, string stateName, int layerIndex = 0)
	{
		bool isNextState = animator.GetNextAnimatorStateInfo(layerIndex).IsName(stateName);
		return isNextState && animator.IsInTransition(layerIndex);
	}

	public static bool IsInTransitionToState(this Animator animator, int stateHash, int layerIndex = 0)
	{
		bool isNextState = animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash == stateHash;
		return isNextState && animator.IsInTransition(layerIndex);
	}

	// NOTE(broscoe): This should replace the string version eventually.
	public static bool IsInState(this Animator animator, int stateHash, int layerIndex = 0, bool canTransitionTo = true, bool canTransitionFrom = true)
	{
		// Check to see if the object that its attached to is valid 
		if (!animator.gameObject.activeInHierarchy)
			return false;

		bool isCurrentState = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == stateHash;
		bool isInTransition = animator.IsInTransition(layerIndex);

		if (isCurrentState && !canTransitionFrom)
			return !isInTransition;

		bool isNextState = animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash == stateHash;

		if (isNextState && !canTransitionTo)
			return !isInTransition;

		return isCurrentState || isNextState;
	}

	public static bool IsInState(this Animator animator, string stateName, int layerIndex = 0, bool canTransitionTo = true, bool canTransitionFrom = true)
	{
		// Check to see if the object that its attached to is valid 
		if (!animator.gameObject.activeInHierarchy)
			return false;

		bool isCurrentState = animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
		bool isInTransition = animator.IsInTransition(layerIndex);

		if (isCurrentState && !canTransitionFrom)
			return !isInTransition;

		bool isNextState = animator.GetNextAnimatorStateInfo(layerIndex).IsName(stateName);

		if (isNextState && !canTransitionTo)
			return !isInTransition;

		return isCurrentState || isNextState;
	}

	public static bool IsInTaggedState(this Animator animator, string tag, int layerIndex = 0, bool canTransitionTo = true, bool canTransitionFrom = true)
	{
		// Check to see if the object that its attached to is valid 
		if (!animator.gameObject.activeInHierarchy)
			return false;

		bool isCurrentState = animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tag);
		bool isInTransition = animator.IsInTransition(layerIndex);

		if (isCurrentState && !canTransitionFrom)
			return !isInTransition;

		bool isNextState = animator.GetNextAnimatorStateInfo(layerIndex).IsTag(tag);

		if (isNextState && !canTransitionTo)
			return !isInTransition;

		return isCurrentState || isNextState;
	}

	public static bool IsStateAtEndOfMotion(this Animator animator, string stateName, int layerIndex = 0, bool canTransitionTo = true, bool canTransitionFrom = true)
	{
		bool isInState = animator.IsInState(stateName, layerIndex, canTransitionTo, canTransitionFrom);
		bool isAtEnd = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= 1.0f;
		bool result = isInState && isAtEnd;
		return result;
	}

	public static void SetTriggerWithUpdate(this Animator animator, string name)
	{
		animator.SetTrigger(name);
		animator.Update(0.0f);
	}

	public static void SetTriggerWithUpdate(this Animator animator, int hash)
	{
		animator.SetTrigger(hash);
		animator.Update(0.0f);
	}

	public static void ResetTriggerWithUpdate(this Animator animator, string name)
	{
		animator.ResetTrigger(name);
		animator.Update(0.0f);
	}

	public static void ResetTriggerWithUpdate(this Animator animator, int hash)
	{
		animator.ResetTrigger(hash);
		animator.Update(0.0f);
	}

	public static void SetBoolWithUpdate(this Animator animator, string name, bool value)
	{
		animator.SetBool(name, value);
		animator.Update(0.0f);
	}

	public static void SetBoolWithUpdate(this Animator animator, int hash, bool value)
	{
		animator.SetBool(hash, value);
		animator.Update(0.0f);
	}

	public static void SetIntegerWithUpdate(this Animator animator, string name, int value)
	{
		animator.SetInteger(name, value);
		animator.Update(0.0f);
	}

	public static void SetIntegerWithUpdate(this Animator animator, int hash, int value)
	{
		animator.SetInteger(hash, value);
		animator.Update(0.0f);
	}
}