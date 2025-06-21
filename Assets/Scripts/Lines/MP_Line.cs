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

	private void OnNewPosAddedToLine(Vector2 obj)
	{
		point.Value = obj;
	}

	private void OnValueChanged(Vector2 previousValue, Vector2 newValue)
	{
		//Debug.Log($"OnLineValueChanged - {newValue}, IsOwner - {IsOwner}");
		if(!IsOwner)
		{
			_line.SetPosition(point.Value);
		}
	}
	
	public void SetNewValueToPoint(Vector2 point)
	{
		Debug.Log($"Set New value to point called");
		this.point.Value = point;
	}
	
}
