using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes multiple yield instructions and waits for all of them to complete before continuing.
/// </summary>
public class CompositeYieldInstruction : CustomYieldInstruction
{
	private readonly CustomYieldInstruction[] yield;

	public CompositeYieldInstruction(params CustomYieldInstruction[] yield)
	{
		this.yield = yield;
	}

	public override bool keepWaiting
	{
		get
		{
			foreach (CustomYieldInstruction y in yield)
			{
				if (y != null && y.keepWaiting)
					return true;
			}

			return false;
		}
	}
}
