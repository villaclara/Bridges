using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DrawMessenger : NetworkBehaviour
{

	private NumbersList _numbers;

	[SerializeField] private Line _linePrefab;
	[SerializeField] private BridgeScript _bridgePrefab;
	private Line _currentLine;
	private BridgeScript _currentBridge;

	[SerializeField]
	private List<Sprite> _bridgeSprites;

	/// <summary>
	/// Is invoked when using _numbers.MoveNext() returns false, meaning its endgame.
	/// Is invoked on another client (we invoke on local client in <see cref="DrawManager"/> script.
	/// </summary>
	public event Action NumbersMoveNextReturnFalse;

	public void SetNumbersListReference(NumbersList numbersList)
	{
		_numbers = numbersList;
	}

	/// <summary>
	/// Sends Rpc to another client to invoke _numbers.MoveNext() to update numbers.Current and numbers.Next.
	/// </summary>
	public void NumbersMoveNext(bool isHost)
	{
		NumbersMoveNextRpc(isHost);
	}

	[Rpc(SendTo.ClientsAndHost)]
	private void NumbersMoveNextRpc(bool sentFromHost)
	{
		// If we sent from Host then we only want to invoke on Client.
		if(sentFromHost && !IsHost)
		{
			if (!_numbers.MoveNext())
			{
				NumbersMoveNextReturnFalse?.Invoke();
			}
		}
		// else if we sent from Client then only invoke on Host.
		else if(!sentFromHost && IsHost)
		{
			if (!_numbers.MoveNext())
			{
				NumbersMoveNextReturnFalse?.Invoke();
			}
		}	
	}


	/// <summary>
	/// Sends to the Server request to instantiate new line only Locally and set color of P2.
	/// The Line of Client is spawned locally on Client side too.
	/// </summary>
	/// <param name="position"></param>
	[Rpc(SendTo.Server)]
	public void RequestServerSpawnLineOnServerRpc(Vector3 position)
	{
		if (!IsServer)
		{
			return;
		}
		var newLine = Instantiate(_linePrefab, position, Quaternion.identity);
		//newLine.GetComponent<NetworkObject>().Spawn();
		_currentLine = newLine;
		// Set line color of P2 localy only on Server.
		_currentLine.SetLineColor(PlayerManager.player2.ColorHEX);
	}

	/// <summary>
	/// Sends to the Server request to Add new position to the current Line.
	/// Current line is private var set in <see cref="RequestServerSpawnLineOnServerRpc(Vector3)"/>.
	/// </summary>
	[Rpc(SendTo.Server)]
	public void RequestAddVertextToLineRpc(Vector3 position)
	{
		if (!IsServer)
		{
			return;
		}

		if(_currentLine == null)
		{
			return;
		}

		_currentLine.SetPosition(position);
	}

	[Rpc(SendTo.Server)]
	public void RequestPlayerTurnSwitchRpc()
	{
		if (!IsServer)
		{
			return;
		}
		Debug.Log($"In host Reqeust to switch turns");
		PlayerManager.SwitchTurns();
		Debug.Log($"In host After switching turns - {PlayerManager.playerTurn}");
	}

	[Rpc(SendTo.Server)]
	public void RequestInstantiateBridgeOnServerRpc(Vector3 position)
	{
		if (!IsServer)
		{
			return;
		}
		_currentBridge = Instantiate(_bridgePrefab, position, Quaternion.identity);
	}

	/// <summary>
	/// Changes the sprite of bridge locally only on Host.
	/// </summary>
	[Rpc(SendTo.Server)]
	public void RequestBridgeSpriteChangeRpc()
	{
		if(!IsServer)
		{
			return;
		}
		_currentBridge.GetComponent<SpriteRenderer>().sprite = _bridgeSprites[1];
	}

	/// <summary>
	/// Sends the request to Server to call PlayerManager.AddBridgeToPlayer. 
	/// It will trigger event OnBridgesCountChanges and it will set the Bridges count on both Clients.
	/// </summary>
	[Rpc(SendTo.Server)]
	public void RequestAddBridgeToPlayerOnServerRpc(bool isP1, int count)
	{
		if (!IsServer)
		{
			return;
		}

		var player = isP1 ? PlayerManager.player1 : PlayerManager.player2;
		PlayerManager.AddBridgeToPlayer(player, count);
	}

	/// <summary>
	/// Sends request to Server to assign currentPlayer field of BridgeScript. To keep track which player owns the bridge. 
	/// Also updates the clients.
	/// </summary>
	[Rpc(SendTo.Server)]
	public void RequestSetBridgePlayerOnServerRpc(bool isP1)
	{
		if (!IsServer)
		{
			return;
		}

		var player = isP1 ? PlayerManager.player1 : PlayerManager.player2;
		_currentBridge.GetComponent<BridgeScript>().currentPlayer = player; 
	}
}
