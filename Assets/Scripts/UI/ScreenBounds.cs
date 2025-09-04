using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class ScreenBoundsEdges : MonoBehaviour
{
	private EdgeCollider2D edgeCollider;
	private LineRenderer lineRenderer;
	private int lastScreenWidth, lastScreenHeight;

	private float topUiHeight;
	public static ScreenBoundsEdges Instance;

	void Awake()
	{
		Instance = this;
		edgeCollider = GetComponent<EdgeCollider2D>();
		lineRenderer = GetComponent<LineRenderer>();
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;

		// Check if mobile / desktop to render collider
		float aspect = (float)Screen.width / Screen.height;
		topUiHeight = aspect > 1 ? 115f : 280f;

		// Check if in Mobile Landscape mode
		var isMobile = BgSelector.IsRunningOnMobileWeb();
		if (isMobile && aspect > 1)
		{
			topUiHeight = 270f;
		}
		UpdateBounds();
	}

	void Update()
	{
		if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
		{
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;

			// Check if mobile / desktop to render collider
			float aspect = (float)Screen.width / Screen.height;
			topUiHeight = aspect > 1 ? 115f : 280f;

			// Check if in Mobile Landscape mode
			var isMobile = BgSelector.IsRunningOnMobileWeb();
			if (isMobile && aspect > 1)
			{
				topUiHeight = 270f;
			}
			UpdateBounds();
		}
	}

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
		 * ADD these in case you want to make full rectangle appear
		*/			
		//topLeft.x += uiWorldHeight * 0.05f;
		//bottomLeft.x += uiWorldHeight * 0.05f;
		//topRight.x -= uiWorldHeight * 0.05f;
		//bottomRight.x -= uiWorldHeight * 0.05f;
		//bottomLeft.y += uiWorldHeight * 0.05f;
		//bottomRight.y += uiWorldHeight * 0.05f;

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
		
	}

	public Bounds GetBounds()
	{
		return edgeCollider.bounds;
	}

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
	}
}
