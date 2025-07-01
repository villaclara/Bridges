using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class for updating the ConnectionStatus Text of Multiplayer.
/// </summary>
public class ConnectionStatus : NetworkBehaviour
{
	[SerializeField] private TextMeshProUGUI connectionStatusText;
	[SerializeField] private TextMeshProUGUI _restartPlayersCountTMP;

	public GameObject gameManager;

	// NetworkVariable to sync the status message across clients
	private NetworkVariable<FixedString128Bytes> statusMessage = new NetworkVariable<FixedString128Bytes>(
		"Waiting for connection...");

	// NetworkVariable to sync the count of players wanting to restart game in MP EndGameScreen
	private NetworkVariable<int> _restartPlayersCount = new();

	private void Start()
	{ 
		statusMessage.OnValueChanged += OnStatusChanged;
		_restartPlayersCount.OnValueChanged += OnRestartPlayersCountChanged;
		gameManager.GetComponent<GameManager>().MP_RestartAsked += ConnectionStatus_MP_RestartClick;
	}

	/// <summary>
	/// Handles the Restart Button click on both clients.
	/// Sets the network variable <see cref="_restartPlayersCount"/> to the value of wanting players to restart.
	/// </summary>
	private void ConnectionStatus_MP_RestartClick()
	{
		// Client is not allowed to change network variables directly.
		if (!IsHost)
		{
			RequestRestartGameFromClientPressRpc();
		}
		else
		{
			// Logic is:
			// if only 1 player pressed then we set value to 1
			// on another client the value is now 1 and if he presses restart then we set to 2.
			if (_restartPlayersCount.Value == 0)
			{
				_restartPlayersCount.Value = 1;
			}
			else if (_restartPlayersCount.Value == 1)
			{
				_restartPlayersCount.Value = 2;
			}
		}
	}

	/// <summary>
	/// Checks count of players wanting to restart and if so then restarts the game by calling another RPC <see cref="StartGameOnClientRpc"/>.
	/// </summary>
	/// <param name="previousValue"></param>
	/// <param name="newValue"></param>
	private void OnRestartPlayersCountChanged(int previousValue, int newValue)
	{
		if(newValue == 1)
		{
			_restartPlayersCountTMP.text = "Restart - 1/2";
		}
		if(newValue == 2)
		{
			StartCoroutine(nameof(Wait1Sec));
			// reset count for next endGameScreen.
			_restartPlayersCount.Value = 0;
			StartGameOnClientRpc();
		}
	}

	/// <summary>
	/// Simply calls the <see cref="ConnectionStatus_MP_RestartClick"/> to invoke again as this RPC is a call from Client.
	/// </summary>
	[Rpc(SendTo.Server)]
	private void RequestRestartGameFromClientPressRpc()
	{
		if(!IsServer)
		{
			return;
		}
		ConnectionStatus_MP_RestartClick();
	}

	public override void OnDestroy()
	{
		statusMessage.OnValueChanged -= OnStatusChanged;
		_restartPlayersCount.OnValueChanged -= OnRestartPlayersCountChanged;
		base.OnDestroy();
	}

	// Change the status
	private void OnStatusChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
	{
		Debug.Log("On sstatus changed");
		if (connectionStatusText != null)
			connectionStatusText.text = newValue.ToString();
	}

	public override void OnNetworkSpawn()
	{
		//When client connects, update message on server
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
		}
	}

	private void OnClientConnected(ulong clientId)
	{
		// Only Host/Server handles starting the game
		if (!IsHost && !IsServer)
			return;

		// Only update the status if a client has connected (and we're the server)
		if (NetworkManager.Singleton.ConnectedClients.Count >= 2) // Host + 1 client = 2 or more connected
		{
			// Update the NetworkVariable, which will synchronize to all clients
			statusMessage.Value = "Connected 2/2";

			// TODO - Changed Task.Delay To yield return -- Not sure if it works in WEbgl
			//await Task.Delay(1000);
			StartCoroutine(Wait1Sec());
			StartGameOnClientRpc();
		}
	}

	[Rpc(SendTo.ClientsAndHost)]
	private void StartGameOnClientRpc()
	{
		Debug.Log("Starting game in Client!");

		// Resets the 'Restart 1/2 text used in EndGameScreen
		_restartPlayersCountTMP.text = " ";

		//gameManager.SetActive(true);
		gameManager.GetComponent<GameManager>().StartGame(isOnline: true);
	}

	private IEnumerator Wait1Sec()
	{
		yield return new WaitForSeconds(2f);
	}
}
