using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

/// <summary>
/// Class, which is exposed to managers to get current, next etc Numbers.
/// Singleton.
/// </summary>
public class NumbersList
{
	private List<NumberModel> _numbers;
	private static NumbersList _instance;
	private static int _currentIndex = 0;
	public static NumbersList GetInstance()
	{
		return _instance ??= new NumbersList();
	}

	private NumbersList()
	{
		_numbers = new List<NumberModel>();
	}

	public bool MoveNext()
	{
		SpinningCircleHelper.DisableSpinningCircle(Next, false);
        _currentIndex++;
		if (_currentIndex >= _numbers.Count - 1)
		{
			return false;
		}

		this.Current = _numbers[_currentIndex];
        this.Next = _numbers[_currentIndex + 1];
        SpinningCircleHelper.DisableSpinningCircle(Current, true);
        return true;
	}

	public NumberModel Current { get; private set; }
	public NumberModel Next { get; private set; }

	public void Setup()
	{
		Current = _numbers[0];
		Next = _numbers[1];
		_currentIndex = 0;
        UnityEngine.Transform childTransform = Current.NumberObject.transform.Find("spinningCircle");
        if (childTransform != null)
        {
            var childGameObject = childTransform.gameObject;
            childGameObject.SetActive(true);
        }

    }

	public void Add(NumberModel number)
	{
		_numbers.Add(number);
	}

	public void RemoveAll()
	{
		_numbers.Clear();
	}

}

public class NumberModel
{
	public int Value { get; }
	public Vector2 Position { get; }
	public float Radius { get; }

	public GameObject NumberObject { get; }
	public NumberModel(int number, Vector2 position, float radius, GameObject numberObject)
	{
		Value = number;
		Position = position;
		Radius = radius;
		NumberObject = numberObject;	
	}
}
