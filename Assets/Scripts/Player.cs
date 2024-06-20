using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rollSpeed; 
    
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
        HandleMovement();
    }


    private void HandleMovement()
    {
        float rollLeft = playerController.GetRollLeftMovement();
        float rollRight = playerController.GetRollRightMovement();       

        float roll = rollRight - rollLeft;
        Quaternion rollRotation = Quaternion.AngleAxis(roll * rollSpeed * Time.fixedDeltaTime, Vector3.forward);


        //Quaternion combinedRotation = pitchRotation * yawRotation * rollRotation;
        rb.MoveRotation(rb.rotation * rollRotation);

         //SPACESHIP MOVEMENT FORWARD 
        Vector3 movementDirection = transform.forward;
        rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);

    }
}
