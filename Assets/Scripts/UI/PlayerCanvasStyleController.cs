using System;
using UnityEngine;

/// <summary>
/// Responsible for changing the Border color of active player.
/// </summary>
public class PlayerCanvasStyleController : MonoBehaviour
{
	private GameObject _obj;

	[SerializeField]
	private int _playerId;

	private void Awake()
	{
		_obj = transform.GetChild(0).gameObject;
		_obj.SetActive(false);
		PlayerManager.OnPlayerTurnSwitch += PlayerManager_OnPlayerTurnSwitch;
	}

	private void PlayerManager_OnPlayerTurnSwitch()
	{
		Debug.Log($"{nameof(PlayerManager.OnPlayerTurnSwitch)} called in code, turn - {PlayerManager.playerTurn}");
		Action a = PlayerManager.playerTurn switch
		{
			PlayerTurn.P1_Turn => ChangeOutline_forP1,
			PlayerTurn.P2_Turn => ChangeOutline_forP2,
			_ => () => { Debug.LogWarning($"{nameof(PlayerManager_OnPlayerTurnSwitch)} - pattern matching switch not found"); }
		};
		a?.Invoke();
	}


	// Sets the thicker border if player 1 turn
	private void ChangeOutline_forP1() =>
		_obj.SetActive(_playerId == 1);

	// Sets the thicker border if player 2 turn
	private void ChangeOutline_forP2() =>
		_obj.SetActive(_playerId != 1);

	public void Reset()
	{
		_obj.SetActive(false);
	}
}
