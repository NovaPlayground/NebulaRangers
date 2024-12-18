using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerTop : MonoBehaviour
{
    private Controls inputActions;
    private InputAction moveAction;
    private InputAction normalShootAction;
    private InputAction flipAction;
    private InputAction interactAction;

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

        flipAction = inputActions.FindAction("Flip");
        flipAction.performed += OnFlip;

        interactAction = inputActions.FindAction("Interact");
        interactAction.performed += OnInteract;

        moveAction.Enable();
        normalShootAction.Enable();
        flipAction.Enable();
        interactAction.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();

        moveAction.performed -= OnMove;
        moveAction.Disable();

        normalShootAction.performed -= OnShoot;
        normalShootAction.Disable();

        flipAction.performed -= OnFlip;
        flipAction.Disable();

        interactAction.performed -= OnInteract;
        interactAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
    }

    private void OnShoot(InputAction.CallbackContext context) 
    {
        bool input = context.ReadValueAsButton();
    }

    private void OnFlip(InputAction.CallbackContext context)
    {
        bool input = context.ReadValueAsButton();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        bool input = context.ReadValueAsButton();
    }

    public float GetShoot()
    {
        return normalShootAction.ReadValue<float>();
    }

    public float GetFlip()
    {
        return flipAction.ReadValue<float>();
    }

    public Vector2 GetMovement()
    {
        return moveAction.ReadValue<Vector2>();
    }

    public float GetInteract()
    {
        return interactAction.ReadValue<float>();
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
