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

	private void Start()
	{
		_returnButton.onClick.AddListener(ReturnButtonClick);
	}

	private void ReturnButtonClick()
	{
		NetworkManager.Singleton.Shutdown();
		_loadingScreen.SetActive(true);
		this.gameObject.SetActive(false);
	}
}
