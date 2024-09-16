using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMine : Mine
{
    [SerializeField] private GameObject endingPoint;
    [SerializeField] private float speed;
    
    private Vector3 endingPosition;
    private Vector3 startingPosition;
    private float counter = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        endingPosition = endingPoint.transform.position;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (endingPosition != null)
        {
            counter += Time.fixedDeltaTime;

            float lerpFactor = (Mathf.Sin(counter * speed) + 1.0f) * 0.5f;
            Debug.Log(lerpFactor);

            transform.position = Vector3.Lerp(startingPosition, endingPosition, lerpFactor);
        }
    }
}
