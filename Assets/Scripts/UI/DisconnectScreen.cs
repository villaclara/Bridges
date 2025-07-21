using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectScreen : MonoBehaviour
{
	[SerializeField]
	private Button _returnButton;

	[SerializeField]
	private GameObject _loadingScreen;
	[SerializeField]
	private GameObject _mpLoadingScreen;
	[SerializeField]
	private GameObject _endGameScreen;


	private void Start()
	{
		_returnButton.onClick.AddListener(ReturnButtonClick);
	}

	private void ReturnButtonClick()
	{
		NetworkManager.Singleton.Shutdown();
		_loadingScreen.SetActive(true);
		_mpLoadingScreen.SetActive(false);
		_endGameScreen.SetActive(false);
		this.gameObject.SetActive(false);
	}
}
