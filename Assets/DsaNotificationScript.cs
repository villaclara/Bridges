using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class DsaNotificationScript : MonoBehaviour
{
	private const string LastReadKey = "DSA_LastRead"; // PlayerPrefs key

	[Header("UI References")]
	public GameObject notificationPanel;  // Assign a simple panel prefab
	public TextMeshProUGUI notificationText;         // Text component inside panel
	public Button nextButton;             // "Next" button
	public Button closeButton;            // "Close" button

	private readonly Queue<Notification> _queue = new();

	private async void Start()
	{
		await InitializeAndCheckNotifications();
	}

	private async Task InitializeAndCheckNotifications()
	{
		try
		{
			if (UnityServices.State == ServicesInitializationState.Uninitialized)
				await UnityServices.InitializeAsync();

			// Anonymous sign-in (required for Relay)
			if (!AuthenticationService.Instance.IsSignedIn)
				await AuthenticationService.Instance.SignInAnonymouslyAsync();

			// Compare with last read
			string lastNotificationDate = AuthenticationService.Instance.LastNotificationDate;
			long lastRead = PlayerPrefs.GetInt(LastReadKey, 0);

			if (!string.IsNullOrEmpty(lastNotificationDate))
			{
				long lastCreated = long.Parse(lastNotificationDate);
				if (lastCreated > lastRead)
				{
					var notifications = await AuthenticationService.Instance.GetNotificationsAsync();
					EnqueueNotifications(notifications);
				}
			}
		}
		catch (AuthenticationException e)
		{
			// If sign-in fails due to restrictions, notifications may be inside the exception
			if (e.Notifications != null)
				EnqueueNotifications(e.Notifications);
		}
		catch (Exception e)
		{
			Debug.LogException(e);
		}
	}

	private void EnqueueNotifications(List<Notification> notifications)
	{
		foreach (var n in notifications)
			_queue.Enqueue(n);

		if (_queue.Count > 0)
			ShowNext();
	}

	private void ShowNext()
	{
		if (_queue.Count == 0)
		{
			notificationPanel.SetActive(false);
			return;
		}

		var n = _queue.Dequeue();
		notificationText.text = n.Message;

		// Save last read timestamp
		long createdAt = long.Parse(n.CreatedAt);
		long lastRead = PlayerPrefs.GetInt(LastReadKey, 0);
		if (createdAt > lastRead)
		{
			PlayerPrefs.SetInt(LastReadKey, (int)createdAt);
			PlayerPrefs.Save(); // important for WebGL
		}

		notificationPanel.SetActive(true);

		nextButton.onClick.RemoveAllListeners();
		nextButton.onClick.AddListener(ShowNext);

		closeButton.onClick.RemoveAllListeners();
		closeButton.onClick.AddListener(() => notificationPanel.SetActive(false));
	}
}
