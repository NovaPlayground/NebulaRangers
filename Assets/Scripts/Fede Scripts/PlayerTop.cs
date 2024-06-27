using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTop : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private NormalBullet normalBullet;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject[] muzzles;

    private PlayerControllerTop playerController;
    private Rigidbody rb;
    private float shootCooldown;

    void Start()
    {
        playerController = GetComponent<PlayerControllerTop>();
        rb = GetComponent<Rigidbody>();
        shootCooldown = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        LookAtMouse();
        Move();
        Shoot();
    }

    private void LookAtMouse()
    {
        Vector2 mouseScreenPos = playerController.GetMousePosition2D();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.transform.position.y));

        if (IsNearThan(mouseWorldPos, transform.position, 0.5f)) return;

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
        Vector2 input = playerController.GetMovement();

        Vector3 movementDirection = new Vector3(input.x, 0f, input.y);
        movementDirection.Normalize();


        Vector3 movement = movementDirection.x * transform.right + movementDirection.z * transform.forward;

        rb.MovePosition(rb.position + movement *  speed * Time.fixedDeltaTime);
    }

    private void Shoot()
    {
        if (playerController.GetShoot() > 0)
        {
            if (shootCooldown >= shootDelay)
            {
                for (int i = 0; i < muzzles.Length; i++)
                {
                    Instantiate(normalBullet, muzzles[i].transform.position, transform.rotation);
                }

                shootCooldown = 0;
            }
            else
            {
                shootCooldown += Time.fixedDeltaTime;
            }
        }
        else
        {
            shootCooldown = shootDelay;
        }
    }
}
