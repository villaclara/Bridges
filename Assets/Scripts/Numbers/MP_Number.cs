using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Sets the text value of number to clients in Multiplayer.
/// </summary>
public class MP_Number : NetworkBehaviour
{
    public TextMeshPro textValue;

    public GameObject gameObj;
    //public void SetNumberValueText(string text)
    //{
    //    if(NetworkManager.Singleton.IsHost)
    //    {
    //        UpdateClientRpc(text);
    //    }
    //}

    //[Rpc(SendTo.ClientsAndHost)]
    //private void UpdateClientRpc(string text)
    //{
    //    textValue.text = text;
    //}


    public void SetNumberValueInClient(int number)
    {
        if(NetworkManager.IsHost)
        {
            UpdateNumberInClientRpc(number);
        }
    }

    [Rpc(SendTo.NotOwner)]
    private void UpdateNumberInClientRpc(int number)
    {
        gameObject.GetComponent<NumberScript>().value = number;
		textValue.text = number.ToString();
	}
}
