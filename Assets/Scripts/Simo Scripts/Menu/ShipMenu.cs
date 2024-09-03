using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMenu : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 startPosition;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        time += moveSpeed * Time.deltaTime;
        float yOffSett = Mathf.Sin(time) * moveDistance;

        transform.position = startPosition + new Vector3(0, yOffSett, 0);
    }
}
