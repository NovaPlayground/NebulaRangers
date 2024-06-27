using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviourFollowMouse : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float maxCameraDistance;
    [SerializeField] private float cameraDistance;

    private PlayerControllerTop playerController;
    private Camera mainCamera;
    private Vector3 targetPoint;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.transform.position + offset;
        transform.rotation = rotation;
        targetPoint = transform.position + offset;

        mainCamera = GetComponent<Camera>();
        playerController = GetComponent<PlayerControllerTop>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        //WIP DO THE CAMERA MOVEMENT
        Vector2 mouseScreenPos = playerController.GetMousePosition2D();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.transform.position.y));

        Vector3 directionToMouse = mouseWorldPos - new Vector3(transform.position.x, 0.0f, transform.position.z);
        directionToMouse.Normalize();

        float distance = Vector3.Distance(new Vector3(mouseWorldPos.x, 0.0f, mouseWorldPos.z), new Vector3(target.transform.position.x, 0.0f, target.transform.position.z));

        if (distance > maxCameraDistance)
        {
            targetPoint = target.transform.position + directionToMouse * cameraDistance + offset;
        }
        else
        {
            targetPoint = target.transform.position + offset;
        }

        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.fixedDeltaTime);
    }
}
