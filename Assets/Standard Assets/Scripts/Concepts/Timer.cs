using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AmbitiousSnake;

[Serializable]
public class Timer
{
	public float duration;
	public float timeRemaining;
	float timeElapsed;
	public float TimeElapsed
	{
		get
		{
			return timeElapsed;
		}
		private set
		{
			timeElapsed = value;
		}
	}
	public bool loop;
	public delegate void OnFinished ();
	public event OnFinished onFinished;
	bool isRunning;
	public bool IsRunning
	{
		get
		{
			return isRunning;
		}
		private set
		{
			isRunning = value;
		}
	}
	Coroutine timerRoutine;
	public bool realtime;
	public bool pauseWhileNotFocused = true;
	public bool autoStopIfNotLooping = true;
	bool isFocused = true;

	public virtual void Start ()
	{
		if (timerRoutine == null)
			timerRoutine = GameManager.GetSingleton<GameManager>().StartCoroutine(TimerRoutine ());
	}

	public virtual void Stop ()
	{
		if (timerRoutine != null)
		{
			GameManager.GetSingleton<GameManager>().StopCoroutine(timerRoutine);
			timerRoutine = null;
			isRunning = false;
		}
	}

	public virtual IEnumerator TimerRoutine ()
	{
		isRunning = true;
		bool justEnded;
		while (true)
		{
			justEnded = false;
			if (!pauseWhileNotFocused || isFocused)
			{
				if (realtime)
				{
					timeRemaining -= Time.unscaledDeltaTime;
					timeElapsed += Time.unscaledDeltaTime;
				}
				else
				{
					timeRemaining -= Time.deltaTime;
					timeElapsed += Time.deltaTime;
				}
			}
			while (timeRemaining <= 0)
			{
				if (onFinished != null)
					onFinished ();
				if (loop)
					timeRemaining += duration;
				else if (autoStopIfNotLooping)
					Stop ();
				justEnded = true;
				yield return new WaitForEndOfFrame();
			}
			if (!justEnded)
				yield return new WaitForEndOfFrame();
		}
	}

	public virtual void OnApplicationFocus (bool focused)
	{
		isFocused = focused;
	}

	public virtual void Reset ()
	{
		Stop ();
		timeRemaining = duration;
		timeElapsed = 0;
	}
}
