using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for Line GO which adds the points to the line and draws it.
/// </summary>
public class Line : MonoBehaviour
{

	[SerializeField] private LineRenderer _renderer;
	[SerializeField] private EdgeCollider2D _collider;
	[SerializeField] private GameObject _bridge;

	private readonly List<Vector2> _points = new List<Vector2>();
	void Start()
	{
		_collider.transform.position -= transform.position; //to make collider points the same as line points
	}

	/// <summary>
	/// Sets the color of <see cref="LineRenderer"/> to the current player color.
	/// </summary>
	/// <param name="colorHEX"></param>
	public void SetLineColor(string colorHEX)
	{
		if (ColorUtility.TryParseHtmlString(colorHEX, out Color newcolor))
		{
			_renderer.material = new Material(Shader.Find("Sprites/Default"));
			_renderer.startColor = newcolor;
			_renderer.endColor = newcolor;
		}
	}

	/// <summary>
	/// Adds position to the line and updates the <see cref="EdgeCollider2D"/> object with Line.
	/// </summary>
	/// <param name="pos">Position to add.</param>
	public void SetPosition(Vector2 pos)
	{
		if (!CanAppend(pos))
		{
			return;
		}

		_points.Add(pos);
		_renderer.positionCount++;
		_renderer.SetPosition(_renderer.positionCount - 1, new Vector3(pos.x, pos.y, -3));  // z = -3 to display line above number. Remove if want display behind number.

		_collider.points = _points.ToArray();
	}

	/// <summary>
	/// Checks if the position can be added into the line.
	/// Using distance between last position of line to the position in parameter.
	/// </summary>
	/// <param name="pos">Position to check.</param>
	/// <returns>True/False if position can be added to the Line.</returns>
	private bool CanAppend(Vector2 pos)
	{
		if (_renderer.positionCount == 0)
			return true;

		return Vector2.Distance(_renderer.GetPosition(_renderer.positionCount - 1), pos) > GlobalVars.LINE_CREATE_MINIMAL_RESOLUTION;
	}
}
