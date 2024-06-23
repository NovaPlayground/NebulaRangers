using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTop : MonoBehaviour
{
    private Controls inputActions;
    private InputAction moveAction;
    private InputAction normalShootAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        inputActions = new Controls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        //InputActionMap playerActionMap = InputActions.FindActionMap("PlayerTop");

        inputActions.Enable();

        moveAction = inputActions.FindAction("Movement");
        moveAction.performed += OnMove;

        normalShootAction = inputActions.FindAction("NormalShoot");
        normalShootAction.performed += OnShoot;

        moveAction.Enable();
        normalShootAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        moveAction.performed -= OnMove;
        moveAction.Disable();

        normalShootAction.performed -= OnShoot;
        normalShootAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
    }

    private void OnShoot(InputAction.CallbackContext context) 
    {
        bool input = context.ReadValueAsButton();
    }

    public float GetShoot()
    {
        return normalShootAction.ReadValue<float>();
    }

    public Vector2 GetMovement()
    {
        return moveAction.ReadValue<Vector2>();
    }

    public Vector2 GetMousePosition2D()
    {
        Vector2 mousePos = inputActions.PlayerTop.MousePosition.ReadValue<Vector2>();
        
        return mousePos;
    }

    public Vector2 GetMouseDelta2D()
    {
        Vector2 mouseDelta = inputActions.PlayerTop.MouseDelta.ReadValue<Vector2>();
        return mouseDelta;
    }
}
