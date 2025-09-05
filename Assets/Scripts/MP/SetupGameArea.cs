using Unity.Netcode;
using UnityEngine;

public class SetupGameArea : NetworkBehaviour
{
	/// <summary>
	/// Compares the current <see cref="ScreenBoundsEdges.Instance"/> <see cref="EdgeCollider2D"/> points with parameter.
	/// </summary>
	/// <param name="pointsOther"><see cref="EdgeCollider2D"/> points of other player.</param>
	/// <returns>True if this current player area is bigger than other player, false otherwise.</returns>
	private bool CompareGameAreaClientWithMeHost(Vector2[] pointsOther)
	{
		var hostArea = GetPolygonArea(ScreenBoundsEdges.Instance.GetComponent<EdgeCollider2D>().points);
		var clientArea = GetPolygonArea(pointsOther);

		return hostArea.CompareTo(clientArea) > 0;
	}

	/// <summary>
	/// Sends to both Client and Host the command to change the <see cref="EdgeCollider2D"/> bounds to be the same.
	/// </summary>
	/// <param name="points">The array of points for EdgeCollider to create.</param>
	[Rpc(SendTo.ClientsAndHost)]
	public void SendGameAreaToClientsRpc(Vector2[] points)
	{
		ScreenBoundsEdges.Instance.ReceiveMPGameArea(points);
	}

	/// <summary>
	/// Sends from Client to Host the request with Clients <see cref="EdgeCollider2D"/> points as array.
	/// Is called at the <see cref="ConnectionStatus.OnClientConnected(ulong)"/> callback in <see cref="ConnectionStatus"/>.
	/// </summary>
	/// <param name="points">Clients Collider points array.</param>
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

	/// <summary>
	/// Gets the area of the rectangle formed from points.
	/// </summary>
	/// <param name="pts">Array of points.</param>
	/// <returns>The area of rectangle formed from points.</returns>
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