using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugNormalBullet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float lifeSpan;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        damage = 25f; 
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
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);  // Destroy the bullet after hitting the target
        }
    }

    public void SetDamage(float newDamage) 
    {
        damage = newDamage;
    }




}
