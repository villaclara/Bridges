using System;
using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
	[SerializeField]
	public GameObject Bridge;

	[SerializeField]
	private PlayerManager _playerManager;

	private bool _canPlaceBridge = true;
	public static float Radius;

	private void Awake()
	{
		Radius = GetComponent<CircleCollider2D>().radius;
	}

	// TODO - Count other bridges, do not count self bridge
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_canPlaceBridge && collision.CompareTag("Line"))
		{
			Debug.Log("Line collision, can place bridge");
			var bridge = Instantiate(Bridge, transform.position, Quaternion.identity);
			bridge.GetComponent<SpriteRenderer>().color = PlayerManager.playerTurn switch
			{
				PlayerTurn.P1_Turn => Color.green,
				PlayerTurn.P2_Turn => Color.blue,
				_ => Color.black
			};
			Action a = PlayerManager.playerTurn switch
			{
				PlayerTurn.P1_Turn => () => PlayerManager.player1.BridgesCount += 1,
				PlayerTurn.P2_Turn => () => PlayerManager.player2.BridgesCount += 1,
				_ => () => Debug.Log($"Switch player turn did not find proper value")
			};
			a.Invoke();

			Debug.Log($"Plyayer ({PlayerManager.player1.Id} - score - {PlayerManager.player1.BridgesCount})");
			Debug.Log($"Plyayer ({PlayerManager.player2.Id} - score - {PlayerManager.player2.BridgesCount})");
			//GlobalVars.score += 1;
		}
		else if (collision.CompareTag("Bridge"))
		{
			Debug.Log("Can not place bidge");
			_canPlaceBridge = false;
		}

		else if (collision.CompareTag("Number"))
		{
			Debug.Log("Can not place bidge");
			_canPlaceBridge = false;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Bridge") || collision.CompareTag("Number"))
		{
			Debug.Log("Can place bidge");
			_canPlaceBridge = true;
		}
	}


}
