using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using TMPro;
using System;
using System.Text.RegularExpressions;
using Unity.Services.Relay.Models;

public class SetupNetwork : MonoBehaviour
{

	[SerializeField] private TextMeshProUGUI createdJoinCode;
	[SerializeField] private TextMeshProUGUI clientJoinCode;
	[SerializeField] private TextMeshProUGUI connectionStatusText;
	[SerializeField] private GameObject _spinner;

	private Allocation _hostRelayAllocation;

	
	/// <summary>
	/// Gets the boolean value when the user disconnects if it is the CURRENT user who disconnected.
	/// Is used in <see cref="ConnectionStatus.OnClientDisconnectCallback(ulong)"/> to determine whether to show <see cref="DisconnectScreen"/>.
	/// </summary>
	public static bool IsSentShutdownFromMe { get; private set; } = false;

	/// <summary>
	/// Start Host with Relay.
	/// Need this function as Button clicks only support VOID return type.
	/// </summary>
	public async Task<bool> StartHostRelay()
	{
		IsSentShutdownFromMe = false;
		var createdCode = await StartHostWithRelayAsync(2, "wss");
		return createdCode != null;
	}
	private async Task<string> StartHostWithRelayAsync(int maxConnections, string connectionType)
	{
		await UnityServices.InitializeAsync();
		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		try
		{

			_hostRelayAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(_hostRelayAllocation, connectionType));
			NetworkManager.Singleton.GetComponent<UnityTransport>().UseWebSockets = true;
			var joinCode = await RelayService.Instance.GetJoinCodeAsync(_hostRelayAllocation.AllocationId);
			Debug.Log($"joincode - " + joinCode);
			createdJoinCode.text = "Room Code - " + joinCode;
			//connectionStatusText.text = "Waiting for players...";
			_spinner.SetActive(false);
			return NetworkManager.Singleton.StartHost() ? joinCode : null;
		}
		catch
		{
			connectionStatusText.text = "Error. Try creating room again.";
			_spinner.SetActive(false);
			return null;
		}
	}


	/// <summary>
	/// Start Client with Relay. 
	/// </summary>
	/// <param name="joinCode">Join Code for session.</param>
	public async Task<bool> StartClientRelay()
	{
		IsSentShutdownFromMe = false;
		string joincode = Regex.Replace(clientJoinCode.text, @"[\u200B-\u200D\uFEFF]", "").Trim();
		if (string.IsNullOrEmpty(joincode))
		{
			return false;
		}
		connectionStatusText.text = "Joining...";
		var result = await JoinClientAsync(joincode);
		return result;
	}
	// one more function because we need to await the results
	private async Task<bool> JoinClientAsync(string joincode)
	{
		try
		{
			var connectResult = await StartClientWithRelay(joincode, "wss");
			return connectResult;
		}
		catch (Exception ex)
		{
			Debug.Log($"Exception when joining - {ex.Message}");
			connectionStatusText.text = "Error when joining.";
			return false;
		}

	}
	private async Task<bool> StartClientWithRelay(string joinCode, string connectionType)
	{
		await UnityServices.InitializeAsync();
		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, connectionType));
		NetworkManager.Singleton.GetComponent<UnityTransport>().UseWebSockets = true;
		return NetworkManager.Singleton.StartClient();
	}

	/// <summary>
	/// Disconnects from the game and calls <see cref="NetworkManager.Shutdown(bool)"/> method.
	/// </summary>
	public void Shutdown()
	{
		if (GameManager.GameMode == GameMode.Local)
		{
			return;
		}

		IsSentShutdownFromMe = true;
		NetworkManager.Singleton.Shutdown();
	}
}
