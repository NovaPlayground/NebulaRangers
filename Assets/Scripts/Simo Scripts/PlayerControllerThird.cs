using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerThird : MonoBehaviour
{
    // KEYBOARD
    
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float strafeSpeed;
    [SerializeField] private float hoverSpeed;

    // time how fast the ship going from 0 to 1, that means ( 1/2)
    [SerializeField] private float forwardAcceleration;
    [SerializeField] private float strafeAcceleration;
    [SerializeField] private float hoverAcceleration;

    private float activeForwardSpeed;
    private float activeStrafeSpeed;
    private float activeHoverSpeed;


    // MOUSE 

    [SerializeField] private float lookRateSpeed;
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollAcceleration;

    // save the mouse position. where the mouse is on the screen
    private Vector2 lookInput;

    // how far is the mouse from the center'screen ( need to know how far to move and rotate )
    private Vector2 screenCenter;

    // howw far the mouse is at a particular time
    private Vector2 mouseDistance;

    private float rollInput;



    // Start is called before the first frame update
    void Start()
    {
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;

        // Keep the mouse inside of the screen game
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Where the mouse is
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        // Distance from the center 
        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y; // use screenCenter.y to don't go out of the screen
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f); // Range, not matter how much we move our mouse, if the value would be more than on, i'll never be able to move beyond it

        // Rotate the ship around along the forward axis ( orientate the ship ) 
        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.deltaTime);


        // Moving Ship
        transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, rollInput * rollSpeed * Time.deltaTime, Space.Self);

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * forwardSpeed, forwardAcceleration * Time.deltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * strafeSpeed, strafeAcceleration * Time.deltaTime);
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * hoverSpeed, hoverAcceleration * Time.deltaTime);

        transform.position += transform.forward * activeForwardSpeed * Time.deltaTime;
        transform.position += (transform.right * activeStrafeSpeed * Time.deltaTime) + (transform.up * activeHoverSpeed * Time.deltaTime);
       
    }

    
    
}
