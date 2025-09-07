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
	public NumberModel Current { get; private set; }
	public NumberModel Next { get; private set; }
	
	private NumbersList()
	{
		_numbers = new List<NumberModel>();
	}

	/// <summary>
	/// Moves the pointers of <see cref="Current"/> and <see cref="Next"/> to the next values in List of <see cref="NumberModel"/> if possible.
	/// </summary>
	/// <returns>True if move successfull, false otherwise.</returns>
	public bool MoveNext()
	{
		SpinningCircleHelper.SetSpinningCircleForNumberModel(Next, false);

		// this is called to remove circle when timer ends.
		SpinningCircleHelper.SetSpinningCircleForNumberModel(Current, false, destroyThisGO: true);	
		_currentIndex++;
		if (_currentIndex >= _numbers.Count - 1)
		{
			return false;
		}

		this.Current = _numbers[_currentIndex];
        this.Next = _numbers[_currentIndex + 1];
        SpinningCircleHelper.SetSpinningCircleForNumberModel(Current, true);
        return true;
	}

	/// <summary>
	/// Setups the initial values for <see cref="Current"/> and <see cref="Next"/> Numbers.
	/// </summary>
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

	/// <summary>
	/// Adds <see cref="NumberModel"/> to the list of numbers.
	/// </summary>
	/// <param name="number">Object ot add.</param>
	public void Add(NumberModel number)
	{
		_numbers.Add(number);
	}

	/// <summary>
	/// Clears the list of <see cref="NumberModel"/>.
	/// </summary>
	public void RemoveAll()
	{
		_numbers.Clear();
	}

}

/// <summary>
/// Represents the model of number object.
/// </summary>
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
