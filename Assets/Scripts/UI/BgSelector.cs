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

		if (IsRunningOnMobileWeb())
		{
			Debug.Log("Running on mobile browser (WebGL)");
			GlobalVars.IsMobilePlatform = true;
		}
		else
		{
			Debug.Log("Running on desktop or editor");
			GlobalVars.IsMobilePlatform = false;
		}
	}

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern int IsMobilePlatform();
#endif

	public static bool IsRunningOnMobileWeb()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
        return IsMobilePlatform() == 1;
#else
		return Application.isMobilePlatform;
#endif
	}

	//void Start()
	//{
	//	if (IsRunningOnMobileWeb())
	//	{
	//		Debug.Log("Running on mobile browser (WebGL)");
	//		GlobalVars.IsMobilePlatform = true;
	//	}
	//	else
	//	{
	//		Debug.Log("Running on desktop or editor");
	//		GlobalVars.IsMobilePlatform = false;
	//	}
	//}
}
