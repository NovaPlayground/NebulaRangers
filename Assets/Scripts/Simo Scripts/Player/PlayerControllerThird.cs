using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerThird : MonoBehaviour
{
    private Controls inputActions;
    private InputAction moveAction;
    private InputAction moveUpDownAction;
    private InputAction lookAction;
    private InputAction rollAction;
    private InputAction shootAction;
    private InputAction shootMissileAction;

    private void Awake()
    {
        inputActions = new Controls();

    }

    private void OnEnable()
    {
        inputActions.Enable();

        moveAction = inputActions.FindAction("Move");
        moveAction.performed += OnMove;

        moveUpDownAction = inputActions.FindAction("MoveUpDown");
        moveUpDownAction.performed += OnMoveUpDown;

        lookAction = inputActions.FindAction("Look");
        lookAction.performed += OnLook;


        rollAction = inputActions.FindAction("Roll");
        rollAction.performed += OnRoll;

        shootAction = inputActions.FindAction("Shoot");
        shootAction.performed += OnShoot;

        shootMissileAction = inputActions.FindAction("ShootMissile");
        shootMissileAction.performed += OnShootMissile;


        moveAction.Enable();
        lookAction.Enable();
        rollAction.Enable();
        shootAction.Enable();
        shootMissileAction.Enable();
        moveUpDownAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        moveAction.performed -= OnMove;
        lookAction.performed -= OnLook;
        rollAction.performed -= OnRoll;
        shootAction.performed -= OnShoot;
        shootMissileAction.performed -= OnShootMissile;
        moveUpDownAction.performed -= OnMoveUpDown;


        moveAction.Disable();
        lookAction.Disable();
        rollAction.Disable();
        shootAction.Disable();
        shootMissileAction.Disable();
        moveUpDownAction.Disable();
    }

    public Vector2 GetMovement()
    {
        return moveAction.ReadValue<Vector2>();
    }

    public float GetMovementUpDown()
    {
        return moveUpDownAction.ReadValue<float>();
    }

    public float GetRoll()
    {
        return rollAction.ReadValue<float>();
    }

    public Vector2 GetMouseLook()
    {
        Vector2 mousePos = inputActions.PlayerThird.Look.ReadValue<Vector2>();
        return mousePos;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
    }

    private void OnRoll(InputAction.CallbackContext context)
    {
        float rollInput = context.ReadValue<float>();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveUpDown(InputAction.CallbackContext context)
    {
        float moveUpDownInput = context.ReadValue<float>();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        bool shootInput = context.ReadValueAsButton();
    }

    private void OnShootMissile(InputAction.CallbackContext context)
    {
        bool shootInput = context.ReadValueAsButton();
    }

    public float GetShoot()
    {
        return shootAction.ReadValue<float>();

    }

    public bool GetShootMissile()
    {
        return shootMissileAction.ReadValue<float>() > 0.5f;

    }

    //public bool GetShoot()
    //{
    //    return shootAction.ReadValue<bool>();
    //}

    //public bool GetShootMissile()
    //{
    //    return shootMissileAction.ReadValue<bool>();
    //}

}
