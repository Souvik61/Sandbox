using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple object that can be yielded in a coroutine. Useful when taking an object that only has
/// event callbacks and converting it so we can yield wait for it to return.
/// See async result for a similar version that also returns a result object.
/// </summary>
public class CustomYieldable : CustomYieldInstruction
{
	public override bool keepWaiting => !complete;

	private bool complete;

	/// <summary>
	/// Complete this yield-able, continuing the coroutine when next checked.
	/// </summary>
	public void Complete() => complete = true;
}
