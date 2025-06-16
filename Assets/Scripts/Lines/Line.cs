using System.Collections.Generic;
using UnityEngine;

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

	public void SetLineColor(string colorHEX)
	{
		if (ColorUtility.TryParseHtmlString(colorHEX, out Color newcolor))
		{
			_renderer.material = new Material(Shader.Find("Sprites/Default"));
			_renderer.startColor = newcolor;
			_renderer.endColor = newcolor;
			Debug.Log($"set color - {newcolor}");
		}
	}

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

	private bool CanAppend(Vector2 pos)
	{
		if (_renderer.positionCount == 0)
			return true;

		return Vector2.Distance(_renderer.GetPosition(_renderer.positionCount - 1), pos) > GlobalVars.LINE_CREATE_MINIMAL_RESOLUTION;
	}

	//private void OnTriggerEnter2D(Collider2D collision)
	//{
	//    Instantiate(_bridge, collision.transform.position, Quaternion.identity);
	//    Debug.Log(GlobalVars.score + 1);
	//    GlobalVars.score += 1;
	//}

}
