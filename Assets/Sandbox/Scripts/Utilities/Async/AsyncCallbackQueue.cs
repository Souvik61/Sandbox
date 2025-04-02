using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows queuing of multiple asynchronous callbacks that will be executed a maximum once per frame until the queue is depleted.
/// </summary>
public class AsyncCallbackQueue<T>
{
    private readonly Queue<Func<AsyncResult<T>>> callbacks = new Queue<Func<AsyncResult<T>>>();
    private bool queueEvaluating;
    private int lastEvaluatedFrame;
    private AsyncResult<T> resultCallback;
    private bool firstItem = true;
    private Func<T, T, T> resultEvaluator;
    private T lastResult = default;

    /// <summary>
    /// Enqueue a new callback that will be run when previous queued callbacks have completed.
    /// </summary>
    public void Enqueue(Func<AsyncResult<T>> _callback)
    {
        callbacks.Enqueue(_callback);
        if (callbacks.Count == 1 && !queueEvaluating)
            EvaluateQueue();
    }

    /// <summary>
    /// Clears the queue and cancels any currently running callbacks.
    /// </summary>
    public void Clear()
    {
        callbacks.Clear();
        resultCallback?.Cancel();
    }

    /// <summary>
    /// uses result evaluator to summarise a callback value and returns it on completion of the queue
    /// </summary>
    /// <param name="_resultEvaluator"> first argument = last return value , second argument = new return value</param>
    /// <returns></returns>
    public AsyncResult<T> OnComplete(Func<T, T, T> _resultEvaluator)
    {
        resultCallback = new AsyncResult<T>();

        //check if there is a active task running 
        if (queueEvaluating == false)
        {
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                resultCallback.Complete(lastResult);
            }));
        }
        else
        {
            resultEvaluator = _resultEvaluator;

        }

        return resultCallback;
    }

    private void EvaluateQueue()
    {
        queueEvaluating = true;

        void RunCallback()
        {
            lastEvaluatedFrame = Time.frameCount;
            var result = callbacks.Dequeue().Invoke();
            result.OnComplete(_ =>
            {
                if (resultEvaluator == null || firstItem)
                {
                    firstItem = false;
                    lastResult = _;
                }
                else
                {
                    lastResult = resultEvaluator(lastResult, _);
                }


                CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
                {
                    if (callbacks.Count == 0)
                    {
                        queueEvaluating = false;
                        if (resultCallback != null)
                        {
                            resultCallback.Complete(lastResult);
                        }
                    }
                    else
                    {
                        EvaluateQueue();
                    }
                }));
            });
        }

        if (Time.frameCount == lastEvaluatedFrame && false)
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(RunCallback));
        else
            RunCallback();
    }

}
