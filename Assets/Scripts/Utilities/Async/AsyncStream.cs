using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Similar to an async callback, but allows for a stream of data to be returned over time.
/// </summary>
public class AsyncStream<ResultType>
{
	/// <summary>
	/// Fired every time a result is pushed to the stream.
	/// </summary>
	public event System.Action<ResultType> OnResult;

	/// <summary>
	/// Fired if the total results count has been passed to this stream. Represents total number of entries
	/// that can be expected over the course of this streams lifetime.
	/// </summary>
	public event System.Action<int> OnResultsCount;

	/// <summary>
	/// Fired when the stream is fully complete.
	/// </summary>
	public event System.Action Completed;

	/// <summary>
	/// Whether this stream is fully complete.
	/// </summary>
	public bool IsCompleted { get; private set; }

	/// <summary>
	/// Whether this stream has been canceled.
	/// </summary>
	public bool Cancelled { get; private set; }

	/// <summary>
	/// If set, represents the total number of results that can be expected over the lifetime of this stream.
	/// </summary>
	public int ResultsCount { get; private set; }

	/// <summary>
	///  Completes the stream.
	/// </summary>
	public void Complete()
	{
		Completed?.Invoke();
		IsCompleted = true;
	}

	/// <summary>
	/// Pushes a new result to the stream.
	/// </summary>
	public void Push(ResultType _result)
	{
		OnResult?.Invoke(_result);
	}

	/// <summary>
	/// Set the expected total results count for this stream.
	/// </summary>
	public void SetResultsCount(int _resultsCount)
	{
		ResultsCount = _resultsCount;
		OnResultsCount?.Invoke(ResultsCount);
	}

	/// <summary>
	/// Marks the callback as canceled, removing any listeners to Completed event.
	/// </summary>
	public void Cancel()
	{
		Cancelled = true;
		Completed = null;
		OnResult = null;
	}
}
