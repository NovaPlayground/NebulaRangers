using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset InputActions;

    //[SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    private InputAction rollLeft;
    private InputAction rollRight;

    private float yaw;
    private float pitch;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        //Update yaw and pitch angles
        yaw += mouseX;
        pitch -= mouseY; // inverted axis
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        //Apply rotation to the camera
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);
    }

    private void OnEnable()
    {
        InputActionMap playerActionMap = InputActions.FindActionMap("Player");
        
        rollLeft = playerActionMap.FindAction("RollLeft");
        rollRight = playerActionMap.FindAction("RollRight");


        rollLeft.performed += OnMove;
        rollRight.performed += OnMove;

        rollLeft.Enable();
        rollRight.Enable();
  
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        Debug.Log(input);
    }

    

    public float GetRollLeftMovement()
    {
        return rollLeft.ReadValue<float>();

    }

    public float GetRollRightMovement()
    {
        return rollRight.ReadValue<float>();

    }

    

    private void OnDisable()
    {

        rollLeft.Disable();
        rollRight.Disable();

        rollLeft.performed -= OnMove;
        rollRight.performed -= OnMove;
  
    }
}
