using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTop : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;

    private PlayerControllerTop playerController;
    private Rigidbody rb;

    void Start()
    {
        playerController = GetComponent<PlayerControllerTop>();
        rb = GetComponent<Rigidbody>();

        mainCamera.transform.position = transform.position + cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        LookAtMouse();
        Move();
    }

    private void LookAtMouse()
    {
        Vector2 mouseScreenPos = playerController.GetMousePosition2D();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.transform.position.y));

        if (IsNearThan(mouseWorldPos, transform.position, 2.0f)) return;

        Vector3 directionToMouse = mouseWorldPos - transform.position;
        directionToMouse.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private bool IsNearThan(Vector3 a, Vector3 b, float distance)
    {
        return (b - a).magnitude <= distance;
    }

    private void Move()
    {
        float input = playerController.GetMovement();

        Vector3 movementDirection = new Vector3(0f, 0f, input);
        movementDirection.Normalize();

        rb.MovePosition(rb.position + input * transform.forward * speed * Time.fixedDeltaTime);
    }
}
