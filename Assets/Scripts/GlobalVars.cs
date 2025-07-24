using System.Collections.Generic;
using UnityEngine;

public class GlobalVars
{
	public static bool IsMobilePlatform = false;

	public const int NUMBERS_COUNT = 5;

	public const int TURN_TIMER_VALUE_SECONDS = 10;

	public static int score = 0;

	public const float LINE_CREATE_MINIMAL_RESOLUTION = 0.02f;

	public readonly static List<GameObject> linesToDelete = new();
	public readonly static List<GameObject> bridgesToDelete = new();


	public readonly static Color32 DEFAULT_GREEN_BUTTON_COLOR = new(0, 191, 166, 227);
	public readonly static Color32 DARKER_GREEN_BUTTON_COLOR = new Color(
			DEFAULT_GREEN_BUTTON_COLOR.r * GlobalVars.DARKER_MULTIPLYER / 256,
			DEFAULT_GREEN_BUTTON_COLOR.g * GlobalVars.DARKER_MULTIPLYER / 256,
			DEFAULT_GREEN_BUTTON_COLOR.b * GlobalVars.DARKER_MULTIPLYER / 256,
			DEFAULT_GREEN_BUTTON_COLOR.a);
	public const float DARKER_MULTIPLYER = 0.8f;
}
