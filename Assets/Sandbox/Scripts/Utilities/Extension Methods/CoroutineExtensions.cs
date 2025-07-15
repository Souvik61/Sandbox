using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains simple extension methods for simple coroutine patterns.
/// </summary>
public static class CoroutineExtensions
{
	/// <summary>
	/// Persistant monobehaviour that runs all global coroutines.
	/// </summary>
	private static MonoBehaviour globalCoroutineBehaviour;

	/// <summary>
	/// Coroutine to wait for the specified amount of time, then call the given callback.
	/// </summary>
	public static IEnumerator DelayedCallbackRoutine(float _time, System.Action _callback)
	{
		yield return new WaitForSeconds(_time);
		_callback?.Invoke();
	}

	/// <summary>
	/// Coroutine to wait for next frame then call the given callback.
	/// </summary>
	public static IEnumerator NextFrameRoutine(System.Action _callback)
	{
		yield return null;
		_callback?.Invoke();
	}

	/// <summary>
	/// Coroutine that calls a given callback every frame for the given time, passing a normalized time value to the callback.
	/// May also have optional callback to call once transition is complete.
	/// </summary>
	public static IEnumerator TransitionCallbackRoutine(float _time, System.Action<float> _callback, System.Action _onComplete)
	{
		float currentTime = 0;
		while (true)
		{
			currentTime += Time.deltaTime;
			if (currentTime < _time)
			{
				_callback?.Invoke(Mathf.InverseLerp(0, _time, currentTime));
				yield return null;
			}
			else break;
		}

		_callback?.Invoke(1);
		_onComplete?.Invoke();
	}

	/// <summary>
	/// Coroutine that continually calls the given callback each frame, transitioning from 0 - 1, endlessly until the coroutine is manually stopped.
	/// </summary>
	public static IEnumerator TransitionLoopCallbackRoutine(float _time, System.Action<float> _callback)
	{
		while (true)
		{
			yield return TransitionCallbackRoutine(_time, _callback, null);
		}
	}


	/// <summary>
	/// Coroutine that calls the given condition callback each frame until it returns true, then calls the complete callback.
	/// </summary>
	public static IEnumerator ConditionCallbackRoutine(System.Func<bool> _conditionCallback, System.Action _completeCallback)
	{
		while (!_conditionCallback())
		{
			yield return null;
		}

		_completeCallback?.Invoke();
	}

	/// <summary>
	/// Coroutine that waits for all the given custom yield-ables to complete before calling the complete callback.
	/// </summary>
	public static IEnumerator WaitAllRoutine(System.Action _callback, IEnumerable<CustomYieldInstruction> _yields)
	{
		bool allComplete;

		do
		{
			yield return null;
			allComplete = true;
			foreach (CustomYieldInstruction yield in _yields)
			{
				if (yield != null && yield.keepWaiting)
				{
					allComplete = false;
					break;
				}
			}
		} while (!allComplete);

		_callback?.Invoke();
	}

	/// <summary>
	/// Waits for the end of the current frame then calls the given callback.
	/// </summary>
	public static IEnumerator WaitEndFrameRoutine(System.Action _callback)
	{
		yield return new WaitForEndOfFrame();
		_callback.Invoke();
	}

	/// <summary>
	/// Starts a global coroutine (allows starting a coroutine not tied to a specific monobehaviour).
	/// </summary>
	public static Coroutine StartGlobalCoroutine(IEnumerator _coroutine)
	{
		if (globalCoroutineBehaviour == null)
		{
			globalCoroutineBehaviour = new GameObject("[Global Coroutines]").AddComponent<GlobalCoroutinesBehaviour>();
			Object.DontDestroyOnLoad(globalCoroutineBehaviour.gameObject);
		}

		return globalCoroutineBehaviour.StartCoroutine(_coroutine);
	}

	/// <summary>
	/// Stops a global coroutine.
	/// </summary>
	public static void StopGlobalCoroutine(Coroutine _coroutine)
	{
		if (globalCoroutineBehaviour != null)
			globalCoroutineBehaviour.StopCoroutine(_coroutine);
	}

	/// <summary>
	/// Stops all global coroutines.
	/// </summary>
	public static void StopAllGlobalCoroutines()
	{
		if (globalCoroutineBehaviour != null)
			globalCoroutineBehaviour.StopAllCoroutines();
	}

	/// <summary>
	/// Calls the given callback after the specified time.
	/// </summary>
	public static Coroutine DelayedCallback(this MonoBehaviour _monoBehaviour, float _time, System.Action _callback)
	{
		return _monoBehaviour.StartCoroutine(DelayedCallbackRoutine(_time, _callback));
	}

	/// <summary>
	/// Calls the given callback on the next frame.
	/// </summary>
	public static Coroutine NextFrame(this MonoBehaviour _monoBehaviour, System.Action _callback)
	{
		return _monoBehaviour.StartCoroutine(NextFrameRoutine(_callback));
	}

	/// <summary>
	/// Calls a given callback every frame for the given time, passing a normalized time value to the callback.
	/// May also have optional callback to call once transition is complete.
	/// </summary>
	public static Coroutine TransitionCallback(this MonoBehaviour _monoBehaviour, float _time,
		System.Action<float> _callback, System.Action _onComplete = null)
	{
		if (_monoBehaviour != null && _monoBehaviour.gameObject != null && (!_monoBehaviour.gameObject.activeSelf || !_monoBehaviour.gameObject.activeInHierarchy))
		{
			_monoBehaviour.gameObject.SetActive(true);
		}
		return _monoBehaviour.StartCoroutine(TransitionCallbackRoutine(_time, _callback, _onComplete));
	}

	/// <summary>
	/// Continually calls the given callback each frame, transitioning from 0 - 1, endlessly until the coroutine is manually stopped.
	/// </summary>
	public static Coroutine TransitionLoopCallback(this MonoBehaviour _monoBehaviour, float _time, System.Action<float> _callback)
	{
		return _monoBehaviour.StartCoroutine(TransitionLoopCallbackRoutine(_time, _callback));
	}

	/// <summary>
	/// Calls the given condition callback each frame until it returns true, then calls the complete callback.
	/// </summary>
	public static Coroutine ConditionCallback(this MonoBehaviour _monoBehaviour, System.Func<bool> _conditionCallback,
		System.Action _completeCallback)
	{
		if (_conditionCallback == null)
			throw new System.ArgumentException("Condition callback cannot be null", "_conditionCallback");

		return _monoBehaviour.StartCoroutine(ConditionCallbackRoutine(_conditionCallback, _completeCallback));
	}

	/// <summary>
	/// Waits for all the given custom yield-ables to complete before calling the complete callback.
	/// </summary>
	public static Coroutine WaitAll(this MonoBehaviour _monoBehaviour, System.Action _callback, params CustomYieldInstruction[] _yields)
	{
		return _monoBehaviour.StartCoroutine(WaitAllRoutine(_callback, _yields));
	}
}