using UnityEngine;

public class BgSelector : MonoBehaviour
{

	public GameObject iPhoneAR;
	public GameObject DesktopAR;

	// Start is called before the first frame update
	void Awake()
	{
		float aspect = (float)Screen.width / Screen.height;

		if (aspect < 1)
		{
			iPhoneAR.SetActive(true);
			DesktopAR.SetActive(false);
		}
		else
		{
			iPhoneAR.SetActive(false);
			DesktopAR.SetActive(true);
		}
	}
}
