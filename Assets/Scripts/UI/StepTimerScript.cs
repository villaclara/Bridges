using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Countdown script for the player turn.
/// </summary>
public class StepTimerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerTMP;

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
	}

	/// <summary>
	/// Stops current and starts new timer.
	/// </summary>
	public void PlayerManager_OnPlayerTurnSwitch()
	{
		ResetTimer();
		StartTimer();
	}

	/// <summary>
	///  Starts the time.
	/// </summary>
	public void StartTimer()
    {
        gameObject.SetActive(true);
		_coroutine = StartCoroutine(StartTimerEnumerator());
    }

	/// <summary>
	/// Reset timer to default value from <see cref="GlobalVars"/>.
	/// </summary>
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

		_timerTMP.text = "0"; // Optional
        gameObject.SetActive(false);
		
		// Invoking the Finished Event. 
		// In MP - then subscribers (Host and Client) decide who calls MoveNext based on PlayerManager.playerTurn.
		// In Local - nothing needed, it works.
		StepTimerFinished?.Invoke();
	}
}
