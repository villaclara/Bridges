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

	private float defaultRadius;	// is used in collider check to block number not by center, but rather full

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
			_circleRenderer = GetComponentInChildren<SpriteRenderer>();
			_circleRenderer.color = Color.white;
			SpinningCircleHelper.SetSpinningCircleForGO(gameObject, false, false);
			_isEnabled = false;
		}

		// if we ARE host OR the game is Local
		// We position numbers manually and subscribe to event to change color of number.
		else
		{
			_cam = Camera.main;
			_circleRenderer = GetComponentInChildren<SpriteRenderer>();
			_circleRenderer.color = Color.yellow;
			OnIsAllowedToStopDragChanged += SetColorIfAllowedToDrop;
		}

		var childObj = transform.GetChild(0); // get Circle object
		var childCircle = childObj.GetComponent<CircleCollider2D>(); // index 1 because the parent itself has collider and it also counts

		defaultRadius = childCircle.radius * childObj.transform.lossyScale.x;
	}

	// Update is called once per frame
	private void Update()
	{
		// If we are client we listening to host moving the number and the Update is not even called in Client.
		if (!_isDragging)
		{
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
		Vector3 newPos = mousePos;
		if (ScreenBoundsEdges.Instance != null)
		{
			Debug.Log($"Transform pos - {transform.position}");
			Debug.Log($"Edge pos - {ScreenBoundsEdges.Instance.GetBounds().min.x}");
			Bounds b = ScreenBoundsEdges.Instance.GetBounds();

			
			// adding/subtracting defaultRadius to set number position to full under the edgeCollider
			newPos.x = Mathf.Clamp(newPos.x, b.min.x + defaultRadius, b.max.x - defaultRadius);
			newPos.y = Mathf.Clamp(newPos.y, b.min.y + defaultRadius, b.max.y - defaultRadius);
		}
		this.transform.position = newPos;

		var rectangleRect = ScreenBoundsEdges.Instance.GetComponent<RectTransform>();
		//if (RectTransformUtility.ScreenPointToLocalPointInRectangle(boundsRect, Input.mousePosition, null, out var localPoint))
		//{
		//	// Clamp the position inside the rectangle
		//	float clampedX = Mathf.Clamp(localPoint.x, -boundsRect.rect.width / 2 + transform.GetComponent<RectTransform>().rect.width / 2, boundsRect.rect.width / 2 - transform.GetComponent<RectTransform>().rect.width / 2);
		//	float clampedY = Mathf.Clamp(localPoint.y, -boundsRect.rect.height / 2 + this.transform.GetComponent<RectTransform>().rect.height / 2, boundsRect.rect.height / 2 - transform.GetComponent<RectTransform>().rect.height / 2);

		//	this.transform.localPosition = new Vector2(clampedX, clampedY);
		//}

		//Vector3[] corners = new Vector3[4];
		//rectangleRect.GetWorldCorners(corners);
		//float minX = corners[0].x; // bottom-left
		//float maxX = corners[2].x; // top-right
		//float minY = corners[0].y;
		//float maxY = corners[2].y;
		//Debug.Log($"{corners[0].x} {corners[2].x} {corners[0].y} {corners[2].y}");
		//// Clamp circle position inside rectangle
		//Vector3 clampedPos = mousePos;
		//clampedPos.x = Mathf.Clamp(mousePos.x, minX, maxX);
		//clampedPos.y = Mathf.Clamp(mousePos.y, minY, maxY);

		//transform.position = clampedPos;


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

		Debug.Log("On Collision stay");
		if (collision.gameObject.TryGetComponent<NumberScript>(out _).GetType() == typeof(NumberScript))
		{
			IsValidPosToStopDrag = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// checking this because the OnCollision method triggers for both Numbers.
		if (!_isDragging)
		{
			return;
		}

		if (collision.gameObject.TryGetComponent<NumberScript>(out NumberScript number))
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

		if (collision.gameObject.TryGetComponent<NumberScript>(out _))
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
