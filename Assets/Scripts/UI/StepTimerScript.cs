using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepTimerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerTMP;
    // Start is called before the first frame update

    private int _currentTimer;
	private Coroutine _coroutine;

	public static event Action StepTimerFinished;
	private void Awake()
	{
		PlayerManager.OnPlayerTurnSwitch += PlayerManager_OnPlayerTurnSwitch;
		gameObject.SetActive(false);
		Debug.Log($"Subscribed to playerturnswtich in Timer");
	}

	private void PlayerManager_OnPlayerTurnSwitch()
	{
		Debug.Log($"Event triggered in StarTTimer");
		ResetTimer();
		StartTimer();
	}

	public void StartTimer()
    {
		Debug.Log($"Calling StarTTimer");
        gameObject.SetActive(true);
		_coroutine = StartCoroutine(StartTimerEnumerator());
    }

	public void ResetTimer()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
			gameObject.SetActive(false);
		}
	}
    // Two Coroutines are used because of WebGL to use WaitForSeconds.
    private IEnumerator StartTimerEnumerator()
    {
        yield return GetCounter();
    }

    private IEnumerator GetCounter()
    {
		_currentTimer = 10;

		while (_currentTimer > 0)
		{
			if (_timerTMP != null)
				_timerTMP.text = _currentTimer.ToString(); // Update UI
			yield return new WaitForSeconds(1f);
			_currentTimer--;
		}

		_timerTMP.text = "0"; // Optional
		StepTimerFinished?.Invoke();
        gameObject.SetActive(false);
		Debug.Log("Countdown finished!");
	}
}
