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

	public GameObject LoadingScreen;
	public GameObject GameManager;

	// NetworkVariable to sync the status message across clients
	private NetworkVariable<FixedString128Bytes> statusMessage = new NetworkVariable<FixedString128Bytes>(
		"Waiting for connection...");

	private void Start()
	{ 
		statusMessage.OnValueChanged += OnStatusChanged;
	}

	public override void OnDestroy()
	{
		statusMessage.OnValueChanged -= OnStatusChanged;
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

	private async void OnClientConnected(ulong clientId)
	{
		// Only Host/Server handles starting the game
		if (!IsHost && !IsServer)
			return;

		// Only update the status if a client has connected (and we're the server)
		if (NetworkManager.Singleton.ConnectedClients.Count >= 2) // Host + 1 client = 2 or more connected
		{
			// Update the NetworkVariable, which will synchronize to all clients
			statusMessage.Value = "Connected 2/2";

			await Task.Delay(1000);
			StartGameOnClientRpc();
		}
	}

	[Rpc(SendTo.ClientsAndHost)]
	private void StartGameOnClientRpc()
	{
		Debug.Log("Starting game in Client!");

		// Optional: Start on host
		LoadingScreen.SetActive(false);
		GameManager.SetActive(true);
		GameManager.GetComponent<GameManager>().SetGameMode(isOnline: true);
	}
}
