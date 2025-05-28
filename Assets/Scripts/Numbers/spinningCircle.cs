using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinningCircle : MonoBehaviour
{
    private float spinSpeed = 100f;
    void Update()
    {
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
    }
}
