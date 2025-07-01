using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
	public GameObject Bridge;

	[SerializeField]
	private PlayerManager _playerManager;

	[SerializeField]
	private Sprite _p1Sprite;
	[SerializeField]
	private Sprite _p2Sprite;

	private bool _canPlaceBridge = true;
	public static float Radius;


	private readonly List<GameObject> _bridgesToDelete = new();

	public DrawMessenger drawMessenger;

	private void Awake()
	{
		Radius = GetComponent<CircleCollider2D>().radius;
		Debug.Log($"_canplacebridge in AWAKE {_canPlaceBridge}");
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Debug.Log($"Intersection Collider - OnTriggerEnter - {collision.gameObject.name}, tag {collision.gameObject.tag}, _canplacebridge - {_canPlaceBridge}");
		Debug.Log($"Inters Collider - On Trigger Enter - Ishost - {NetworkManager.Singleton.IsHost}, IsClient - {NetworkManager.Singleton.IsClient}, ");
		if (_canPlaceBridge && collision.CompareTag("Line"))
		{
			Debug.Log("Line collision, can place bridge");

			GameObject bridge;

			bridge = Instantiate(Bridge, new Vector3(transform.position.x, transform.position.y, -5), Quaternion.identity);

			if(GameManager.GameMode == GameMode.Multiplayer)
			{
				if(NetworkManager.Singleton.IsHost)
				{
					bridge.GetComponent<NetworkObject>().Spawn();
				}
				else
				{
					drawMessenger.RequestInstantiateBridgeOnServerRpc(new Vector3(transform.position.x, transform.position.y, -5));
				}
			}

			GlobalVars.bridgesToDelete.Add(bridge);
			//_bridgesToDelete.Add(bridge);
			Action a = PlayerManager.playerTurn switch
			{
				PlayerTurn.P1_Turn => () =>
				{
					bridge.GetComponent<BridgeScript>().currentPlayer = PlayerManager.player1;

					// Assign currentPlayer field of BridgeScript to P1 on Server (will update the Client too).
					// This is needed to keep track of which bridge is assigned to which player.
					if (NetworkManager.Singleton.IsHost)
					{
						bridge.GetComponent<BridgeScript>().RequestSetCurrentPlayerRpc(true);
					}
					else
					{
						// Ask to add reference to currentPlayer in local copy of Server (in Client we do it above).
						drawMessenger.RequestAssingBridgeToPlayerOnServerRpc(true);
					}
					ChangeSpriteOfBridge(bridge, _p1Sprite);
					AddBridgeToPlayer(PlayerManager.player1, 1);
				}
				,
				PlayerTurn.P2_Turn => () =>
				{
					bridge.GetComponent<BridgeScript>().currentPlayer = PlayerManager.player2;

					// Assign currentPlayer field of BridgeScript to P1 on Server (will update the Client too).
					// This is needed to keep track of which bridge is assigned to which player.
					if (NetworkManager.Singleton.IsHost)
					{
						bridge.GetComponent<BridgeScript>().RequestSetCurrentPlayerRpc(isP1: false);
					}
					else
					{
						// Ask to add reference to currentPlayer in local copy of Server (in Client we do it above).
						drawMessenger.RequestAssingBridgeToPlayerOnServerRpc(isP1: false);
					}
					ChangeSpriteOfBridge(bridge, _p2Sprite);
					AddBridgeToPlayer(PlayerManager.player2, 1);
				}
				,
				_ => () => Debug.Log($"Switch player turn did not find proper value")
			};
			a.Invoke();

			Debug.Log($"PlayerTurn - {PlayerManager.playerTurn}");
			Debug.Log($"P1 - {PlayerManager.player1.IsMyTurn}");
			Debug.Log($"P2 - {PlayerManager.player2.IsMyTurn}");
			Debug.Log($"Current Player Bridge Is My TUrn - {bridge.GetComponent<BridgeScript>().currentPlayer.IsMyTurn}");	

				Debug.Log($"Plyayer ({PlayerManager.player1.Id} - score - {PlayerManager.player1.BridgesCount})");
			Debug.Log($"Plyayer ({PlayerManager.player2.Id} - score - {PlayerManager.player2.BridgesCount})");
			//GlobalVars.score += 1;
		}
		else if (collision.CompareTag("Bridge"))
		{
			Debug.Log("Can not place bidge in Bridge tag");
			_canPlaceBridge = false;

			Debug.Log($"PlayerTurn In collision Bridge  - {PlayerManager.playerTurn}");
			Debug.Log($"Current Player collision Is My TUrn - {collision.GetComponent<BridgeScript>().currentPlayer.IsMyTurn}");

			if (!collision.GetComponent<BridgeScript>().currentPlayer.IsMyTurn)
			{
				Action a = PlayerManager.playerTurn switch
				{
					PlayerTurn.P1_Turn => () => AddBridgeToPlayer(PlayerManager.player1, 5),
					PlayerTurn.P2_Turn => () => AddBridgeToPlayer(PlayerManager.player2, 5),
					_ => () => Debug.Log($"Switch player turn did not find proper value")
				};
				a.Invoke();

				Debug.Log($"Plyayer ({PlayerManager.player1.Id} - score - {PlayerManager.player1.BridgesCount})");
				Debug.Log($"Plyayer ({PlayerManager.player2.Id} - score - {PlayerManager.player2.BridgesCount})");
			}


		}
		else if (collision.CompareTag("Number"))
		{
			Debug.Log("Can not place bidge in Nnunmber Tag");
			_canPlaceBridge = false;
			return;
		}

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Bridge") || collision.CompareTag("Number"))
		{
			//Debug.Log("Can place bidge");
			_canPlaceBridge = true;
			Debug.Log($"Inters Collider - ONTriggerEXIT collisiion - {collision.gameObject.name}, tag {collision.gameObject.tag},  canplacebridge - {_canPlaceBridge}");
		}
	}

	private void ChangeSpriteOfBridge(GameObject bridge, Sprite sprite)
	{
		bridge.GetComponent<SpriteRenderer>().sprite = sprite;

		// Change sprite in Multiplayer
		if(GameManager.GameMode == GameMode.Multiplayer)
		{
			if(NetworkManager.Singleton.IsHost)
			{
				// If we Host then we spawn Network Object bridge
				// And change on client the network object sprite.
				bridge.GetComponent<BridgeScript>().RequestBridgeSpriteChangeRpc();
			}
			else
			{
				// If we client then we send request to DrawMessenger
				// DrawMessenger has reference to LOCAL object of Bridge in Host
				// And in Host we change the sprite locally.
				drawMessenger.RequestBridgeSpriteChangeRpc();
			}
		}
	}

	private void AddBridgeToPlayer(IPlayerModel player, int count)
	{
		if(GameManager.GameMode == GameMode.Multiplayer)
		{
			if (!NetworkManager.Singleton.IsHost)
			{
				// request add bridge on server
				drawMessenger.RequestAddBridgeCountToPlayerOnServerRpc(player.Id == 1, count);
				return;
			}
		}

		// Invoke if local and if Server in Multiplayer
		PlayerManager.AddBridgeToPlayer(player, count);
	}

	public void DestroyAllBridges()
	{
		foreach (var bridge in GlobalVars.bridgesToDelete)
		{
			Destroy(bridge);
		}
	}

}
