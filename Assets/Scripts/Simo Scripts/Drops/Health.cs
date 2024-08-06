using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IPickable
{
    [SerializeField] private int healthAmount = 25;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 startPosition;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        time += moveSpeed * Time.deltaTime;
        float yOffSett = Mathf.Sin(time) * moveDistance;

        transform.position = startPosition + new Vector3(0, yOffSett, 0);
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
        
        Destroy(gameObject);
    }

    public int Value
    {
        get { return healthAmount; }
    }
}
