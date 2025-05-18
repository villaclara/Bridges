using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	// List of all stages of the game flow (setup numbers, draw lines, end).
	[SerializeField]
	private List<IGameStage> _stages = new();

	[Header("Manager References")]
	public NumbersManager numbersManager;
	public DrawManager drawManager;
	public IntersectionCollider intersectionsCollider;

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
		foreach (var stage in _stages)
		{
			stage.ResetStage();
		}
		intersectionsCollider.DestroyAllBridges();
		PlayerManager.ResetBridgesCountForPlayers();
		_currentStageIndex = 0;
		StartStage(_currentStageIndex);
	}



}
