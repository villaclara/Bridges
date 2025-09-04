using Unity.Netcode;
using UnityEngine;

public class SetupGameArea : NetworkBehaviour
{
	public static SetupGameArea Instance;

	public override void OnNetworkSpawn()
	{
		Instance = this;
	}

	private bool CompareGameAreaClientWithMeHost(Vector2[] pointsOther)
	{
		var hostArea = GetPolygonArea(ScreenBoundsEdges.Instance.GetComponent<EdgeCollider2D>().points);
		var clientArea = GetPolygonArea(pointsOther);

		return hostArea.CompareTo(clientArea) > 0;
	}

	[Rpc(SendTo.ClientsAndHost)]
	public void SendGameAreaToClientsRpc(Vector2[] points)
	{
		ScreenBoundsEdges.Instance.ReceiveMPGameArea(points);
	}

	[Rpc(SendTo.ClientsAndHost)]
	public void RequestSendClientGameAreaToHostRpc(Vector2[] points)
	{
		if (IsServer)
		{
			var isHostGABigger = CompareGameAreaClientWithMeHost(points);
			if (isHostGABigger)
			{
				SendGameAreaToClientsRpc(points);
			}
			else
			{
				SendGameAreaToClientsRpc(ScreenBoundsEdges.Instance.GetComponent<EdgeCollider2D>().points);
			}
		}
	}

	public static float GetPolygonArea(Vector2[] pts)
	{
		if (pts == null || pts.Length < 3) return 0f;

		float area = 0f;
		for (int i = 0; i < pts.Length; i++)
		{
			Vector2 p1 = pts[i];
			Vector2 p2 = pts[(i + 1) % pts.Length]; // wrap around
			area += p1.x * p2.y - p2.x * p1.y;
		}

		return Mathf.Abs(area) / 2f;
	}
}