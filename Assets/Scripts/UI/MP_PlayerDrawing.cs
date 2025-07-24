using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class MP_PlayerDrawing : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerDrawingTMP;
    [SerializeField] private TextMeshProUGUI _p1TextTMP;
    [SerializeField] private TextMeshProUGUI _p2TextTMP;
	//[SerializeField] private PlayerManager _playerManager;

    private NetworkVariable<FixedString64Bytes> _playerDrawingChanged = new();

	private NetworkVariable<bool> _isP1Turn = new();
	private NetworkVariable<int> _p1Bridges = new();
	private NetworkVariable<int> _p2Bridges = new();

	public static event Action OnPlayerTurnChangedInMPNetworkVar;

	private void Start()
	{
		//_playerDrawingChanged.OnValueChanged += OnValueChanged;
		//_isP1Turn.OnValueChanged += OnPlayerTurnChanged;


		//_p1Bridges.OnValueChanged += OnP1BridgesCountChanged;
		//_p2Bridges.OnValueChanged += OnP2BridgesCountChanged;
	}

	/// <inheritdoc/>
	public override void OnNetworkSpawn()
	{
		if (IsServer)
		{
			PlayerManager.OnPlayerTurnSwitch += PlayerManager_OnPlayerTurnSwitch;
			PlayerManager.OnPlayerBridgesChanged += PlayerManager_OnPlayerBridgesChanged;
		}

		_playerDrawingChanged.OnValueChanged += OnPlayerDrawingValueChanged;
		_isP1Turn.OnValueChanged += OnPlayerTurnNetworkVariableChanged;
		
		_p1Bridges.OnValueChanged += OnP1BridgesCountChanged;
		_p2Bridges.OnValueChanged += OnP2BridgesCountChanged;

		base.OnNetworkSpawn();
	}

	/// <inheritdoc/>
	public override void OnNetworkDespawn()
	{
		if (IsServer)
		{
			PlayerManager.OnPlayerTurnSwitch -= PlayerManager_OnPlayerTurnSwitch;
			PlayerManager.OnPlayerBridgesChanged -= PlayerManager_OnPlayerBridgesChanged;
		}

		_playerDrawingChanged.OnValueChanged -= OnPlayerDrawingValueChanged;
		_isP1Turn.OnValueChanged -= OnPlayerTurnNetworkVariableChanged;

		_p1Bridges.OnValueChanged -= OnP1BridgesCountChanged;
		_p2Bridges.OnValueChanged -= OnP2BridgesCountChanged;

		base.OnNetworkDespawn();
	}

	/// <summary>
	/// Handles the <see cref="_p2TextTMP"/> text in the UI with newest values.
	/// </summary>
	private void OnP2BridgesCountChanged(int previousValue, int newValue)
	{
		// Do not redraw text when the value has not changed. Not sure if it is actually needed.
		if (previousValue == newValue)
		{
			return;
		}
		PlayerManager.player2.BridgesCount = newValue;
		_p2TextTMP.text = $"P2 - {newValue}";
		_p2TextTMP.gameObject.transform.parent.GetComponent<Animation>().Play("PlayerBridgesAnimation");
	}

	/// <summary>
	/// Handles the <see cref="_p1TextTMP"/> text in the UI with newest values.
	/// </summary>
	private void OnP1BridgesCountChanged(int previousValue, int newValue)
	{
		// Do not redraw text when the value has not changed.
		if (previousValue == newValue)
		{
			return;
		}
		PlayerManager.player1.BridgesCount = newValue;
		_p1TextTMP.text = $"P1 - {newValue}";
		_p1TextTMP.gameObject.transform.parent.GetComponent<Animation>().Play("PlayerBridgesAnimation");
	}

	/// <summary>
	/// Handles the change of boolean <see cref="_isP1Turn"/> network variable 
	/// that is updated when <see cref="PlayerManager.OnPlayerTurnSwitch"/> is invoked.
	/// </summary>
	private void OnPlayerTurnNetworkVariableChanged(bool previousValue, bool newValue)
	{
		PlayerManager.playerTurn = newValue ? PlayerTurn.P1_Turn : PlayerTurn.P2_Turn;
		// Changing the values in Clients to reflect actual value of player turn.
		PlayerManager.player1.IsMyTurn = PlayerManager.playerTurn == PlayerTurn.P1_Turn;
		PlayerManager.player2.IsMyTurn = PlayerManager.playerTurn == PlayerTurn.P2_Turn;
		OnPlayerTurnChangedInMPNetworkVar?.Invoke();
	}

	/// <summary>
	/// Handles the UI text of <see cref="_playerDrawingTMP"/>, which indicates which player is currently making turn.
	/// </summary>
	private void OnPlayerDrawingValueChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
	{
		_playerDrawingTMP.text = newValue.ToString();
	}

	/// <summary>
	/// Is called when <see cref="PlayerManager.OnPlayerBridgesChanged"/> is invoked. 
	/// Updates the private network variables of bridges count to sync between players.
	/// Is only called on Server.
	/// </summary>
	private void PlayerManager_OnPlayerBridgesChanged()
	{
		_p1Bridges.Value = PlayerManager.player1.BridgesCount;
		_p2Bridges.Value = PlayerManager.player2.BridgesCount;
	}

	/// <summary>
	/// Is called when <see cref="PlayerManager.OnPlayerTurnSwitch"/> is invoked. 
	/// Updates the private network variables of player turn to sync between players.
	/// Is only called on Server.
	/// </summary>
	private void PlayerManager_OnPlayerTurnSwitch()
	{
		_playerDrawingChanged.Value = PlayerManager.playerTurn switch
		{
			PlayerTurn.P2_Turn => "P2 turn",
			_ => "P1 turn"
		};
		//PlayerManager.playerTurn = PlayerManager.playerTurn;
		_isP1Turn.Value = PlayerManager.playerTurn == PlayerTurn.P1_Turn;
	}
}
