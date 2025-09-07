using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manager class for <see cref="Line"/> which handles logic to mouse movement and instantianing new Line.
/// </summary>
public class DrawManager : MonoBehaviour, IGameStage
{

	#region fields
	[SerializeField] private Line _linePrefab;
	[SerializeField] private GameObject Bridge;
	[SerializeField] private GameObject _intersectionCollider;
	[SerializeField] private NumbersList _numbers;
	[SerializeField] private PlayerManager _playerManager;
	[SerializeField] private MP_StageSetup _stageSetup;

	private bool _isDrawing = false;
	private bool _isDrawingToNextCompleted = true;

	private Camera _cam;
	private Line _currentLine;
	
	// is used for temporary saving _currentLine object into memory
	private Line _unfinishedLine;
	private Vector2 _previousColliderPos;

	/// <inheritdoc/>
	public event Action OnStageExecutionCompleted;

	public NumberMessenger numberMessenger;
	public DrawMessenger drawMessenger;

	#endregion fields

	void Start()
	{
		drawMessenger.NumbersMoveNextReturnedFalse += DrawMessenger_NumbersMoveNextReturnFalse;
		drawMessenger.SetNumbersListReference(_numbers);
		StepTimerScript.StepTimerFinished += StepTimerScript_StepTimerFinished;
	}

