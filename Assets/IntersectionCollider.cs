using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
	[SerializeField] public GameObject Bridge;
	private bool _canPlaceBridge = true;

	public static float Radius;

	private void Awake()
	{
		Radius = GetComponent<CircleCollider2D>().radius;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (_canPlaceBridge && collision.CompareTag("Line"))
		{
			Debug.Log("Line collision, can place bridge");
			Instantiate(Bridge, transform.position, Quaternion.identity);
			Debug.Log(GlobalVars.score + 1);
			GlobalVars.score += 1;
		}
		else if (collision.CompareTag("Bridge"))
		{
			Debug.Log("Can not place bidge");
			_canPlaceBridge = false;
		}

		else if (collision.CompareTag("Number"))
		{
			Debug.Log("Can not place bidge");
			_canPlaceBridge = false;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Bridge") || collision.CompareTag("Number"))
		{
			Debug.Log("Can place bidge");
			_canPlaceBridge = true;
		}
	}


}
