using System;
using UnityEngine;

/// <summary>
/// Manager class for players instances with logic to add bridges, turn management.
/// Singleton.
/// </summary>
public class PlayerManager : MonoBehaviour
{
	public readonly static IPlayerModel player1 = new PlayerModel(1, GlobalVars.DEFAULT_P1_COLOR_HEX);

	public readonly static IPlayerModel player2 = new PlayerModel(2, GlobalVars.DEFAULT_P2_COLOR_HEX);

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

	/// <summary>
	/// Sets the first turn.
	/// </summary>
	/// <param name="isPlayer1First">If the p1 turns first. Is set randomly.</param>
	public static void SetupFirstTurn(bool isPlayer1First)
	{
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
		OnPlayerTurnSwitch?.Invoke();
	}

	/// <summary>
	/// Switch turns and invoke an Event.
	/// </summary>
	public static void SwitchTurns()
	{
		player1.IsMyTurn = !player1.IsMyTurn;
		player2.IsMyTurn = !player2.IsMyTurn;
		playerTurn = player1.IsMyTurn ? PlayerTurn.P1_Turn : PlayerTurn.P2_Turn;
		OnPlayerTurnSwitch?.Invoke();
	}

	/// <summary>
	/// Adds bridge to the selected player.
	/// </summary>
	/// <param name="player">Player to add bridge.</param>
	/// <param name="count">Count of bridges to add.</param>
	public static void AddBridgeToPlayer(IPlayerModel player, int count = 1)
	{
		player.BridgesCount += count;
		OnPlayerBridgesChanged?.Invoke();
	}

	/// <summary>
	/// Resets the bridges values for the players and invoke an Event.
	/// </summary>
	public static void ResetBridgesCountForPlayers()
	{
		player1.BridgesCount = 0;
		player2.BridgesCount = 0;
		OnPlayerBridgesChanged?.Invoke();
	}
}
