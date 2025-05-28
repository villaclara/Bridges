using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager : MonoBehaviour, IGameStage
{
	private Camera _cam;
	[SerializeField] private Line _linePrefab;
	[SerializeField] public GameObject Bridge;
	[SerializeField] private GameObject _intersectionCollider;
	[SerializeField] private NumbersList _numbers;
	[SerializeField] private PlayerManager _playerManager;

	[SerializeField]
	private bool _isDrawing = false;

	[SerializeField]
	private bool _isDrawingToNextCompleted = true;

	private Line _currentLine;

	// is used for temporary saving _currentLine object into memory
	private Line _unfinishedLine;
	private Vector2 _previousColliderPos;


	private readonly List<GameObject> _linesToDelete = new();


	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public event Action OnStageExecutionCompleted;

	void Start()
	{
		//_cam = Camera.main;
		//_numbers = NumbersList.GetInstance();

		//// Set first turn randomly
		//uint seed = (uint)System.Environment.TickCount;
		//if (seed == 0) seed = 1;
		//var rnd = new Unity.Mathematics.Random(seed).NextBool();
		//PlayerManager.SetupFirstTurn(rnd);
		//Debug.Log($"First turn - {PlayerManager.playerTurn}");
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

		// Press 
		if (Input.GetMouseButtonDown(0))
		{
			// when starting new line (at first or after completed drawing to next number)
			if (_isDrawingToNextCompleted)
			{
				// check positions
				if (IfPointInsideCurrentNumber(mousePos))
				{
					SpinningCircleHelper.DisableSpinningCircle(_numbers.Current, false);
                    SpinningCircleHelper.DisableSpinningCircle(_numbers.Next, true);
					//TODO
					//remove spinning circle after this for better resource management

                    _currentLine = Instantiate(_linePrefab, mousePos, Quaternion.identity);
					_linesToDelete.Add(_currentLine.gameObject);
					if (PlayerManager.playerTurn == PlayerTurn.P1_Turn)
					{
						_currentLine.SetLineColor(PlayerManager.player1.ColorHEX);
						Debug.Log($"Current turn P1 - set color to - {PlayerManager.player1.ColorHEX}");
					}
					else
					{
						_currentLine.SetLineColor(PlayerManager.player2.ColorHEX);
						Debug.Log($"Current turn P2 - set color to - {PlayerManager.player2.ColorHEX}");
					}
					Debug.Log($"current turn - {PlayerManager.playerTurn}");
					_intersectionCollider.transform.position = mousePos;

					_isDrawing = true;
					_isDrawingToNextCompleted = false;
				}
			}

			// drawing to next not completed
			// check the position of previous line
			else
			{
				if (IfPointNearUnfinishedLine(mousePos))
				{
					_currentLine = _unfinishedLine;
					_intersectionCollider.transform.position = mousePos;

					_isDrawing = true;
					_isDrawingToNextCompleted = false;
				}
			}

		}

		// Hold
		if (Input.GetMouseButton(0) && _isDrawing && !_isDrawingToNextCompleted)
		{
			_currentLine.SetPosition(mousePos);
			_intersectionCollider.transform.position = mousePos;

			if (IfPointPreciselyInsideNextNumber(mousePos))
			{
				_isDrawingToNextCompleted = true;
				_isDrawing = false;
				PlayerManager.SwitchTurns();
				Debug.Log("HOLD - Player manager switch turne");

				// get new values for numbers.Current and .Next
				var isMoveNextReady = _numbers.MoveNext();
				if (!isMoveNextReady)
				{
					Debug.Log("Execution ended");
					this.enabled = false;
					OnStageExecutionCompleted?.Invoke();
				}
			}
		}

		// Release
		if (Input.GetMouseButtonUp(0) && _isDrawing)
		{
			// Line is at the next number.
			if (IfPointInsideNextNumber(mousePos))
			{
				_isDrawingToNextCompleted = true;
				PlayerManager.SwitchTurns();
				Debug.Log("RELEASE  - Player manager switch turne");

				// get new values for numbers.Current and .Next
				var isMoveNextReady = _numbers.MoveNext();
				if (!isMoveNextReady)
				{
					Debug.Log("Execution ended");
					this.enabled = false;
					OnStageExecutionCompleted?.Invoke();
				}
			}

			// Line released somewhere, we save its position to start drawing again from it
			else
			{
				_unfinishedLine = _currentLine;
				_isDrawingToNextCompleted = false;

				_previousColliderPos = mousePos;
			}

			_isDrawing = false;
		}
	}

	public void ExecuteStage()
	{
		//_canDraw = true;
		gameObject.SetActive(true);
		this.enabled = true;

		_cam = Camera.main;
		_numbers = NumbersList.GetInstance();

		// Set first turn randomly
		uint seed = (uint)System.Environment.TickCount;
		if (seed == 0) seed = 1;
		var rnd = new Unity.Mathematics.Random(seed).NextBool();
		PlayerManager.SetupFirstTurn(rnd);
		Debug.Log($"First turn - {PlayerManager.playerTurn}");
	}

	private bool IfPointInsideCurrentNumber(Vector2 point) =>
		(point.x >= _numbers.Current.Position.x - _numbers.Current.Radius) && (point.x <= _numbers.Current.Position.x + _numbers.Current.Radius)
				&& (point.y >= _numbers.Current.Position.y - _numbers.Current.Radius) && (point.y <= _numbers.Current.Position.y + _numbers.Current.Radius);

	private bool IfPointInsideNextNumber(Vector2 point) =>
		(point.x >= _numbers.Next.Position.x - _numbers.Next.Radius) && (point.x <= _numbers.Next.Position.x + _numbers.Next.Radius)
				&& (point.y >= _numbers.Next.Position.y - _numbers.Next.Radius) && (point.y <= _numbers.Next.Position.y + _numbers.Next.Radius);

	private bool IfPointPreciselyInsideNextNumber(Vector2 point) =>
		(point.x >= _numbers.Next.Position.x - _numbers.Next.Radius / 4) && (point.x <= _numbers.Next.Position.x + _numbers.Next.Radius / 4)
				&& (point.y >= _numbers.Next.Position.y - _numbers.Next.Radius / 3) && (point.y <= _numbers.Next.Position.y + _numbers.Next.Radius / 3);

	private bool IfPointNearUnfinishedLine(Vector2 point) =>
		(point.x >= _previousColliderPos.x - IntersectionCollider.Radius * 2) && (point.x <= _previousColliderPos.x + IntersectionCollider.Radius * 2)
					&& (point.y >= _previousColliderPos.y - IntersectionCollider.Radius * 2) && (point.y <= _previousColliderPos.y + IntersectionCollider.Radius * 2);

	public void ResetStage()
	{
		foreach (var line in _linesToDelete)
		{
			Destroy(line);
		}
	}
}
