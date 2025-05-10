using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumbersManager : MonoBehaviour, IGameStage
{
	[SerializeField]
	private int _currentNumber = 1;

	public Number numberPrefab;

	public List<NumberModel> NumbersList { get; } = new List<NumberModel>();

	// event is called when all numbers are added and positions are set
	public event Action OnStageExecutionCompleted;

	private void CreateNumber(NumberModel model = null)
	{
		if (model is not null)
		{
			NumbersList.Add(model);
		}

		if (_currentNumber > GlobalVars.NUMBERS_COUNT)
		{
			//GlobalVars.OnNumbersPlaced?.Invoke();
			OnStageExecutionCompleted?.Invoke();
			return;
		}

		var current = Instantiate(numberPrefab, Number.DefaultPosition, Quaternion.identity);
		var textObject = current.transform.GetComponentInChildren<TextMeshPro>();
		textObject.text = _currentNumber.ToString();
		current.gameObject.SetActive(true);
		current.value = _currentNumber;
		_currentNumber++;

		// call CreateNumber again when drag ended.
		current.OnDragEnded = CreateNumber;
	}

	public void ExecuteStage()
	{
		CreateNumber(null);
	}
}

