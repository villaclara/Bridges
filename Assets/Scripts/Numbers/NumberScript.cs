using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NumberScript : NetworkBehaviour
{
	public static readonly Vector3 DefaultPosition = new(0f, 0f, -2f);   // if 0z - the black border is not visible

	#region private fields

	private Camera _cam;
	private bool _isDragging;
	private bool _isValidPositionToStopDrag = true; // by default we allow to stop
	private SpriteRenderer _circleRenderer;
	private bool _isEnabled = true;

	public int value;
	#endregion

	#region public fields

	public event Action<bool> OnIsAllowedToStopDragChanged;

	public bool IsValidPosToStopDrag
	{
		get => _isValidPositionToStopDrag;
		private set
		{
			_isValidPositionToStopDrag = value;
			OnIsAllowedToStopDragChanged?.Invoke(_isValidPositionToStopDrag);
		}
	}

	// invokes when the position is set. Is assigned in NumbersManager to create next number.
	public Action<NumberModel> OnDragEnded;

	#endregion

	// Start is called before the first frame update
	private void Start()
	{
		// If we are the client, not Host, we only watch the numbers to be drawn.
		if (GameManager.GameMode == GameMode.Multiplayer && !IsOwner && !IsHost)
		{
			Debug.Log($"NumberScript Start in MP in Client !Host");
			_circleRenderer = GetComponentInChildren<SpriteRenderer>();
			_circleRenderer.color = Color.white;
			SpinningCircleHelper.SetSpinningCircleForGO(gameObject, false, false);
			_isEnabled = false;
		}

		// if we ARE host OR the game is Local
		// We position numbers manually and subscribe to event to change color of number.
		else
		{
			Debug.Log($"NumberScript Start in Host or Local");
			_cam = Camera.main;
			_circleRenderer = GetComponentInChildren<SpriteRenderer>();
			_circleRenderer.color = Color.yellow;
			OnIsAllowedToStopDragChanged += SetColorIfAllowedToDrop;
		}
	}

	// Update is called once per frame
	private void Update()
	{
		Debug.Log($"IsOwner - {IsOwner}");
		// If we are client we listening to host moving the number and the Update is not even called in Client.
		if(GameManager.GameMode == GameMode.Multiplayer && !NetworkManager.Singleton.IsHost)
		{
			Debug.Log($"number position - {transform.position}, value - {value}");
		}
		if (!_isDragging)
		{
			Debug.Log($"IsDragging false - IsClient - {NetworkManager.Singleton.IsClient}, IsHost - {NetworkManager.Singleton.IsHost}");
			return;
		}

		// Check if mouse is not over the screen.
		// xz chu norm tyt.

		Vector3 mousePos1 = Input.mousePosition;
		var isMouseOverScreen = mousePos1.x >= 0 && mousePos1.x <= Screen.width &&
			   mousePos1.y >= 0 && mousePos1.y <= Screen.height;

		if (!isMouseOverScreen)
		{
			return;
		}

		Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = -2f;   // -2 because of the border of circle. (border has z= -1 in prefab).
		this.transform.position = mousePos;

		if (Input.GetMouseButtonUp(0) && IsValidPosToStopDrag)
		{
			OnIsAllowedToStopDragChanged -= SetColorIfAllowedToDrop;

			_isDragging = false;
			_circleRenderer.color = Color.white;
			this.enabled = false; // disable script after drag ends
			_isEnabled = false;     // disable OnMouseDown registering event


			// disabling Update() calls on both client and host.
			if(GameManager.GameMode == GameMode.Multiplayer && IsOwner && IsHost)
			{
				DisableNumberUpdateRpc();
			}

			var childObj = transform.GetChild(0); // get Circle object
			var childCircle = childObj.GetComponent<CircleCollider2D>(); // index 1 because the parent itself has collider and it also counts
			Debug.Log($"parent scale - {transform.localScale}, name - {name}");
			Debug.Log($"child scale - {childObj.transform.localScale}, name - {childObj.name}");
			Debug.Log($"child radisu - {childCircle.radius * childObj.transform.lossyScale.x}");
			// Notify Manager when drag ends
			OnDragEnded?.Invoke(
				new NumberModel(value,
								new Vector2(transform.position.x, transform.position.y),
								childCircle.radius * childObj.transform.lossyScale.x,
								gameObject)); // here to get real radius in World space
		}
	}

	
	private void OnMouseDown()
	{
		// do not perform any movement when the Number is already placed on grid.
		if (!_isEnabled)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			_isDragging = true;
			// set default color to green when starting
			SetColorIfAllowedToDrop(IsValidPosToStopDrag);
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		IsValidPosToStopDrag = false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// checking this because the OnCollision method triggers for both Numbers.
		if (!_isDragging)
		{
			return;
		}

		if (collision.gameObject.GetComponent<NumberScript>().GetType() == typeof(NumberScript))
		{
			IsValidPosToStopDrag = false;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		// checking this because the OnCollision method triggers for both Numbers.
		if (!_isDragging)
		{
			return;
		}

		if (collision.gameObject.GetComponent<NumberScript>().GetType() == typeof(NumberScript))
		{
			IsValidPosToStopDrag = true;
		}
	}


	private void SetColorIfAllowedToDrop(bool isAllowedToDrop)
	{
		_circleRenderer.color = isAllowedToDrop
			? new Color32(60, 143, 79, 255)
			: new Color32(255, 0, 0, 255);
	}


	/// <summary>
	/// Disables the Update() calls on clients and host as well after the number.
	/// Is called after the host stops moving a number.
	/// </summary>
	[Rpc(SendTo.ClientsAndHost)]
	private void DisableNumberUpdateRpc()
	{
		this.enabled = false;
	}
}
