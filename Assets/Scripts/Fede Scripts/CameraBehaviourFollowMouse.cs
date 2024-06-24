using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviourFollowMouse : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float maxCameraDistance;

    private PlayerControllerTop playerController;
    private Camera mainCamera;
    private Vector3 targetPoint;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.transform.position + offset;
        transform.rotation = rotation;

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

        Vector3 newPos = Vector3.Lerp(transform.position, mouseWorldPos + offset, Time.fixedDeltaTime * 0.5f);

        if (Vector3.Distance(mouseWorldPos + offset, transform.position) > 1)
        {
            transform.position = newPos;
        }
    }
}
