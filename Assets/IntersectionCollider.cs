using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCollider : MonoBehaviour
{
    [SerializeField] public GameObject _bridge;
    private bool canPlaceBridge = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canPlaceBridge && collision.CompareTag("Line"))
        {
            Debug.Log("Line collision, can place bridge");
            Instantiate(_bridge, transform.position, Quaternion.identity);
            Debug.Log(GlobalVars.score + 1);
            GlobalVars.score += 1;
        }
        else if(collision.CompareTag("Bridge") || collision.CompareTag("Number"))
        {
            Debug.Log("Can not place bidge");
            canPlaceBridge = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Bridge") || collision.CompareTag("Number"))
        {
            Debug.Log("Can place bidge");
            canPlaceBridge = true;
        }
    }
}