	// Update is called once per frame
	void Update()
	{
		if(GameManager.GameMode == GameMode.Multiplayer && PlayerManager.playerTurn == PlayerTurn.P2_Turn && NetworkManager.Singleton.IsHost)
		{
			return;
		}

		if (GameManager.GameMode == GameMode.Multiplayer && PlayerManager.playerTurn == PlayerTurn.P1_Turn && !NetworkManager.Singleton.IsHost)
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

		Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

		// Restrict setting mouse position and therefore Line by Game Area Bounds.
		Vector3 newPos = mousePos;
		if (ScreenBoundsEdges.Instance != null)
		{
			Bounds b = ScreenBoundsEdges.Instance.GetBounds();

			newPos.x = Mathf.Clamp(newPos.x, b.min.x, b.max.x);
			newPos.y = Mathf.Clamp(newPos.y, b.min.y, b.max.y);
		}
		mousePos = newPos;

		// Press 
		if (Input.GetMouseButtonDown(0))
		{
			// when starting new line (at first or after completed drawing to next number)
			if (_isDrawingToNextCompleted)
			{
				// check positions
				if (IfPointInsideCurrentNumber(mousePos))
				{
					// Instantiating line in MP
					if(GameManager.GameMode == GameMode.Multiplayer)
					{
						numberMessenger.DisableSpinCircleRpc(isCurrent: true, showCircle: false, destroyThisGO: true);
						numberMessenger.DisableSpinCircleRpc(isCurrent: false, showCircle: true);
						
						_currentLine = Instantiate(_linePrefab, new Vector3(mousePos.x, mousePos.y, -4), Quaternion.identity);
						if(NetworkManager.Singleton.IsHost)
						{
							// Host spawns Network Object line to be dispayed in Client.
							_currentLine.GetComponent<NetworkObject>().Spawn();
							_currentLine.GetComponent<MP_Line>().RequestLineColorChangeOnClientRpc();
						}
						else
						{
							// Client asks the Host to instantiate and spawn Line only on Server (on Client we already have one).
							drawMessenger.RequestInstantiateLineOnServerRpc(new Vector3(mousePos.x, mousePos.y, -4));
						}
					}

					// Local gameplay, nothing changes
					else
					{
						SpinningCircleHelper.SetSpinningCircleForNumberModel(_numbers.Current, false, destroyThisGO: true);
						SpinningCircleHelper.SetSpinningCircleForNumberModel(_numbers.Next, true);
						_currentLine = Instantiate(_linePrefab, new Vector3(mousePos.x, mousePos.y, -4), Quaternion.identity);
					}

					GlobalVars.linesToDelete.Add(_currentLine.gameObject);
					if (PlayerManager.playerTurn == PlayerTurn.P1_Turn)
					{
						_currentLine.SetLineColor(PlayerManager.player1.ColorHEX);
					}
					else
					{
						_currentLine.SetLineColor(PlayerManager.player2.ColorHEX);
					}

					// disabling-enabling collider to reset its position to not call the OnCollision() Methods
					_intersectionCollider.GetComponent<CircleCollider2D>().enabled = false;
					_intersectionCollider.transform.position = mousePos;
					_intersectionCollider.GetComponent<CircleCollider2D>().enabled = true;

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

			if(GameManager.GameMode == GameMode.Multiplayer && NetworkManager.Singleton.IsHost)
			{
				// Spawn net point of Line on Network Object to display on Client side.
				_currentLine.GetComponent<MP_Line>().SetNewValueToPoint(mousePos);
			}
			else if (GameManager.GameMode == GameMode.Multiplayer && !NetworkManager.Singleton.IsHost)
			{
				// Spawn new point of Line ONLY localy on Host.
				drawMessenger.RequestAddVertextToLineOnServerRpc(mousePos);
			}

			if (IfPointPreciselyInsideNextNumber(mousePos))
			{
				_isDrawingToNextCompleted = true;
				_isDrawing = false;

				NumbersMoveNext_and_PlayerSwitchTurns();
			}
		}

		// Release
		if (Input.GetMouseButtonUp(0) && _isDrawing)
		{
			// Line is at the next number.
			if (IfPointInsideNextNumber(mousePos))
			{
				_isDrawingToNextCompleted = true;
				_isDrawing = false;

				NumbersMoveNext_and_PlayerSwitchTurns();
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

	/// <summary>
	/// Execute NumbersMoveNext and Switch Turns when the player did not reach next number in time.
	/// </summary>
	private void StepTimerScript_StepTimerFinished()
	{
		// TOOD - Here is invoked only on Host. Need to invoke in client too.
		_isDrawing = false;
		_isDrawingToNextCompleted = true;

		if (GameManager.GameMode == GameMode.Local)
		{
			NumbersMoveNext_and_PlayerSwitchTurns();
			return;
		}

		// Deciding who should call method
		if (PlayerManager.playerTurn == PlayerTurn.P1_Turn && NetworkManager.Singleton.IsHost)
		{
			// Is P1 and Host
			NumbersMoveNext_and_PlayerSwitchTurns();
		}
		else if (PlayerManager.playerTurn == PlayerTurn.P2_Turn && !NetworkManager.Singleton.IsHost)
		{
			// Is P2 and Client
			NumbersMoveNext_and_PlayerSwitchTurns();
		}
	}

	/// <summary>
	/// Calls the Endgame in MP.
	/// </summary>
	private void DrawMessenger_NumbersMoveNextReturnFalse()
	{
		this.enabled = false;
		_stageSetup.SetEndGameRpc();
	}

	/// <summary>
	/// Executes <see cref="NumbersList.MoveNext"/> to get Current and Next numbers to draw.
	/// Switches player turns <see cref="PlayerManager.SwitchTurns"/>. 
	/// Triggers <see cref="OnStageExecutionCompleted"/> in case <see cref="NumbersList.MoveNext"/> returns false.
	/// </summary>
	private void NumbersMoveNext_and_PlayerSwitchTurns()
	{
		if (GameManager.GameMode == GameMode.Multiplayer)
		{
			// passing the Host parameter to decide and make call MoveNext on ANOTHER client. 
			// Because below we invoke it locally to have the boolean var of end game condition
			drawMessenger.NumbersMoveNext(NetworkManager.Singleton.IsHost);
		}

		// get new values for numbers.Current and .Next
		var isMoveNextReady = _numbers.MoveNext();
		if (!isMoveNextReady)
		{
			this.enabled = false;
			//OnStageExecutionCompleted?.Invoke();

			if (GameManager.GameMode == GameMode.Multiplayer)
			{
				_stageSetup.SetEndGameRpc();
			}
			else
			{
				InvokeStageEnd();
			}
			return;
		}

		if (GameManager.GameMode == GameMode.Local)
		{
			PlayerManager.SwitchTurns();
		}
		else
		{
			if (NetworkManager.Singleton.IsHost)
			{
				PlayerManager.SwitchTurns();
			}
			else
			{
				drawMessenger.RequestServerPlayerTurnSwitchRpc();
			}
		}
	}

	#region StageExecution
	
	/// <inheritdoc/>
	public void ExecuteStage()
	{
		//_canDraw = true;
		gameObject.SetActive(true);
		this.enabled = true;

		_cam = Camera.main;
		_numbers = NumbersList.GetInstance();

		// Set first turn randomly
		bool rnd = true;
		if (GameManager.GameMode == GameMode.Multiplayer)
		{
			rnd = MP_PlayerDrawing.IsP1TurnRandomSpawnedMP;
		}
		else
		{
			uint seed = (uint)System.Environment.TickCount;
			if (seed == 0) seed = 1;
			rnd = new Unity.Mathematics.Random(seed).NextBool();
		}
		PlayerManager.SetupFirstTurn(rnd);
	}
	
	/// <inheritdoc/>
	public void ResetStage()
	{
		foreach (var line in GlobalVars.linesToDelete)
		{
			Destroy(line);
		}
	}
	public void InvokeStageEnd()
	{
		OnStageExecutionCompleted?.Invoke();
	}

	#endregion StageExecution

	#region CheckPoints secion
	/* These helper methods are used to check if the current point (the Head of line) is:
	 * 1. near or inside Number object
	 * 2. crossing another Line
	 * 3. near unfinished line -> is used to continue drawing the same turn
	 */
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

	#endregion CheckPoints section
}
