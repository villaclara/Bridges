using TMPro;
using UnityEngine;

/// <summary>
/// Responsible for changing the Score text of players.
/// </summary>
public class ScoreManager : MonoBehaviour
{
	public static ScoreManager Instance { get; private set; }

	[SerializeField]
	private TextMeshProUGUI _p1ScoreText;

	[SerializeField]
	private TextMeshProUGUI _p2ScoreText;

	private int _prevP1Score = 0;
	private int _prevP2Score = 0;

	private void Awake()
	{
		// singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		// TOOD - Check if we need this in multiplayer. As in MP we already subscribe in MP_PlayerDrawing
		PlayerManager.OnPlayerBridgesChanged += RedrawText;
	}

	private void RedrawText()
	{
		_p1ScoreText.text = $"P1 - {PlayerManager.player1.BridgesCount}";
		//_p1ScoreText.gameObject.transform.parent.GetComponent<Animator>().Play("PlayerBridgesAnimation");
		_p2ScoreText.text = $"P2 - {PlayerManager.player2.BridgesCount}";

		if(PlayerManager.player1.BridgesCount != _prevP1Score)
		{
			Debug.Log($"P1 score not the same in REDRAW");
			_p1ScoreText.gameObject.transform.parent.GetComponent<Animator>().Play("PlayerBridgesAnimation", -1, 0f);
			_prevP1Score = PlayerManager.player1.BridgesCount;
		}

		if (PlayerManager.player2.BridgesCount != _prevP2Score)
		{
			Debug.Log($"P2 score not the same in REDRAW");
			_p2ScoreText.gameObject.transform.parent.GetComponent<Animator>().Play("PlayerBridgesAnimation", -1, 0f);
			_prevP2Score = PlayerManager.player2.BridgesCount;
		}
	}
}
