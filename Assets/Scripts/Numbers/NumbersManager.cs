	using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
	private NumberScript prevCircle = null;

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
			//_playerDrawingText.gameObject.SetActive(false);
			_playerDrawingText.text = "              ";
			OnStageExecutionCompleted?.Invoke();
			Debug.Log("Numbermanager after completed calling invoke.");
			SpinningCircleHelper.DisableSpinningCircle(model, false);
            return;
		}
        SpinningCircleHelper.DisableSpinningCircle(model, false);

		if(GameManager.gameMode == GameMode.Multiplayer)
		{
			if (NetworkManager.Singleton.IsHost)
			{
				InstantiateNewNumber(spawnNetworkObject: true);
			}
		}
		else
		{
			InstantiateNewNumber(spawnNetworkObject: false);
		}

	}

	public void ExecuteStage()
	{
		gameObject.SetActive(true);

		var rnd1 = new System.Random().Next(1, 3);
		_playerDrawingText.text = $"P{rnd1} is drawing";
		_playerDrawingText.gameObject.SetActive(true);
		CreateNumber(null);
	}

	
	public void ResetStage()
	{
		foreach (var number in _numbersToDelete)
		{
			Destroy(number);
		}

		_currentNumber = 1;
		_numbersList.RemoveAll();
	}

	private void InstantiateNewNumber(bool spawnNetworkObject)
	{
		var current = Instantiate(numberPrefab, NumberScript.DefaultPosition, Quaternion.identity);
		prevCircle = current;
		_numbersToDelete.Add(current.gameObject);
		var textObject = current.transform.GetComponentInChildren<TextMeshPro>();
		textObject.text = _currentNumber.ToString();
		current.gameObject.SetActive(true);
		current.value = _currentNumber;
		_currentNumber++;

		// call CreateNumber again when drag ended.
		current.OnDragEnded = CreateNumber;

		// only if multiplayer active we spawn and send network object
		if (spawnNetworkObject)
		{
			current.GetComponent<NetworkObject>().Spawn();
		}
	}
}

