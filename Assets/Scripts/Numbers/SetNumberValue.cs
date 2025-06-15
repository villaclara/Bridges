using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Sets the text value of number to clients in Multiplayer.
/// </summary>
public class SetNumberValue : NetworkBehaviour
{
    public TextMeshPro textValue;
   
    public void SetNumberValueText(string text)
    {
        if(NetworkManager.Singleton.IsHost)
        {
            UpdateClientRpc(text);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateClientRpc(string text)
    {
        textValue.text = text;
    }
}
