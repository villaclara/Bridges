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
        // Waits 1 sec and then displays End Game Screen.
        // Is used to set small pause and to properly update the Bridges count on Client side.
        StartCoroutine(StartWait1sec());
	}

    private IEnumerator StartWait1sec()
    {
        yield return Wait1SecEnumerator();
		drawManager.GetComponent<DrawManager>().InvokeStageEnd();
		drawManager.SetActive(false);
	}

    private IEnumerator Wait1SecEnumerator()
    {
        yield return new WaitForSeconds(1);
    }
}
