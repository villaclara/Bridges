using System.Collections.Generic;
using UnityEngine;

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
		_currentIndex++;
		if (_currentIndex >= _numbers.Count - 1)
		{
			return false;
		}

		this.Current = _numbers[_currentIndex];
		this.Next = _numbers[_currentIndex + 1];
		return true;
	}

	public NumberModel Current { get; private set; }
	public NumberModel Next { get; private set; }

	public void Setup()
	{
		this.Current = _numbers[0];
		this.Next = _numbers[1];
		_currentIndex = 0;
	}

	public void Add(NumberModel number)
	{
		_numbers.Add(number);
	}

}

public class NumberModel
{
	public int Value { get; }
	public Vector2 Position { get; }
	public float Radius { get; }

	public NumberModel(int number, Vector2 position, float radius)
	{
		Value = number;
		Position = position;
		Radius = radius;
	}
}
