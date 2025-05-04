using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager : MonoBehaviour
{

    private Camera _cam;
    [SerializeField] private Line _linePrefab;
    [SerializeField] public GameObject _bridge;
    [SerializeField] private GameObject _intersectionCollider;



    public const float RESOLUTION = 0.02f;

    private Line _currentLine;
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            _currentLine = Instantiate(_linePrefab, mousePos, Quaternion.identity);
            _intersectionCollider.transform.position = mousePos;
        }

        if (Input.GetMouseButton(0))
        {
            _currentLine.SetPosition(mousePos);
            _intersectionCollider.transform.position = mousePos;
        }
    }
}
