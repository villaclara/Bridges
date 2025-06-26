using System.Collections.Generic;
using UnityEngine;

public class GlobalVars
{
	public static bool IsMobilePlatform = false;

	public const int NUMBERS_COUNT = 5;

	public static int score = 0;

	public const float LINE_CREATE_MINIMAL_RESOLUTION = 0.02f;

	public readonly static List<GameObject> linesToDelete = new();
	public readonly static List<GameObject> bridgesToDelete = new();
}
