using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NumberMessenger : NetworkBehaviour
{
    /// <summary>
    /// Internal list of the client. Used in Multiplayer to keep track of Current, Next etc Numbers.
    /// </summary>
    private NumbersList _numbersList;

    public void SetNumbersListReference(NumbersList numbersList)
    {
        _numbersList = numbersList;
    }

    /// <summary>
    /// Adds <see cref="NumberModel"/> to internal list as Rpc call. Passing all parameters separately because RPC does not support class obj.
    /// </summary>
    public void AddNumberToList(int value, Vector2 pos, float radius, ulong networkObjectId)
    {
        if(!IsHost)
        {
            return;
        }
       
        AddNumberToListRpc(value, pos, radius, networkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddNumberToListRpc(int value, Vector2 pos, float radius, ulong networkObjectId)
    {
		NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var obj);
		if (obj != null)
		{
		    _numbersList.Add(new NumberModel(value, pos, radius, obj.gameObject));
		}
    }

    /// <summary>
    /// Calls Rpc to Setup NumbersList (Current and Next objects) in clients in Multiplayer.
    /// </summary>
    public void SetupNumbersList()
    {
        if(!IsHost)
        {
            return;
        }
        SetupNumbersListRpc();
    }

    [Rpc(SendTo.NotServer)]
    private void SetupNumbersListRpc()
    {
        _numbersList.Setup();
    }

    /// <summary>
    /// Disables Spinning Circle prefab from numbers object depending on parameter.
    /// </summary>
    /// <param name="isCurrent">If true then Numbers.Current. If false then Numbers.Next</param>
    [Rpc(SendTo.ClientsAndHost)]
    public void DisableSpinCircleRpc(bool isCurrent, bool showCircle, bool destroyThisGO = false)
	{
        if(isCurrent)
        {
            SpinningCircleHelper.SetSpinningCircleForNumberModel(_numbersList.Current, showCircle, destroyThisGO);
        }
        else
        {
			SpinningCircleHelper.SetSpinningCircleForNumberModel(_numbersList.Next, showCircle, destroyThisGO);
		}
	}
}
