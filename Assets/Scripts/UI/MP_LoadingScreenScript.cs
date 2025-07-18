using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

public class MP_LoadingScreenScript : MonoBehaviour
{
    [SerializeField] private Button _hostGameButton;
    [SerializeField] private Button _joinGameButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private GameObject _networkMessenger;
    private SetupNetwork _network;

    [SerializeField] private GameObject _spinner;
    [SerializeField] private TextMeshProUGUI _roomCodeTMP;
    [SerializeField] private Image _roomCodeImg;
    [SerializeField] private TextMeshProUGUI _connectionStatus;

	private void OnEnable()
	{
		_roomCodeImg.enabled = false;
		_roomCodeTMP.enabled = false;
		_connectionStatus.text = " ";
		_spinner.SetActive(false);

        _hostGameButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
        _hostGameButton.interactable = true;
        _hostGameButton.GetComponent<DefaultButton>().enabled = true;
        
        _joinGameButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
        _joinGameButton.interactable = true;
        _joinGameButton.GetComponent<DefaultButton>().enabled = true;
    }

	void Start()
    {
        _hostGameButton.onClick.AddListener(OnHostButtonClick);
        _joinGameButton.onClick.AddListener(OnJoinGameButtonClick);
		_exitButton.onClick.AddListener(OnExitButtonClick);
        _network = _networkMessenger.GetComponent<SetupNetwork>();

        _roomCodeImg.enabled = false;
        _roomCodeTMP.enabled = false;
        _connectionStatus.text = " ";
	}
        
	private void OnExitButtonClick()
	{
        Debug.Log("ON EXIT BUTTON CLICK");
        _network.Shutdown();
	}

	private async void OnHostButtonClick()
    {
		//      _hostGameButton.gameObject.GetComponent<Image>().color = GlobalVars.DARKER_GREEN_BUTTON_COLOR;
		//      _hostGameButton.interactable = false;
		//      _hostGameButton.GetComponent<DefaultButton>().enabled = false;

		//      _joinGameButton.gameObject.GetComponent<Image>().color = Color.grey;
		//_joinGameButton.interactable = false;
		//      _joinGameButton.GetComponent<DefaultButton>().enabled = false;

		_connectionStatus.text = string.Empty;
		_hostGameButton.gameObject.GetComponent<DefaultButton>().SetPressed();
		_joinGameButton.gameObject.GetComponent<DefaultButton>().SetDisabled();

		_spinner.SetActive(true);
        var isStarted = await _network.StartHostRelay();
        _spinner.SetActive(false);

        if(isStarted)
        {
            _roomCodeImg.enabled = true;
            _roomCodeTMP.enabled = true;

			_connectionStatus.text = "Connected 1/2.";

		}
		else
		{
			_roomCodeImg.enabled = false;
			_roomCodeTMP.enabled = false;

			_hostGameButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
			_hostGameButton.interactable = true;
			_hostGameButton.GetComponent<DefaultButton>().enabled = true;

			_joinGameButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
			_joinGameButton.interactable = true;
			_joinGameButton.GetComponent<DefaultButton>().enabled = true;
		}
	}

	private async void OnJoinGameButtonClick()
    {
        _connectionStatus.text = string.Empty;
        _joinGameButton.gameObject.GetComponent<DefaultButton>().SetPressed();
        _hostGameButton.gameObject.GetComponent<DefaultButton>().SetDisabled();

		_spinner.SetActive(true);
        var isConnected = await _network.StartClientRelay();

        if(isConnected)
        {

        }
        else
        {
			_spinner.SetActive(false);

			_roomCodeImg.enabled = false;
			_roomCodeTMP.enabled = false;

			_hostGameButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
			_hostGameButton.interactable = true;
			_hostGameButton.GetComponent<DefaultButton>().enabled = true;

			_joinGameButton.GetComponent<Image>().color = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
			_joinGameButton.interactable = true;
			_joinGameButton.GetComponent<DefaultButton>().enabled = true;

		}
	}
}
