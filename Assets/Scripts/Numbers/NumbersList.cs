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

	public static NumbersList GetInstance()
	{
		return _instance ??= new NumbersList();
	}

	private NumbersList()
	{
		_numbers = new List<NumberModel>();
	}

	public void AddNumber(NumberModel number)
	{
		_numbers.Add(number);
	}

	public void MoveNext()
	{
		this.Current = this.Next;
	}

	public NumberModel Current { get; private set; }
	public NumberModel Next { get; private set; }

	public void Setup()
	{
		this.Current = _numbers[0];
		this.Next = _numbers[1];
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
	public int Radius { get; }

	public NumberModel(int number, Vector2 position, int radius)
	{
		Value = number;
		Position = position;
		Radius = radius;
	}
}
