using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumbersManager : MonoBehaviour, IGameStage
{
	[SerializeField]
	private int _currentNumber = 1;

	public NumberScript numberPrefab;

	[SerializeField]
	private TextMeshProUGUI _playerDrawingText;

	private NumbersList _numbersList = NumbersList.GetInstance();

	// event is called when all numbers are added and positions are set
	public event Action OnStageExecutionCompleted;

	private readonly List<GameObject> _numbersToDelete = new();

	private void CreateNumber(NumberModel model = null)
	{
		if (model is not null)
		{
			_numbersList.Add(model);
		}

		if (_currentNumber > GlobalVars.NUMBERS_COUNT)
		{
			//GlobalVars.OnNumbersPlaced?.Invoke();

			_numbersList.Setup();
			gameObject.SetActive(false);
			_playerDrawingText.gameObject.SetActive(false);
			OnStageExecutionCompleted?.Invoke();
			Debug.Log("Numbermanager after completed calling invoke.");
			return;
		}

		var current = Instantiate(numberPrefab, NumberScript.DefaultPosition, Quaternion.identity);
		_numbersToDelete.Add(current.gameObject);
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
		gameObject.SetActive(true);

		uint seed = (uint)System.Environment.TickCount;
		if (seed == 0) seed = 1;
		var rnd = new Unity.Mathematics.Random(seed).NextInt2();
		var rnd1 = new System.Random().Next(1, 3);
		_playerDrawingText.text = $"Player {rnd1} drawing";
		_playerDrawingText.gameObject.SetActive(true);
		CreateNumber(null);
	}

	public void DestroyAllNumbers()
	{
		foreach (var number in _numbersToDelete)
		{
			Destroy(number);
		}

		_currentNumber = 1;
		_numbersList.RemoveAll();
	}
}

