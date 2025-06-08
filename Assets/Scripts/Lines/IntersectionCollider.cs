using System;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
	[SerializeField]
	public GameObject Bridge;

	[SerializeField]
	private PlayerManager _playerManager;

	[SerializeField]
	private Sprite p1Sprite;
	[SerializeField]
	private Sprite p2Sprite;

	private bool _canPlaceBridge = true;
	public static float Radius;


	private readonly List<GameObject> _bridgesToDelete = new();

	private void Awake()
	{
		Radius = GetComponent<CircleCollider2D>().radius;
	}

	// TODO - Count other bridges, do not count self bridge
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_canPlaceBridge && collision.CompareTag("Line"))
		{
			//Debug.Log("Line collision, can place bridge");
			var bridge = Instantiate(Bridge, new Vector3(transform.position.x, transform.position.y, -5), Quaternion.identity);
			_bridgesToDelete.Add(bridge);
			Action a = PlayerManager.playerTurn switch
			{
				PlayerTurn.P1_Turn => () =>
				{
					bridge.GetComponent<BridgeScript>().currentPlayer = PlayerManager.player1;
					//SetBridgeColor(bridge, PlayerManager.player1.ColorHEX);
					ChangeSpriteOfBridge(bridge, p1Sprite);
					PlayerManager.AddBridgeToPlayer(PlayerManager.player1, 1);
				}
				,
				PlayerTurn.P2_Turn => () =>
				{
					bridge.GetComponent<BridgeScript>().currentPlayer = PlayerManager.player2;
					//SetBridgeColor(bridge, PlayerManager.player2.ColorHEX);
					ChangeSpriteOfBridge(bridge, p2Sprite);
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
					PlayerTurn.P1_Turn => () =>
					{
						PlayerManager.AddBridgeToPlayer(PlayerManager.player1, 5);
					}
					,
					PlayerTurn.P2_Turn => () =>
					{
						PlayerManager.AddBridgeToPlayer(PlayerManager.player2, 5);
					}
					,
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

	private void SetBridgeColor(GameObject bridge, string colorHEX)
	{
		if (ColorUtility.TryParseHtmlString(colorHEX, out Color newcolor))
		{
			bridge.GetComponent<SpriteRenderer>().color = newcolor;
			Debug.Log($"set bridge color - {newcolor}");
		}
	}

	private void ChangeSpriteOfBridge(GameObject bridge, Sprite sprite)
	{
		bridge.GetComponent<SpriteRenderer>().sprite = sprite;
	}

	public void DestroyAllBridges()
	{
		foreach (var bridge in _bridgesToDelete)
		{
			Destroy(bridge);
		}
	}

}
