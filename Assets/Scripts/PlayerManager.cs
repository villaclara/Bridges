using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public readonly static PlayerModel player1 = new(1, "#b58950");

	public readonly static PlayerModel player2 = new(2, "#6c94d4");

	public static PlayerTurn playerTurn;

	public void SetupFirstTurn(bool isPlayer1First)
	{
		if (isPlayer1First)
		{
			player1.IsMyTurn = isPlayer1First;
			player2.IsMyTurn = !isPlayer1First;
			playerTurn = PlayerTurn.P1_Turn;
		}
		else
		{
			player1.IsMyTurn = !isPlayer1First;
			player2.IsMyTurn = isPlayer1First;
			playerTurn = PlayerTurn.P2_Turn;
		}
	}

	public void SwitchTurns()
	{
		player1.IsMyTurn = !player1.IsMyTurn;
		player2.IsMyTurn = !player2.IsMyTurn;
	}
}
