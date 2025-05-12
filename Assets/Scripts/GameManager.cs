using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	// List of all stages of the game flow (setup numbers, draw lines, end).
	[SerializeField]
	private List<IGameStage> _stages = new();

	[Header("Manager References")]
	public NumbersManager numbersManager;
	public DrawManager drawManager;

	private int _currentStageIndex = 0;

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

}
