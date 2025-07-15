using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is for when you have a group of events happening async and want to wait for them all to finish before completing 
public class CallbackGroupHandler
{
    private int PendingCallbacks = 0;

    private int SuccessCallbacks = 0;

    private int FailedCallbacks = 0;

    private bool enableTiggering = false;

    private Action OnSuccess;

    private Action OnMixed;

    private Action OnFail;

    private Action OnFinished;

    public CallbackGroupHandler(Action _finished = null, Action _success = null, Action _mixed = null, Action _fail = null)
    {
        OnFinished = _finished;
        OnSuccess = _success;
        OnMixed = _mixed;
        OnFail = _fail;
    }

    public Action OnSuccessCallback()
    {
        PendingCallbacks++;
        return () =>
            {
                SuccessCallbacks++;
                CheckIfFinished();
            };
    }

    public Action OnFailCallback(bool hasAlredyRegisteredSuccess = true)
    {
        if (!hasAlredyRegisteredSuccess)
        {
            PendingCallbacks++;
        }

        return () =>
        {
            FailedCallbacks++;
            CheckIfFinished();
        };
    }

    public void EnableTriggering()
    {
        enableTiggering = true;

        CheckIfFinished();
    }

    public void CheckIfFinished()
    {
        if(enableTiggering == false)
        {
            return;
        }

        if (PendingCallbacks <= SuccessCallbacks + FailedCallbacks)
        {
            OnFinished?.Invoke();

            if (FailedCallbacks == 0)
            {
                OnSuccess?.Invoke();
            }

            if (SuccessCallbacks > 0 && FailedCallbacks > 0)
            {
                OnMixed?.Invoke();
            }

            if (SuccessCallbacks == 0)
            {
                OnFail?.Invoke();
            }
        }
    }
}
