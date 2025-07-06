using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
		Debug.Log($"StepTImerScript - Subscribed to playerturnswtich in Timer");
	}

	private void PlayerManager_OnPlayerTurnSwitch()
	{
		Debug.Log($"StepTImerScript - Event triggered in StarTTimer - {nameof(PlayerManager_OnPlayerTurnSwitch)}");
		ResetTimer();
		StartTimer();
	}

	public void StartTimer()
    {
		Debug.Log($"StepTImerScript - Calling StarTTimer");
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
		_currentTimer = GlobalVars.TURN_TIMER_VALUE_SECONDS;

		while (_currentTimer > 0)
		{
			if (_timerTMP != null)
				_timerTMP.text = _currentTimer.ToString(); // Update UI
			yield return new WaitForSeconds(1f);
			_currentTimer--;
		}

		_timerTMP.text = "0"; // Optional
        gameObject.SetActive(false);
		
		// TODO - Switch turns in Multiplayer and refresh Step Timer on Client.
		// Also when game ends with timer elapsed the client receives last AddBrigdeToPlayer but the End screen does not count this.
		if (GameManager.GameMode == GameMode.Multiplayer)
		{
			if (NetworkManager.Singleton.IsHost)
			{
				StepTimerFinished?.Invoke();
			}
		}
		else
		{
			StepTimerFinished?.Invoke();
		}


		Debug.Log("Countdown finished!");
	}
}
