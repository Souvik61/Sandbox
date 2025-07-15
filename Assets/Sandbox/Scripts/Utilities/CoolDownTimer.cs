using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownTimer
{
    private int _remainingTime;
    private int _completedTime;
    private int _totalTime;
    private MonoBehaviour _mono;

    public Action<int> OnTimerTick;
    public Action OnTimerComplete;
    public Action<int> OnCountTimer;

    private IEnumerator _timerTickingCoroutine;
    private IEnumerator _timerCountingCoroutine;
    //public UnityTimer(MonoBehaviour mono, int timeDuration)
    //{
    //    if (timeDuration <= 0)
    //        throw new Exception("Time Duration should always greater than 0");
    //    _mono = mono;
    //    _remainingTime = timeDuration;
    //}

    public CoolDownTimer(MonoBehaviour mono)
    {
        //if (timeDuration <= 0)
        //    throw new Exception("Time Duration should always greater than 0");
        _mono = mono;
        //_remainingTime = timeDuration;
    }
    //public void StartTimer()
    //{
    //    if (_mono == null)
    //        throw new NullReferenceException();
    //    _mono.StartCoroutine(OnTimerTicking());
    //}
    public void StartTimer(int timeDuration)
    {
        Debug.Log("Timer started with time" + timeDuration);
        if (timeDuration <= 0)
            throw new Exception("Time Duration should always greater than 0");
        _totalTime = timeDuration;
        _remainingTime = timeDuration;
        _completedTime = 0;
        if (_mono == null)
            throw new NullReferenceException();

        OnTimerTick?.Invoke(_remainingTime);
        OnCountTimer?.Invoke(_remainingTime);

        if (_timerTickingCoroutine != null)
            _mono.StopCoroutine(_timerTickingCoroutine);
        if (_timerCountingCoroutine != null)
            _mono.StopCoroutine(_timerCountingCoroutine);

        _timerTickingCoroutine = OnTimerTicking();
        _timerCountingCoroutine = OnTimerCounting();

        _mono.StartCoroutine(_timerTickingCoroutine);
        _mono.StartCoroutine(_timerCountingCoroutine);
    }

    IEnumerator OnTimerCounting()
    {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log(_remainingTime);
        _completedTime += 1;
        if (_completedTime == _totalTime)
        {
            OnCountTimer?.Invoke(_completedTime);
            OnTimerComplete?.Invoke();
            StopTimer();
        }
        else
        {
            OnCountTimer?.Invoke(_completedTime);
            if (_timerCountingCoroutine != null)
                _mono.StopCoroutine(_timerCountingCoroutine);

            _timerCountingCoroutine = OnTimerCounting();
            _mono.StartCoroutine(_timerCountingCoroutine);
        }
    }
    IEnumerator OnTimerTicking()
    {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log(_remainingTime);
        _remainingTime -= 1;
        if (_remainingTime == 0)
        {
            OnTimerTick?.Invoke(_remainingTime);
            OnTimerComplete?.Invoke();
            StopTimer();
        }
        else if (_remainingTime < 0)
        {
            OnTimerTick?.Invoke(_remainingTime);
            OnTimerComplete?.Invoke();
            StopTimer();
        }
        else
        {
            OnTimerTick?.Invoke(_remainingTime);
            if (_timerTickingCoroutine != null)
                _mono.StopCoroutine(_timerTickingCoroutine);

            _timerTickingCoroutine = OnTimerTicking();
            _mono.StartCoroutine(_timerTickingCoroutine);
        }
    }
    public void CountTimer()
    {
        //Debug.Log("Timer started with time" + timeDuration);
        _completedTime = 0;
        if (_mono == null)
            throw new NullReferenceException();
        _mono.StartCoroutine(OnTimerCountingContinuously());
    }

    IEnumerator OnTimerCountingContinuously()
    {
        yield return new WaitForSeconds(1.0f);
        _completedTime += 1;
        Debug.Log("TImer ongoing");
        OnCountTimer?.Invoke(_completedTime);
        _mono.StartCoroutine(OnTimerCountingContinuously());
    }
    public void ChangeTimer(int timerChanged)
    {

        _remainingTime -= timerChanged;
        _completedTime += timerChanged;
    }

    public void StopTimer()
    {
        if (_timerTickingCoroutine != null)
            _mono.StopCoroutine(_timerTickingCoroutine);
        if (_timerCountingCoroutine != null)
            _mono.StopCoroutine(_timerCountingCoroutine);

        _timerTickingCoroutine = OnTimerTicking();
        _timerCountingCoroutine = OnTimerCounting();

        _mono.StopCoroutine(_timerTickingCoroutine);
        _mono.StopCoroutine(_timerCountingCoroutine);

    }
}