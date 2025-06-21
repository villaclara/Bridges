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
	//[SerializeField] private PlayerManager _playerManager;

    private NetworkVariable<FixedString64Bytes> _playerDrawingChanged = new();

	private NetworkVariable<bool> _isP1Turn = new();

	private void Start()
	{
		_playerDrawingChanged.OnValueChanged += OnValueChanged;
		_isP1Turn.OnValueChanged += OnPlayerTurnChanged;
	}

	private void OnPlayerTurnChanged(bool previousValue, bool newValue)
	{
		PlayerManager.playerTurn = newValue ? PlayerTurn.P1_Turn : PlayerTurn.P2_Turn;
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
		base.OnDestroy();
	}

	public override void OnNetworkSpawn()
	{
		if(IsServer)
		{
			PlayerManager.OnPlayerTurnSwitch += PlayerManager_OnPlayerTurnSwitch;
		}
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
		Debug.Log($"new IsP1Turn - {_isP1Turn}");	
		Debug.Log($"OnPlayerTurnSwitch - {PlayerManager.playerTurn}");
	}
}
