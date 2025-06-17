using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Header("Game Mode")]
	[SerializeField] private bool _isOnline;

	public static GameMode gameMode = GameMode.Local;

	// List of all stages of the game flow (setup numbers, draw lines, end).
	[SerializeField]
	private List<IGameStage> _stages = new();

	[Header("Manager References")]
	public NumbersManager numbersManager;
	public DrawManager drawManager;
	public IntersectionCollider intersectionsCollider;
	public PlayerCanvasStyleController player1CanvasStyleController;
	public PlayerCanvasStyleController player2CanvasStyleController;

	private int _currentStageIndex = 0;

	[SerializeField]
	private GameObject _endGameScreen;

	// Start is called before the first frame update
	void Start()
	{
		_stages.Add(numbersManager);
		_stages.Add(drawManager);

		StartStage(_currentStageIndex);
	}

	void StartStage(int index)
	{
		if (index >= _stages.Count)
		{
			_endGameScreen.SetActive(true);
			var playerIdWon = "Draw";
			if (PlayerManager.player1.BridgesCount < PlayerManager.player2.BridgesCount)
			{
				playerIdWon = "Player 1 won. GG";
			}
			else if (PlayerManager.player1.BridgesCount > PlayerManager.player2.BridgesCount)
			{
				playerIdWon = "Player 2 won. GG";
			}
			_endGameScreen.GetComponentsInChildren<TextMeshProUGUI>()[1].text = playerIdWon;
			Debug.Log("Index outside of array. End of game.");
			return;
		}

		// set first stage and subscribe to event when stage completed
		IGameStage currentStage = _stages[index];
		currentStage.OnStageExecutionCompleted += OnStageCompleted;

		currentStage.ExecuteStage();
	}

	private void OnStageCompleted()
	{
		// remove subscription
		_stages[_currentStageIndex].OnStageExecutionCompleted -= OnStageCompleted;

		// start next stage
		_currentStageIndex++;
		StartStage(_currentStageIndex);
	}

	public void ResetAll()
	{
		Debug.Log($"IsHost - {NetworkManager.Singleton.IsHost}, IcLient - {NetworkManager.Singleton.IsClient}");
		Debug.Log($"Reset All called");
		foreach (var stage in _stages)
		{
			stage.ResetStage();
		}
		intersectionsCollider.DestroyAllBridges();
		PlayerManager.ResetBridgesCountForPlayers();
		player1CanvasStyleController.Reset();
		player2CanvasStyleController.Reset();
		_currentStageIndex = 0;
		StartStage(_currentStageIndex);
	}

	public void SetGameMode(bool isOnline)
	{
		this._isOnline = isOnline;
		gameMode = this._isOnline ? GameMode.Multiplayer : GameMode.Local;
	}

}


public enum GameMode
{
	Local = 0,
	Multiplayer = 1
}