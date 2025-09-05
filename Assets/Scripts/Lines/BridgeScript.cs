using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BridgeScript : NetworkBehaviour
{
	public IPlayerModel currentPlayer;
	public int PlayerId => currentPlayer.Id;

	public List<Sprite> _bridgeSprites;

	/// <summary>
	/// Changes the sprite of the Bridge in Client.
	/// Bridge is spawned as NetworkObject from Server.
	/// </summary>
	[Rpc(SendTo.ClientsAndHost)]
	public void RequestBridgeSpriteChangeRpc()
	{
		if(IsServer)
		{
			return;
		}
		GetComponent<SpriteRenderer>().sprite = _bridgeSprites[0];

	}

	/// <summary>
	/// Asks the server to assign local currentPlayer field to the Player. Needed to keep track which player owns this bridge.
	/// </summary>
	/// <param name="isP1">is Player1.</param>
	[Rpc(SendTo.ClientsAndHost)]
	public void RequestSetCurrentPlayerRpc(bool isP1)
	{
		if (IsServer)
		{
			return;
		}
		currentPlayer = isP1 ? PlayerManager.player1 : PlayerManager.player2;
		
	}

}
