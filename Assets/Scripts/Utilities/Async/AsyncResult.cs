using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Generic container for async call. Can yield in coroutine, await Task or subscribe to Completed event.
/// </summary>
public class AsyncResult<ResultType> : CustomYieldInstruction
{
	/// <summary>
	/// Returns a Async result that is already complete using the given value
	/// (Useful if a method can optionally return a running or already complete Async result).
	/// </summary>
	public static AsyncResult<ResultType> AlreadyCompleted(ResultType _result)
	{
		var result = new AsyncResult<ResultType>();
		result.Complete(_result);
		return result;
	}

	/// <summary>
	/// Event fired when the async call is completed.
	/// </summary>
	public event System.Action<ResultType> Completed;

	/// <summary>
	/// The result of the async call, available after completed.
	/// </summary>
	public ResultType Result { get; private set; }

	/// <summary>
	/// The error of the async call, available after completed if error was passed.
	/// </summary>
	public System.Exception Exception { get; private set; }

	/// <summary>
	/// Whether this async call has completed.
	/// </summary>
	public bool IsCompleted { get; private set; }

	/// <summary>
	/// Whether this async result has been canceled.
	/// </summary>
	public bool Cancelled { get; private set; }

	/// <summary>
	/// An awaitable task for this async result.
	/// </summary>
	public Task<ResultType> Task
	{
		get
		{
			var completionSource = new TaskCompletionSource<ResultType>();
			Completed += result => completionSource.TrySetResult(result);
			return completionSource.Task;
		}
	}

	public override bool keepWaiting => !IsCompleted;

	/// <summary>
	/// Event fired when an error is passed to the async callback.
	/// </summary>
	private event System.Action<System.Exception> error;

	/// <summary>
	/// Completes the async call with the given result.
	/// </summary>
	public virtual void Complete(ResultType _result)
	{
		Result = _result;
		Completed?.Invoke(_result);
		IsCompleted = true;
	}

	/// <summary>
	/// Completes the async call with the given error.
	/// </summary>
	public void Error(System.Exception _exception)
	{
		Exception = _exception;
		error?.Invoke(_exception);
		IsCompleted = true;
	}

	/// <summary>
	/// Marks the callback as canceled, removing any listeners to Completed event.
	/// </summary>
	public void Cancel()
	{
		Cancelled = true;
		Completed = null;
	}

	/// <summary>
	/// Calls the given callback when the async result is complete. This may be called immediately if the 
	/// callback has already previously completed.
	/// </summary>
	public AsyncResult<ResultType> OnComplete(System.Action<ResultType> _callback)
	{
		if (_callback != null)
		{
			if (IsCompleted)
				_callback(Result);
			else
			{
				void OnCompleteCallback(ResultType _result)
				{
					Completed -= OnCompleteCallback;
					_callback(_result);
				}

				Completed += OnCompleteCallback;
			}
		}

		return this;
	}

	/// <summary>
	/// Calls the given callback when an exception is passed to the async callback.
	/// </summary>
	public AsyncResult<ResultType> OnError(System.Action<System.Exception> _callback)
	{
		if (_callback != null)
		{
			if (IsCompleted && Exception != null)
				_callback(Exception);
			else
			{
				void OnErrorCallback(System.Exception _exception)
				{
					error -= OnErrorCallback;
					_callback(_exception);
				}

				error += OnErrorCallback;
			}
		}

		return this;
	}
}
