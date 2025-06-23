using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
	}

	// TODO - Count other bridges, do not count self bridge
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_canPlaceBridge && collision.CompareTag("Line"))
		{
			GameObject bridge;

			bridge = Instantiate(Bridge, new Vector3(transform.position.x, transform.position.y, -5), Quaternion.identity);

			if(GameManager.gameMode == GameMode.Multiplayer)
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
			//Debug.Log("Line collision, can place bridge");
			_bridgesToDelete.Add(bridge);
			Action a = PlayerManager.playerTurn switch
			{
				PlayerTurn.P1_Turn => () =>
				{
					bridge.GetComponent<BridgeScript>().currentPlayer = PlayerManager.player1;
					//SetBridgeColor(bridge, PlayerManager.player1.ColorHEX);
					ChangeSpriteOfBridge(bridge, _p1Sprite);
					PlayerManager.AddBridgeToPlayer(PlayerManager.player1, 1);
				}
				,
				PlayerTurn.P2_Turn => () =>
				{
					bridge.GetComponent<BridgeScript>().currentPlayer = PlayerManager.player2;
					//SetBridgeColor(bridge, PlayerManager.player2.ColorHEX);
					ChangeSpriteOfBridge(bridge, _p2Sprite);
					PlayerManager.AddBridgeToPlayer(PlayerManager.player2, 1);
				}
				,
				_ => () => Debug.Log($"Switch player turn did not find proper value")
			};
			a.Invoke();

				Debug.Log($"Plyayer ({PlayerManager.player1.Id} - score - {PlayerManager.player1.BridgesCount})");
			Debug.Log($"Plyayer ({PlayerManager.player2.Id} - score - {PlayerManager.player2.BridgesCount})");
			//GlobalVars.score += 1;
		}
		else if (collision.CompareTag("Bridge"))
		{
			//Debug.Log("Can not place bidge");
			_canPlaceBridge = false;

			if (!collision.GetComponent<BridgeScript>().currentPlayer.IsMyTurn)
			{
				Action a = PlayerManager.playerTurn switch
				{
					PlayerTurn.P1_Turn => () => PlayerManager.AddBridgeToPlayer(PlayerManager.player1, 5),
					PlayerTurn.P2_Turn => () => PlayerManager.AddBridgeToPlayer(PlayerManager.player2, 5),
					_ => () => Debug.Log($"Switch player turn did not find proper value")
				};
				a.Invoke();

				Debug.Log($"Plyayer ({PlayerManager.player1.Id} - score - {PlayerManager.player1.BridgesCount})");
				Debug.Log($"Plyayer ({PlayerManager.player2.Id} - score - {PlayerManager.player2.BridgesCount})");
			}


		}

		else if (collision.CompareTag("Number"))
		{
			//Debug.Log("Can not place bidge");
			_canPlaceBridge = false;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Bridge") || collision.CompareTag("Number"))
		{
			//Debug.Log("Can place bidge");
			_canPlaceBridge = true;
		}
	}

	private void ChangeSpriteOfBridge(GameObject bridge, Sprite sprite)
	{
		bridge.GetComponent<SpriteRenderer>().sprite = sprite;

		// Change sprite in Multiplayer
		if(GameManager.gameMode == GameMode.Multiplayer)
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

	public void DestroyAllBridges()
	{
		foreach (var bridge in _bridgesToDelete)
		{
			Destroy(bridge);
		}
	}

}
