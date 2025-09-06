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

	public readonly static Color32 DEFAULT_P1_COLOR = new(139, 78, 217, 255);
	public readonly static string DEFAULT_P1_COLOR_HEX = "#A259FF";
	public readonly static Color32 DEFAULT_P2_COLOR = new(12, 159, 140, 255);
	public readonly static string DEFAULT_P2_COLOR_HEX = "#00BFA6";

	public readonly static Color32 NUMBER_COLOR = new(228, 233, 190, 255);
}
