using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Handles logic to switch between stages in MP.
/// </summary>
public class MP_StageSetup : NetworkBehaviour
{
    public GameObject gameManager;
    public GameObject numbersManager;
    public GameObject drawManager;

    /// <summary>
    /// Start Drawing Stage.
    /// </summary>
    [Rpc(SendTo.ClientsAndHost)]
    public void SetDrawStageRpc()
    {
        numbersManager.GetComponent<NumbersManager>().InvokeStageEnd();
        numbersManager.SetActive(false);
        drawManager.SetActive(true);
    }

    /// <summary>
    /// Sets End Game.
    /// </summary>
	[Rpc(SendTo.ClientsAndHost)]
	public void SetEndGameRpc()
	{
        // Waits 1 sec and then displays End Game Screen.
        // Is used to set small pause and to properly update the Bridges count on Client side.
        StartCoroutine(StartWait1sec());
	}

    /// <summary>
    /// Two Enumerators is needed because of WebGl. One waiting for another which actually waits 1 sec.
    /// </summary>
    /// <returns></returns>
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
