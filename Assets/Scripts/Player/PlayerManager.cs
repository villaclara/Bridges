using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public readonly static IPlayerModel player1 = new PlayerModel(1, "#A259FF");

	public readonly static IPlayerModel player2 = new PlayerModel(2, "#00BFA6");

	public static PlayerTurn playerTurn;

	public static PlayerManager Instance { get; private set; }

	public static event Action OnPlayerTurnSwitch;
	public static event Action OnPlayerBridgesChanged;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static void SetupFirstTurn(bool isPlayer1First)
	{
		// TODO - Remove fake setup first turn
		isPlayer1First = true;
		if (isPlayer1First)
		{
			player1.IsMyTurn = isPlayer1First;
			player2.IsMyTurn = !isPlayer1First;
			playerTurn = PlayerTurn.P1_Turn;
		}
		else
		{
			player1.IsMyTurn = isPlayer1First;
			player2.IsMyTurn = !isPlayer1First;
			playerTurn = PlayerTurn.P2_Turn;
		}
		Debug.Log($"count of subs of playermanager - {OnPlayerTurnSwitch?.GetInvocationList().Length ?? 0}");
		OnPlayerTurnSwitch?.Invoke();
	}

	public static void SwitchTurns()
	{
		player1.IsMyTurn = !player1.IsMyTurn;
		player2.IsMyTurn = !player2.IsMyTurn;
		playerTurn = player1.IsMyTurn ? PlayerTurn.P1_Turn : PlayerTurn.P2_Turn;
		OnPlayerTurnSwitch?.Invoke();
	}

	public static void AddBridgeToPlayer(IPlayerModel player, int count = 1)
	{
		Debug.Log($"Player - {player.Id}, count to add - {count}");
		player.BridgesCount += count;
		OnPlayerBridgesChanged?.Invoke();
	}

	public static void ResetBridgesCountForPlayers()
	{
		player1.BridgesCount = 0;
		player2.BridgesCount = 0;
		OnPlayerBridgesChanged?.Invoke();
	}
}
