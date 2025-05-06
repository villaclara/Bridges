using System;
using UnityEngine;

public class Number : MonoBehaviour
{
	public static readonly Vector3 DefaultPosition = new(0f, 0f, -2f);   // if 0z - the black border is not visible
	private Camera _cam;
	private bool _isDragging;

	// invokes when the position is set. Is assigned in NumbersManager to create next number.
	public Action OnDragEnded;

	private bool _isEnabled = true;

	// Start is called before the first frame update
	void Start()
	{
		_cam = Camera.main;
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

		if (Input.GetMouseButtonUp(0))
		{
			_isDragging = false;

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
		if (!_isEnabled)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			_isDragging = true;
		}
	}

}
