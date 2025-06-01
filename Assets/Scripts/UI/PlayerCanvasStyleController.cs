using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for changing the Border color of active player.
/// </summary>
public class PlayerCanvasStyleController : MonoBehaviour
{
	private Outline _borderOutline;

	[SerializeField]
	private int _playerId;

	private void Awake()
	{
		_borderOutline = GetComponentInChildren<Outline>();
		_borderOutline.effectColor = _playerId == 1 ? new Color32(139, 78, 217, 255) : new Color32(12, 138, 122, 255);

		PlayerManager.OnPlayerTurnSwitch += PlayerManager_OnPlayerTurnSwitch;

	}

	private void PlayerManager_OnPlayerTurnSwitch()
	{
		Action a = PlayerManager.playerTurn switch
		{
			PlayerTurn.P1_Turn => ChangeOutline_forP1,
			PlayerTurn.P2_Turn => ChangeOutline_forP2,
			_ => () => { Debug.LogWarning($"{nameof(PlayerManager_OnPlayerTurnSwitch)} - pattern matching switch not found"); }
		};
		a?.Invoke();
	}


	// Changes the outline color if P1 turn
	// So green for P1, white for P2
	private void ChangeOutline_forP1() =>
		_borderOutline.effectColor = _playerId == 1 ? new Color32(139, 78, 217, 255) : new Color32(255, 255, 255, 0);

	// Changes the outline color if P2 turn
	// So geen for P2, white for P1
	private void ChangeOutline_forP2() =>
		_borderOutline.effectColor = _playerId == 1 ? new Color32(255, 255, 255, 0) : new Color32(12, 138, 122, 255);

	public void Reset()
	{
		_borderOutline = GetComponentInChildren<Outline>();
		_borderOutline.effectColor = Color.white;
	}

}
