using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class showing the EndGame screen.
/// </summary>
public class EndGameScript : MonoBehaviour
{
    [SerializeField] private Button _restartButton;

	public event Action RestartButtonClick;

	private void Start()
	{
		_restartButton.onClick.AddListener(OnRestartButtonClick);
	}

	private void OnEnable()
	{
		_restartButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
		_restartButton.interactable = true;
		_restartButton.GetComponent<DefaultButton>().enabled = true;
	}

	/// <summary>
	/// Firing the event of 'REstart Button Clicked', the <see cref="GameManager"/> object is subscriber.
	/// </summary>
	private void OnRestartButtonClick()
	{
		_restartButton.GetComponent<DefaultButton>().SetPressed();
		RestartButtonClick?.Invoke();
	}
}
