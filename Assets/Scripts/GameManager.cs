using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Header("Game Mode")]
	[SerializeField] private bool _isOnline;

	// List of all stages of the game flow (setup numbers, draw lines, end).
	[SerializeField] private List<IGameStage> _stages = new();
	private int _currentStageIndex = 0;
	public static GameMode GameMode { get; private set; } = GameMode.Local;

	public event Action MP_RestartAsked;
	
	[Header("Manager References")]
	public NumbersManager numbersManager;
	public DrawManager drawManager;
	public IntersectionCollider intersectionsCollider;
	public PlayerCanvasStyleController player1CanvasStyleController;
	public PlayerCanvasStyleController player2CanvasStyleController;
	public GameObject endGameScreen;
	public GameObject localLoadingScreen;
	public GameObject mpLoadingScreen;
	public GameObject stepTimerScript;

	// Start is called before the first frame update
	void Start()
	{
		_stages.Add(numbersManager);
		_stages.Add(drawManager);

		endGameScreen.GetComponent<EndGameScript>().RestartButtonClick += GameManager_RestartButtonClick;
	}

	/// <summary>
	/// Starts the Action Phase. Is used for both Fresh Start and Re Start. <see cref="GameMode.Local"/> is selected by default if no parameter is specified.
	/// </summary>
	/// <param name="isOnline">Sets the <see cref="GameMode"/> GameMode. </param>
	public void StartGame(bool isOnline = false)
	{
		// Resetting all and hiding all UI Screens
		gameObject.SetActive(true);
		ResetAll();
		localLoadingScreen.SetActive(false);
		mpLoadingScreen.SetActive(false);
		endGameScreen.SetActive(false);
		this._isOnline = isOnline;
		GameMode = isOnline ? GameMode.Multiplayer : GameMode.Local;
		StartStage(_currentStageIndex);
	}

	/// <summary>
	/// Starts the <see cref="IGameStage"/> with given index in internal array of stages.
	/// </summary>
	/// <param name="index">Index of stage to start.</param>
	private void StartStage(int index)
	{
		if (index >= _stages.Count)
		{
			endGameScreen.SetActive(true);
			stepTimerScript.GetComponent<StepTimerScript>().ResetTimer();
			var playerIdWon = "Draw";
			if (PlayerManager.player1.BridgesCount < PlayerManager.player2.BridgesCount)
			{
				playerIdWon = "Player 1 won. GG";
			}
			else if (PlayerManager.player1.BridgesCount > PlayerManager.player2.BridgesCount)
			{
				playerIdWon = "Player 2 won. GG";
			}
			endGameScreen.GetComponentsInChildren<TextMeshProUGUI>()[1].text = playerIdWon;
			return;
		}

		// set first stage and subscribe to event when stage completed
		IGameStage currentStage = _stages[index];
		currentStage.OnStageExecutionCompleted += OnStageCompleted;

		currentStage.ExecuteStage();
	}

	/// <summary>
	/// Handles the logic to start next stage when previous stage was completed.
	/// </summary>
	private void OnStageCompleted()
	{
		// remove subscription
		_stages[_currentStageIndex].OnStageExecutionCompleted -= OnStageCompleted;

		// start next stage
		_currentStageIndex++;
		StartStage(_currentStageIndex);
	}

	/// <summary>
	/// Resets all game states to defaults for a new game.
	/// </summary>
	private void ResetAll()
	{
		foreach (var stage in _stages)
		{
			stage.ResetStage();
			stage.OnStageExecutionCompleted -= OnStageCompleted;
		}
		intersectionsCollider.DestroyAllBridges();
		PlayerManager.ResetBridgesCountForPlayers();
		PlayerManager.playerTurn = PlayerTurn.P1_Turn;	// reset turn to p1 so he is drawing
		player1CanvasStyleController.Reset();
		player2CanvasStyleController.Reset();
		_currentStageIndex = 0;
		stepTimerScript.GetComponent<StepTimerScript>().ResetTimer();
	}

	/// <summary>
	/// Handles the Restart Button click depending on <see cref="GameMode"/> value.
	/// </summary>
	private void GameManager_RestartButtonClick()
	{
		// Redirect handling ConnectionStatus for Multiplayer.
		if (GameMode == GameMode.Multiplayer)
		{
			MP_RestartAsked?.Invoke();
		}
		// If local then simply restart.
		else
		{
			StartGame();
		}
	}
}


/// <summary>
/// Represents the possible game modes.
/// </summary>
public enum GameMode
{
	Local = 0,
	Multiplayer = 1
}