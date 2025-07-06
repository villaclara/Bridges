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

		// Subscribig to playerTurn change in Multiplayer Network Variable to handle Timer.
		// However in Host the PlayerManager_OnPlayerTurnSwitch() (the Timer reset and start) will be called twice. But who cares?
		MP_PlayerDrawing.OnPlayerTurnChangedInMPNetworkVar += PlayerManager_OnPlayerTurnSwitch;
		Debug.Log($"StepTImerScript - Subscribed to playerturnswtich in Timer");
	}

	/// <summary>
	/// Stops current and starts new timer.
	/// </summary>
	public void PlayerManager_OnPlayerTurnSwitch()
	{
		Debug.LogWarning($"StepTImerScript - Event triggered in StarTTimer - {nameof(PlayerManager_OnPlayerTurnSwitch)}");
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

	/// <summary>
	/// Uses <see cref="WaitForSeconds"/> to get timer. Executes <see cref="StepTimerFinished"/> if timer finished.
	/// </summary>
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
		Debug.LogWarning($"GetCounter Timer Method reached 0");

		_timerTMP.text = "0"; // Optional
        gameObject.SetActive(false);

		// TODO - Switch turns in Multiplayer and refresh Step Timer on Client.
		// Also when game ends with timer elapsed the client receives last AddBrigdeToPlayer but the End screen does not count this.
		
		Debug.LogWarning($"Invoking StepTimerFinished");
		
		// Invoking the Finished Event. 
		// In MP - then subscribers (Host and Client) decide who calls MoveNext based on PlayerManager.playerTurn.
		// In Local - nothing needed, it works.
		StepTimerFinished?.Invoke();
		Debug.Log("Countdown finished!");
	}
}
