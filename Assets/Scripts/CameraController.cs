using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = player.transform.position + player.transform.TransformDirection(offset);
        transform.position = cameraPos;

        transform.rotation = player.transform.rotation;
    }
}
