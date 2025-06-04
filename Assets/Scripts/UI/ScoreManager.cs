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

	private void RedrawText()
	{
		_p1ScoreText.text = $"P1 - {PlayerManager.player1.BridgesCount}";
		_p2ScoreText.text = $"P2 - {PlayerManager.player2.BridgesCount}";
	}
}
