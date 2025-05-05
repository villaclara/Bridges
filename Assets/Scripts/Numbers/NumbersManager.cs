using TMPro;
using UnityEngine;

public class NumbersManager : MonoBehaviour
{
	[SerializeField]
	private int _currentNumber = 1;

	public Number numberPrefab;
	// Start is called before the first frame update
	void Start()
	{
		CreateNumber();
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void CreateNumber()
	{
		if (_currentNumber > GlobalVars.NUMBERS_COUNT)
		{
			return;
		}

		var current = Instantiate(numberPrefab, Number.DefaultPosition, Quaternion.identity);
		var textObject = current.transform.GetComponentInChildren<TextMeshPro>();
		current.CurrentNumber = _currentNumber;
		textObject.text = _currentNumber.ToString();
		current.gameObject.SetActive(true);
		_currentNumber++;

		// call CreateNumber again when drag ended.
		current.OnDragEnded = CreateNumber;
	}
}
