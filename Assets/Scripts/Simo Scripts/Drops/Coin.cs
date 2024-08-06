using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour,IPickable
{
    [SerializeField]private float rotationSpeed = 12f;

    private int coinValue = 1;

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
        //Destroy Coin
        Destroy(gameObject);
    }


    public int Value
    {
        get { return coinValue; }
    }

   
}
    


