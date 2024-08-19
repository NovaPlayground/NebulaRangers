using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour,IPickable
{
    [SerializeField] private float rotationSpeed = 12f;

    private int keyValue = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUp(other.gameObject);
        }
    }

    public void PickUp(GameObject picker)
    {
        //Destroy Key
        Destroy(gameObject);
    }


    public int Value
    {
        get { return keyValue; }
    }
}
