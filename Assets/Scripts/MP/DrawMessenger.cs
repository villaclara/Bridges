using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DrawMessenger : NetworkBehaviour
{

	private NumbersList _numbers;

	[SerializeField] private Line _linePrefab;
	private Line _currentLine;

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
			_numbers.MoveNext();
			Debug.Log($"In RPC Numbers move - IsHost - {IsHost}, IsClient - {IsClient}, Numbers Current - {_numbers.Current.Value}");
		}
		// else if we sent from Client then only invoke on Host.
		else if(!sentFromHost && IsHost)
		{
			_numbers.MoveNext();
		Debug.Log($"In RPC Numbers move - IsHost - {IsHost}, IsClient - {IsClient}, Numbers Current - {_numbers.Current.Value}");
		}
	}


	/// <summary>
	/// Sends to the Server request to instantiate new line object and spawn Network Object.
	/// </summary>
	/// <param name="position"></param>
	[Rpc(SendTo.Server)]
	public void RequestServerSpawnLineNetworkObjRpc(Vector3 position)
	{
		if (!IsServer)
		{
			return;
		}
		var newLine = Instantiate(_linePrefab, position, Quaternion.identity);
		//newLine.GetComponent<NetworkObject>().Spawn();
		_currentLine = newLine;
	}

	/// <summary>
	/// Sends to the Server request to Add new position to the current Line.
	/// Current line is private var set in <see cref="RequestServerSpawnLineNetworkObjRpc(Vector3)"/>.
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
}
