using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class ScreenBoundsEdges : MonoBehaviour
{
	private EdgeCollider2D edgeCollider;
	private LineRenderer lineRenderer;
	private int lastScreenWidth, lastScreenHeight;
	private float topUiHeight;

	public static ScreenBoundsEdges Instance;

	public GameObject endGameScreen;
	public GameObject disconnectScreen;
	public SpriteMask spriteMask; 

	void Awake()
	{
		Instance = this;
		edgeCollider = GetComponent<EdgeCollider2D>();
		lineRenderer = GetComponent<LineRenderer>();
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;

		/* Check if mobile / desktop to render collider
		 * The values 4.32f and 8.17f was found manually as the relation of how much space does take topUI
		 * The topUIHeight default value in Editor is 130, which then is calculated related to height.
		 */
		float aspect = (float)Screen.width / Screen.height;
		topUiHeight = aspect > 1 ? Screen.height / 4.32f : Screen.height / 8.17f;
		UpdateBounds();
	}

	void Update()
	{
		// If the game has ended/cancelled then we return EdgeCollider to normal values.
		if (endGameScreen.activeSelf || disconnectScreen.activeSelf)
		{
			UpdateBounds();
		}

		// Restrict changing the collider when the match is runnig.
		if (GameManager.MatchActive)
		{
			return;
		}

		if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
		{
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;

			/* Check if mobile / desktop to render collider
			 * The values 4.32f and 8.17f was found manually as the relation of how much space does take topUI
			 * The topUIHeight default value in Editor is 130, which then is calculated related to height.
			 */
			float aspect = (float)Screen.width / Screen.height;
			topUiHeight = aspect > 1 ? Screen.height / 4.32f : Screen.height / 8.17f;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Sets initial values for the <see cref="EdgeCollider2D"/> component based on <see cref="topUiHeight"/> value 
	/// and draws <see cref="LineRenderer"/> component on collider points.
	/// <br />
	/// For Multiplayer chek <see cref="ReceiveMPGameArea(Vector2[])"/> method.
	/// </summary>
	void UpdateBounds()
	{
		Camera cam = Camera.main;

		// Convert UI height (pixels) into world units
		float uiWorldHeight = cam.ScreenToWorldPoint(new Vector3(0, topUiHeight, 0)).y
							- cam.ScreenToWorldPoint(new Vector3(0, 0, 0)).y;

		// Get screen corners in world space
		Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
		Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, 0));
		Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
		Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, 0));

		// Apply UI offset
		topRight.y -= uiWorldHeight;
		topLeft.y -= uiWorldHeight;

		/*
		 * ADD these in case you want to make full rectangle appear.
		 * It adds some small gaps on the left, right, bottom.
		 */
		topLeft.x += uiWorldHeight * 0.05f;
		bottomLeft.x += uiWorldHeight * 0.05f;
		topRight.x -= uiWorldHeight * 0.05f;
		bottomRight.x -= uiWorldHeight * 0.05f;
		bottomLeft.y += uiWorldHeight * 0.05f;
		bottomRight.y += uiWorldHeight * 0.05f;

		// Convert to local space so collider aligns correctly
		Vector2[] points = new Vector2[5]
		{
			transform.InverseTransformPoint(bottomLeft),
			transform.InverseTransformPoint(bottomRight),
			transform.InverseTransformPoint(topRight),
			transform.InverseTransformPoint(topLeft),
			transform.InverseTransformPoint(bottomLeft) // close loop
		      };

		// setting points to collider
		edgeCollider.points = points;

		// rendering rectangle
		lineRenderer.positionCount = edgeCollider.points.Length;
		for (int i = 0; i < edgeCollider.points.Length; i++)
		{
			Vector3 worldPoint = transform.TransformPoint(edgeCollider.points[i]);
			worldPoint.z = -3;
			lineRenderer.SetPosition(i, worldPoint);
		}


		RenderSpriteMask();
		
	}

	/// <summary>
	/// Directly sets the <see cref="EdgeCollider2D"/> and <see cref="LineRenderer"/> if in Multiplayer game.
	/// </summary>
	/// <param name="points"></param>
	public void ReceiveMPGameArea(Vector2[] points)
	{
		edgeCollider.points = points;
		lineRenderer.positionCount = 0;
		lineRenderer.positionCount = edgeCollider.points.Length;
		for (int i = 0; i < edgeCollider.points.Length; i++)
		{
			Vector3 worldPoint = transform.TransformPoint(edgeCollider.points[i]);
			worldPoint.z = -3;
			lineRenderer.SetPosition(i, worldPoint);
		}

		RenderSpriteMask();
	}

	/// <summary>
	/// Get the bounds of <see cref="EdgeCollider2D"/> to use Math.Clamp.
	/// </summary>
	/// <returns></returns>
	public Bounds GetBounds()
	{
		return edgeCollider.bounds;
	}

	/// <summary>
	/// Computes and sets the Sprite Mask size to make visible background Grid only on Game Area.
	/// </summary>
	private void RenderSpriteMask()
	{
		// Step 1: Take the first 4 points (ignore last duplicate)
		Vector3[] worldPoints = new Vector3[4];
		for (int i = 0; i < 4; i++)
		{
			worldPoints[i] = transform.TransformPoint(edgeCollider.points[i]);
		}

		// Step 2: Compute center
		Vector3 center = (worldPoints[0] + worldPoints[1] + worldPoints[2] + worldPoints[3]) / 4f;
		spriteMask.transform.position = center;

		// Step 3: Compute width and height
		float width = Vector3.Distance(worldPoints[0], worldPoints[1]);
		float height = Vector3.Distance(worldPoints[1], worldPoints[2]);

		// Step 4: Account for parent scale
		Vector3 parentScale = spriteMask.transform.parent != null
			? spriteMask.transform.parent.lossyScale
			: Vector3.one;

		width /= parentScale.x;
		height /= parentScale.y;

		// Step 5: Scale mask relative to sprite size
		Vector2 spriteSize = spriteMask.sprite.bounds.size; // local units
		spriteMask.transform.localScale = new Vector3(
			width / spriteSize.x,
			height / spriteSize.y,
			1f
		);
	}
}
