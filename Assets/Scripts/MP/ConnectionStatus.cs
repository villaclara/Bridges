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
	public DisconnectScreen disconnectScreen;

	// NetworkVariable to sync the status message across clients
	private NetworkVariable<FixedString128Bytes> statusMessage = new NetworkVariable<FixedString128Bytes>(
		"Waiting for connection...");

	// NetworkVariable to sync the count of players wanting to restart game in MP EndGameScreen
	private NetworkVariable<int> _restartPlayersCount = new();

	/// <inheritdoc/>
	public override void OnNetworkSpawn()
	{
		statusMessage.OnValueChanged += OnConnectionsTextStatusChanged;
		_restartPlayersCount.OnValueChanged += OnRestartPlayersCountChanged;
		gameManager.GetComponent<GameManager>().MP_RestartAsked += ConnectionStatus_MP_RestartClick;

		//When client connects, update message on server
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
		}

		NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
	}

	/// <inheritdoc/>
	public override void OnNetworkDespawn()
	{
		statusMessage.OnValueChanged -= OnConnectionsTextStatusChanged;
		_restartPlayersCount.OnValueChanged -= OnRestartPlayersCountChanged;
		gameManager.GetComponent<GameManager>().MP_RestartAsked -= ConnectionStatus_MP_RestartClick;
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
		}
		NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;

		base.OnNetworkDespawn();
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
			_restartPlayersCountTMP.text = "Restart - 2/2";
			StartCoroutine(StartGameAfterDelay());
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

	/// <summary>
	/// Updates the <see cref="connectionStatusText"/> TMP ('Connected 1/2') in Multiplayer for both players.
	/// </summary>
	private void OnConnectionsTextStatusChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
	{
		if (connectionStatusText != null)
		{
			connectionStatusText.text = newValue.ToString();
		}
	}

	/// <summary>
	/// When both Client / Host receives <see cref="NetworkManager.OnClientDisconnectCallback"/> event for other player disconnected.
	/// </summary>
	/// <param name="clientId">Id of client who disconnected. If the Host disconnects then he will receive the id of Client (not host), because the Client actually is disconnected from lobby. </param>
	private void OnClientDisconnectCallback(ulong clientId)
	{
		Debug.Log($"{nameof(OnClientDisconnectCallback)} received disconnect callback with id {clientId}");
		// HOST: Client has disconnected, so display only for host.
		if (NetworkManager.Singleton.IsHost && !SetupNetwork.IsSentShutdownFromMe)
		{
			disconnectScreen.gameObject.SetActive(true);
		}

		// CLIENT: host has disconnected, so display only for Client.
		if (!NetworkManager.Singleton.IsHost && !SetupNetwork.IsSentShutdownFromMe)
		{
			disconnectScreen.gameObject.SetActive(true);
		}
	}
	
	/// <summary>
	/// When the client has connected to a lobby. Only Host handles the connection flow.
	/// </summary>
	/// <param name="clientId">If of client who connected.</param>
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

			// Task.Delay() does not work properly in WebGl.
			//await Task.Delay(1000);
			StartCoroutine(StartGameAfterDelay());
		}
	}

	/// <summary>
	/// <see cref="RpcAttribute"/> method to start game via <see cref="GameManager.StartGame(bool)"/> on BOTH players.
	/// </summary>
	[Rpc(SendTo.ClientsAndHost)]
	private void StartGameOnClientRpc()
	{
		// Resets the 'Restart 1/2' text used in EndGameScreen and 'Connected 1/2' text.
		_restartPlayersCountTMP.text = " ";

		//gameManager.SetActive(true);
		gameManager.GetComponent<GameManager>().StartGame(isOnline: true);
	}

	/// <summary>
	/// Enumerator to wait 1 second. Also to reset network variables to defaults.
	/// </summary>
	private IEnumerator StartGameAfterDelay()
	{
		if(IsHost)
		{
			// Wait 1 sec only on Host. Client waits anyway for host to call the StartGameRpc.
			yield return new WaitForSecondsRealtime(1f);

			// Reset the network variables for next sequential game.
			_restartPlayersCount.Value = 0;
			statusMessage.Value = " ";

			StartGameOnClientRpc();
		}
	}
}
