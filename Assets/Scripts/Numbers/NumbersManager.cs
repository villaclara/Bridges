using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manager CLass for Drawing Numbers stage. 
/// Handles the Stage execution. 
/// </summary>
public class NumbersManager : MonoBehaviour, IGameStage
{
	[SerializeField] private int _currentNumber = 1;
	[SerializeField] private TextMeshProUGUI _playerDrawingText;
	[SerializeField] private MP_StageSetup _stageSetup;

	private readonly NumbersList _numbersList = NumbersList.GetInstance();
	private readonly List<GameObject> _numbersToDelete = new();
	
	// event is called when all numbers are added and positions are set
	public event Action OnStageExecutionCompleted;
	public NumberScript numberPrefab;
	public NumberMessenger numberMessenger;

	private void Awake()
	{
		numberMessenger.SetNumbersListReference(_numbersList);
	}

	/// <summary>
	/// Creates the new <see cref="NumberModel"/> object and checks the stage execution.
	/// </summary>
	/// <param name="model"></param>
	private void CreateNumber(NumberModel model = null)
	{
		if (model is not null)
		{
			// If MP then we pass the created Number Model to all clients to add to their respective _numbersList.
			if(GameManager.GameMode == GameMode.Multiplayer)
			{
				var no = model.NumberObject.GetComponent<NetworkObject>();
				numberMessenger.AddNumberToList(model.Value, model.Position, model.Radius, no.NetworkObjectId);
			}
			// local normal behavior.
			else
			{
				_numbersList.Add(model);
			}
		}

		if (_currentNumber > GlobalVars.NUMBERS_COUNT)
		{
			_numbersList.Setup();
			gameObject.SetActive(false);
			_playerDrawingText.text = "              ";
			if(GameManager.GameMode == GameMode.Multiplayer)
			{
				numberMessenger.SetupNumbersList();
				_stageSetup.SetDrawStageRpc();
			}
			else
			{
				InvokeStageEnd();
			}
			SpinningCircleHelper.SetSpinningCircleForNumberModel(model, false);
            return;
		}
        SpinningCircleHelper.SetSpinningCircleForNumberModel(model, false);

		if(GameManager.GameMode == GameMode.Multiplayer)
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
		_playerDrawingText.text = $"P1 is drawing";
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

	/// <summary>
	/// Instantiates new <see cref="NumberScript"/> GO.
	/// </summary>
	/// <param name="spawnNetworkObject">Wheter to spawn NetworkObject related to this GO.</param>
	private void InstantiateNewNumber(bool spawnNetworkObject)
	{
		var current = Instantiate(numberPrefab, NumberScript.DefaultPosition, Quaternion.identity);
		_numbersToDelete.Add(current.gameObject);
		var textObject = current.transform.GetComponentInChildren<TextMeshPro>();
		textObject.text = _currentNumber.ToString();
		current.value = _currentNumber;
		current.gameObject.SetActive(true);

		// call CreateNumber again when drag ended.
		current.OnDragEnded = CreateNumber;

		// only if multiplayer active we spawn and send network object
		if (spawnNetworkObject)
		{
			current.GetComponent<NetworkObject>().Spawn();
			current.GetComponent<MP_Number>().SetNumberValueInClient(_currentNumber);
		}
		_currentNumber++;
	}

	public void InvokeStageEnd()
	{
		OnStageExecutionCompleted?.Invoke();
	}
}

