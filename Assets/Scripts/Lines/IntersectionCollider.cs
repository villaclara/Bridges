using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// The small object on top of the drawing Line, responsible for detecting intersection of current line with another GO.
/// </summary>
public class IntersectionCollider : MonoBehaviour
{
	[SerializeField] private PlayerManager _playerManager;
	[SerializeField] private Sprite _p1Sprite;
	[SerializeField] private Sprite _p2Sprite;

	public GameObject Bridge;
	public DrawMessenger drawMessenger;

	private bool _canPlaceBridge = true;
	public static float Radius;

	private void Awake()
	{
		Radius = GetComponent<CircleCollider2D>().radius;
		StepTimerScript.StepTimerFinished += StepTimerScript_StepTimerFinished;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_canPlaceBridge && collision.CompareTag("Line"))
		{
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
				},
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
				},
				_ => () => Debug.Log($"Switch player turn did not find proper value")
			};
			a.Invoke();
		}
		else if (collision.CompareTag("Bridge"))
		{
			_canPlaceBridge = false;
			if (!collision.GetComponent<BridgeScript>().currentPlayer.IsMyTurn)
			{
				Action a = PlayerManager.playerTurn switch
				{
					PlayerTurn.P1_Turn => () => AddBridgeToPlayer(PlayerManager.player1, 5),
					PlayerTurn.P2_Turn => () => AddBridgeToPlayer(PlayerManager.player2, 5),
					_ => () => Debug.Log($"Switch player turn did not find proper value")
				};
				a.Invoke();
			}
		}
		else if (collision.CompareTag("Number"))
		{
			_canPlaceBridge = false;
			return;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Bridge") || collision.CompareTag("Number"))
		{
			_canPlaceBridge = true;
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

	/// <summary>
	/// Adds 5 bridges to the current player if the timer has finished.
	/// </summary>
	private void StepTimerScript_StepTimerFinished()
	{
		if(GameManager.GameMode == GameMode.Local)
		{
			AddBridgeToPlayer(PlayerManager.playerTurn == PlayerTurn.P1_Turn ? PlayerManager.player1 : PlayerManager.player2, 5);
			return;
		}

		// Deciding who should call method
		if (PlayerManager.playerTurn == PlayerTurn.P1_Turn && NetworkManager.Singleton.IsHost)
		{
			// Is P1 and Host
			AddBridgeToPlayer(PlayerManager.player1, 5);
		}
		else if (PlayerManager.playerTurn == PlayerTurn.P2_Turn && !NetworkManager.Singleton.IsHost)
		{
			// Is used just to be sure that when on last turn the P2 does not move line to number in time he has correct count of bridges.
			// It has impact only on p2 local side when comparing P1.Bridges >< P2.Bridges in the Game End Screen.
			// On Host the p2 bridges are addes with Rpc request and then Host updates the value for both players using NetworkVariable.
			PlayerManager.player2.BridgesCount += 5;
			// Is P2 and Client
			AddBridgeToPlayer(PlayerManager.player2, 5);
		}
	}

	public void DestroyAllBridges()
	{
		foreach (var bridge in GlobalVars.bridgesToDelete)
		{
			Destroy(bridge);
		}
	}
}
