using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class MP_PlayerDrawing : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerDrawingTMP;

    private NetworkVariable<FixedString64Bytes> _playerDrawingChanged = new();

	private void Start()
	{
		_playerDrawingChanged.OnValueChanged += OnValueChanged;
	}

	private void OnValueChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
	{
		_playerDrawingTMP.text = newValue.ToString();
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
	}
}
