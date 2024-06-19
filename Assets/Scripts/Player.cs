using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private PlayerController playerController;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        float input = playerController.GetMovement();
        Vector3 movementDirection = new Vector3(0f, 0f, input);
        movementDirection.Normalize();

        rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
    }
}
