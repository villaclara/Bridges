using System;
using UnityEngine;

/// <summary>
/// Class for managing spinning cirle around the Number. 
/// Handles fade animation and color.
/// </summary>
public class SpinningCircle : MonoBehaviour
{
	private float spinSpeed = 70f;
	private float alpha = 0.3f;
	private bool fadingOut = true;
	private SpriteRenderer _renderer;
	
	public SpriteRenderer spriteRenderer;
	public float fadeSpeed = 1f; // Speed of fade

	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		_renderer.color = PlayerManager.playerTurn switch
		{
			PlayerTurn.P1_Turn => GlobalVars.DEFAULT_P1_COLOR,	// purple
			PlayerTurn.P2_Turn => GlobalVars.DEFAULT_P2_COLOR,	// green
			_ => new Color32(47, 107, 67, 255)						// dark green
		};

		transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

		// Fade logic
		if (fadingOut)
		{
			alpha -= Time.deltaTime * fadeSpeed;
		}
		else
		{
			alpha += Time.deltaTime * fadeSpeed;
		}

		// Clamp between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// Apply new color with updated alpha
		Color c = spriteRenderer.color;
		c.a = alpha;
		spriteRenderer.color = c;

		// Switch direction at bounds
		if (alpha <= 0f)
		{
			fadingOut = false;
		}
		else if (alpha >= 1f)
		{
			fadingOut = true;
		}
	}
}
