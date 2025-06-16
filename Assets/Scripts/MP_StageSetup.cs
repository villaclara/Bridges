using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MP_StageSetup : NetworkBehaviour
{
    public GameObject gameManager;
    public GameObject numbersManager;
    public GameObject drawManager;

    [Rpc(SendTo.ClientsAndHost)]
    public void SetDrawStageRpc()
    {
        numbersManager.GetComponent<NumbersManager>().InvokeStageEnd();
        numbersManager.SetActive(false);
        drawManager.SetActive(true);
    }

	[Rpc(SendTo.ClientsAndHost)]
	public void SetEndGameRpc()
	{
		numbersManager.GetComponent<DrawManager>().InvokeStageEnd();
		drawManager.SetActive(false);
	}

}
