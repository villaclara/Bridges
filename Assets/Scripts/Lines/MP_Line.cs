using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Multiplayer features for Line, essentially doing the same as <see cref="Line"/>.
/// </summary>
public class MP_Line : NetworkBehaviour
{
	[SerializeField] private Line _line;

	private NetworkVariable<Vector2> point = new();

	private void Start()
	{
		point.OnValueChanged += OnValueChanged;
	}

	private void OnValueChanged(Vector2 previousValue, Vector2 newValue)
	{
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
