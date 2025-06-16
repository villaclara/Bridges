using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MP_Line : NetworkBehaviour
{
	//[SerializeField] private GameObject _drawManager;
	[SerializeField] private Line _line;

	NetworkVariable<Vector2> points = new();

	private void Start()
	{
		points.OnValueChanged += OnValueChanged;
		//_drawManager.GetComponent<DrawManager>().OnNewPosAddedToLine += OnNewPosAddedToLine;
	}

	private void OnNewPosAddedToLine(Vector2 obj)
	{
		points.Value = obj;
	}

	private void OnValueChanged(Vector2 previousValue, Vector2 newValue)
	{
		Debug.Log($"OnLineValueChanged - {newValue}, IsOwner - {IsOwner}");
		if(!IsOwner)
		{
			_line.SetPosition(points.Value);
		}
	}
	
	public void SetNewValueToPoint(Vector2 point)
	{
		Debug.Log($"Set New value to point called");
		points.Value = point;
	}
	
}
