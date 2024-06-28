using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offSet;
    [SerializeField] public float followSpeed = 10f;

    

    // Start is called before the first frame update
    void Start()
    {

    }

   
    void FixedUpdate()
    {
        
        Vector3 desiredPosition = player.position + player.rotation * offSet;

        // Lerp camera position 
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // keep the same rotation of the player
        transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation, followSpeed * Time.deltaTime);

    }
}
