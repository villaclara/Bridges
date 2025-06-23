using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MP_Line : NetworkBehaviour
{
	//[SerializeField] private GameObject _drawManager;
	[SerializeField] private Line _line;

	private NetworkVariable<Vector2> point = new();

	private void Start()
	{
		point.OnValueChanged += OnValueChanged;
		//_drawManager.GetComponent<DrawManager>().OnNewPosAddedToLine += OnNewPosAddedToLine;
	}

	private void OnValueChanged(Vector2 previousValue, Vector2 newValue)
	{
		//Debug.Log($"OnLineValueChanged - {newValue}, IsOwner - {IsOwner}");
		if(!IsOwner)
		{
			_line.SetPosition(point.Value);
		}
	}
	
	/// <summary>
	/// Adds new point to Line Network Object (to represent on Client).
	/// </summary>
	/// <param name="point"></param>
	public void SetNewValueToPoint(Vector2 point)
	{
		Debug.Log($"Set New value to point called");
		this.point.Value = point;
	}

	/// <summary>
	/// Sets the Line color of P1 (Purple) on the Client side using Network Object line with Server.
	/// </summary>
	[Rpc(SendTo.ClientsAndHost)]
	public void RequestLineColorChangeOnClientRpc()
	{
		if (IsServer)
		{
			return;
		}
		GetComponent<Line>().SetLineColor(PlayerManager.player1.ColorHEX);
	}

}
