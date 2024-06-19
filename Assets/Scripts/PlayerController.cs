using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction moveAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        InputActionMap playerActionMap = InputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");

        moveAction.performed += OnMove;

        moveAction.Enable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();

        Debug.Log(input);
    }

    public float GetMovement()
    {
        return moveAction.ReadValue<float>();
    }

    private void OnDisable()
    {
        moveAction.Disable();

        moveAction.performed -= OnMove;
    }
}
