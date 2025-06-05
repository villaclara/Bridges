using UnityEngine;

public class spinningCircle : MonoBehaviour
{
	private float spinSpeed = 50f;

	public SpriteRenderer spriteRenderer;
	public float fadeSpeed = 0.5f; // Speed of fade

	private float alpha = 0.3f;
	private bool fadingOut = true;

	private SpriteRenderer _renderer;

	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		_renderer.color = PlayerManager.playerTurn switch
		{
			PlayerTurn.P1_Turn => new Color32(139, 78, 217, 255),
			PlayerTurn.P2_Turn => new Color32(12, 159, 140, 255),
			_ => new Color32(176, 255, 0, 255)
		};


		transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

		// Fade logic
		if (fadingOut)
			alpha -= Time.deltaTime * fadeSpeed;
		else
			alpha += Time.deltaTime * fadeSpeed;

		// Clamp between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// Apply new color with updated alpha
		Color c = spriteRenderer.color;
		c.a = alpha;
		spriteRenderer.color = c;

		// Switch direction at bounds
		if (alpha <= 0f)
			fadingOut = false;
		else if (alpha >= 1f)
			fadingOut = true;
	}
}
