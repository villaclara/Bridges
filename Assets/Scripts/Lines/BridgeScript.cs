using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BridgeScript : NetworkBehaviour
{
	public IPlayerModel currentPlayer;

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

}
