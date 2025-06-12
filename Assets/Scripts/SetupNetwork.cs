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

public class SetupNetwork : MonoBehaviour
{

	public TextMeshProUGUI createdJoinCode;
	public TextMeshProUGUI clientJoinCode;
	public TextMeshProUGUI IsJoinedClient;




	/// <summary>
	/// Start Host with Relay.
	/// Need this function as Button clicks only support VOID return type.
	/// </summary>
	public void StartHostRelay()
	{
		_ = StartHostWithRelayAsync(2, "wss");
	}
	private async Task<string> StartHostWithRelayAsync(int maxConnections, string connectionType)
	{
		await UnityServices.InitializeAsync();
		if (!AuthenticationService.Instance.IsSignedIn)
		{
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, connectionType));
		NetworkManager.Singleton.GetComponent<UnityTransport>().UseWebSockets = true;
		var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
		Debug.Log($"joincode - " + joinCode);
		createdJoinCode.text = "Room Code - " + joinCode;
		return NetworkManager.Singleton.StartHost() ? joinCode : null;
	}


	/// <summary>
	/// Start Client with Relay. 
	/// Need this function as Button clicks only support VOID return type.
	/// </summary>
	/// <param name="joinCode">Join Code for session.</param>
	public void StartClientRelay()
	{
		string joincode = Regex.Replace(clientJoinCode.text, @"[\u200B-\u200D\uFEFF]", "").Trim();
		if (string.IsNullOrEmpty(joincode))
		{
			Debug.Log($"Client join empty - {joincode}");
		}
		_ = JoinClientAsync(joincode);
	}
	// one more function because we need to await the results
	private async Task JoinClientAsync(string joincode)
	{
		try
		{
			bool result = await StartClientWithRelay(joincode, "wss");
			IsJoinedClient.text = result ? "Client Joined" : "Error when joining";
		}
		catch (Exception ex)
		{
			Debug.Log($"Exception when joining - {ex.Message}");
			IsJoinedClient.text = "Catch error when joining. " + ex.Message;
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
		return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
	}

}
