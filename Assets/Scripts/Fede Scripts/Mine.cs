using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject mine;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float rotationSpeed;


    // Start is called before the first frame update
    void Awake()
    {
        rotationSpeed = Random.Range(100.0f, 250.0f);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerTop player = collision.collider.GetComponent<PlayerTop>();
            
            Instantiate(explosionVFX, gameObject.transform.position, transform.rotation);
            player.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
