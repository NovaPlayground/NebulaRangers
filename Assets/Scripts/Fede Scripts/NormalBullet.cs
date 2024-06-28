using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : MonoBehaviour
{
    [SerializeField] private float lifeSpan;
    [SerializeField] private float speed;
    //[SerializeField] public int damage;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.forward * Time.fixedDeltaTime * speed);
        lifeSpan -= Time.fixedDeltaTime;

        if (lifeSpan < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerThird player = collision.gameObject.GetComponent<PlayerThird>();

        if (player != null)
        {
            player.TakeDamage(1);
        }

        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
