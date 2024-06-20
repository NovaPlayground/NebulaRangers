using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTop : MonoBehaviour
{
    private Controls inputActions;
    private InputAction moveAction;

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

        moveAction = inputActions.FindAction("Move");
        moveAction.performed += OnMove;

        moveAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        moveAction.performed -= OnMove;

        moveAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();
    }

    public float GetMovement()
    {
        return moveAction.ReadValue<float>();
    }

    public Vector2 GetMousePosition2D()
    {
        Vector2 mousePos = inputActions.PlayerTop.MousePosition.ReadValue<Vector2>();
        Debug.Log(mousePos);
        return mousePos;
    }

    public Vector2 GetMouseDelta2D()
    {
        Vector2 mouseDelta = inputActions.PlayerTop.MouseDelta.ReadValue<Vector2>();
        return mouseDelta;
    }
}
