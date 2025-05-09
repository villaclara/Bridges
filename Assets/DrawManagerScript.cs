using UnityEngine;

public class DrawManager : MonoBehaviour
{

	private Camera _cam;
	[SerializeField] private Line _linePrefab;
	[SerializeField] public GameObject Bridge;
	[SerializeField] private GameObject _intersectionCollider;
	[SerializeField] private GameObject _numbersManager;



	public const float RESOLUTION = 0.02f;
	private bool _canDraw = false;

	private Line _currentLine;
	void Start()
	{
		_cam = Camera.main;
		GlobalVars.OnNumbersPlaced += EnableDrawing;
	}

	// Update is called once per frame
	void Update()
	{
		// Check if mouse is not over the screen.
		// xz chu norm tyt.
		Vector3 mousePos1 = Input.mousePosition;
		var isMouseOverScreen = mousePos1.x >= 0 && mousePos1.x <= Screen.width &&
			   mousePos1.y >= 0 && mousePos1.y <= Screen.height;

		if (!isMouseOverScreen)
		{
			return;
		}

		Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(0) && _canDraw)
		{
			_currentLine = Instantiate(_linePrefab, mousePos, Quaternion.identity);
			_intersectionCollider.transform.position = mousePos;
		}

		if (Input.GetMouseButton(0) && _canDraw)
		{
			_currentLine.SetPosition(mousePos);
			_intersectionCollider.transform.position = mousePos;
		}
	}

	private void EnableDrawing()
	{
		_canDraw = true;
	}
}
