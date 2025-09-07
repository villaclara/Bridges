using TMPro;
using UnityEngine;

/// <summary>
/// Responsible for changing the Score text of players.
/// </summary>
public class ScoreManager : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _p1ScoreText;
	[SerializeField] private TextMeshProUGUI _p2ScoreText;
	
	private int _prevP1Score = 0;
	private int _prevP2Score = 0;
	
	public static ScoreManager Instance { get; private set; }

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
		PlayerManager.OnPlayerBridgesChanged += RedrawText;
	}

	/// <summary>
	/// Updates text values with Score.
	/// </summary>
	private void RedrawText()
	{
		_p1ScoreText.text = $"P1 - {PlayerManager.player1.BridgesCount}";
		_p2ScoreText.text = $"P2 - {PlayerManager.player2.BridgesCount}";

		if(PlayerManager.player1.BridgesCount != _prevP1Score)
		{
			_p1ScoreText.gameObject.transform.parent.GetComponent<Animation>().Play("PlayerBridgesAnimation");
			_prevP1Score = PlayerManager.player1.BridgesCount;
		}

		if (PlayerManager.player2.BridgesCount != _prevP2Score)
		{
			_p2ScoreText.gameObject.transform.parent.GetComponent<Animation>().Play("PlayerBridgesAnimation");
			_prevP2Score = PlayerManager.player2.BridgesCount;
		}
	}
}
