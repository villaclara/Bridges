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

	private void Start()
	{
		_playerDrawingChanged.OnValueChanged += OnValueChanged;
		_isP1Turn.OnValueChanged += OnPlayerTurnChanged;


		_p1Bridges.OnValueChanged += OnP1BridgesCountChanged;
		_p2Bridges.OnValueChanged += OnP2BridgesCountChanged;
	}

	private void OnP2BridgesCountChanged(int previousValue, int newValue)
	{
		Debug.Log($"Prev - {previousValue}, new - {newValue}");
		// TODO - Check if this needed too.
		// Do not redraw text when the value has not changed. Not sure if it is actually needed.
		if(previousValue == newValue)
		{
			return;
		}
		PlayerManager.player2.BridgesCount = newValue;
		_p2TextTMP.text = $"P2 - {newValue}";
	}


	private void OnP1BridgesCountChanged(int previousValue, int newValue)
	{
		Debug.Log($"Prev - {previousValue}, new - {newValue}");
		// Do not redraw text when the value has not changed.
		if (previousValue == newValue)
		{
			return;
		}
		PlayerManager.player1.BridgesCount = newValue;
		_p1TextTMP.text = $"P1 - {newValue}";
	}

	private void OnPlayerTurnChanged(bool previousValue, bool newValue)
	{
		PlayerManager.playerTurn = newValue ? PlayerTurn.P1_Turn : PlayerTurn.P2_Turn;
		// Changing the values in Clients to reflect actual value of player turn.
		PlayerManager.player1.IsMyTurn = PlayerManager.playerTurn == PlayerTurn.P1_Turn;
		PlayerManager.player2.IsMyTurn = PlayerManager.playerTurn == PlayerTurn.P2_Turn;
		Debug.Log($"OnPlayerTurnChanged in isHost - {IsHost}, isClient - {IsClient}, newvalueText - {newValue}, newValuebool - {PlayerManager.playerTurn}");
	}

	private void OnValueChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
	{
		_playerDrawingTMP.text = newValue.ToString();
		Debug.Log($"OnValueChanged in isHost - {IsHost}, isClient - {IsClient}, newvalueText - {newValue}, newValuebool - {PlayerManager.playerTurn}");
	}

	public override void OnDestroy()
	{
		_playerDrawingChanged.OnValueChanged -= OnValueChanged;
		_p1Bridges.OnValueChanged -= OnP1BridgesCountChanged;
		_p2Bridges.OnValueChanged -= OnP2BridgesCountChanged;
		_isP1Turn.OnValueChanged -= OnPlayerTurnChanged;
		base.OnDestroy();
	}

	public override void OnNetworkSpawn()
	{
		if(IsServer)
		{
			PlayerManager.OnPlayerTurnSwitch += PlayerManager_OnPlayerTurnSwitch;
			PlayerManager.OnPlayerBridgesChanged += PlayerManager_OnPlayerBridgesChanged;
		}
	}

	private void PlayerManager_OnPlayerBridgesChanged()
	{
		Debug.Log($"OnPlayerBridgesChanged - p1 ({PlayerManager.player1.BridgesCount}), p2 ({PlayerManager.player2.BridgesCount})");
		_p1Bridges.Value = PlayerManager.player1.BridgesCount;
		_p2Bridges.Value = PlayerManager.player2.BridgesCount;
	}

	private void PlayerManager_OnPlayerTurnSwitch()
	{
		_playerDrawingChanged.Value = PlayerManager.playerTurn switch
		{
			PlayerTurn.P2_Turn => "P2 turn",
			_ => "P1 turn"
		};
		//PlayerManager.playerTurn = PlayerManager.playerTurn;
		_isP1Turn.Value = PlayerManager.playerTurn == PlayerTurn.P1_Turn;
		Debug.Log($"new IsP1Turn - {_isP1Turn.Value}");	
		Debug.Log($"OnPlayerTurnSwitch - {PlayerManager.playerTurn}");
	}
}
