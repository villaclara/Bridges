using System;
using UnityEngine;

public class Number : MonoBehaviour
{
	public static readonly Vector3 DefaultPosition = new(0f, 0f, -2f);   // if 0z - the black border is not visible
	private Camera _cam;
	private bool _isDragging;
	private bool _isValidPositionToStopDrag = true;
	public event Action<bool> OnIsAllowedToStopDragChanged;

	public bool IsValidPosToStopDrag
	{
		get => _isValidPositionToStopDrag;
		private set
		{
			_isValidPositionToStopDrag = value;
			Debug.Log($"Is allowed to drop set prop - {_isValidPositionToStopDrag}");
			OnIsAllowedToStopDragChanged?.Invoke(_isValidPositionToStopDrag);
		}
	}

	private SpriteRenderer _circleRenderer;

	// invokes when the position is set. Is assigned in NumbersManager to create next number.
	public Action OnDragEnded;

	private bool _isEnabled = true;

	// Start is called before the first frame update
	void Start()
	{
		_cam = Camera.main;
		_circleRenderer = GetComponentInChildren<SpriteRenderer>();
		//OnIsAllowedToStopDragChanged += SetColorIfAllowedToDrop;
		Debug.Log(OnIsAllowedToStopDragChanged.Method);
	}

	// Update is called once per frame
	void Update()
	{
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
		transform.position = mousePos;

		if (Input.GetMouseButtonUp(0) && IsValidPosToStopDrag)
		{
			OnIsAllowedToStopDragChanged -= SetColorIfAllowedToDrop;

			_isDragging = false;
			_circleRenderer.color = Color.yellow;
			// TODO - Disabling Update in Number after drag and release
			// Check whats better here. No need to set enabled two different times.
			// Maybe after adding lines it will be clear what to use.
			enabled = false; // disable script after drag ends
			_isEnabled = false;     // disable OnMouseDown registering event
			OnDragEnded?.Invoke(); // Notify Manager when drag ends
		}
	}

	void OnMouseDown()
	{
		// do not perform any movement when the Number is already placed on grid.
		if (!_isEnabled)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			_isDragging = true;
			//SetColorIfAllowedToDrop(_isAllowedToStopDrag);
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

		if (collision.gameObject.GetComponent<Number>().GetType() == typeof(Number))
		{
			IsValidPosToStopDrag = false;
			Debug.Log($"collision enter - {IsValidPosToStopDrag}");
			//SetColorIfAllowedToDrop(_isAllowedToStopDrag);
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		// checking this because the OnCollision method triggers for both Numbers.
		if (!_isDragging)
		{
			return;
		}

		if (collision.gameObject.GetComponent<Number>().GetType() == typeof(Number))
		{
			IsValidPosToStopDrag = true;
			Debug.Log($"collision exit - {IsValidPosToStopDrag}");

			//SetColorIfAllowedToDrop(_isAllowedToStopDrag);
		}
	}


	private void SetColorIfAllowedToDrop(bool isAllowedToDrop)
	{
		_circleRenderer.color = isAllowedToDrop
			? new Color32(60, 143, 79, 255)
			: new Color32(255, 0, 0, 255);
	}

}
