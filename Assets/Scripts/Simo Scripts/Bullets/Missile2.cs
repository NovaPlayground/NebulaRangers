using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile2 : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float damage = 100f;
    [SerializeField] private float lifeSpan = 5f;       // Missile's life before destruction
    [SerializeField] private float turnRate = 5f;       // How fast the missile turns towards its target
    [SerializeField] private float proximityThreshold = 1f; // How close the missile needs to get to the target before hitting
    [SerializeField] private float missileRange = 400f;    // Max range before self-destruction

    private Transform target;  // Target the missile will home in on
    private Rigidbody rb;      

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (target == null)
        {
            
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            TrackTarget();   // Method to track and steer toward the target
        }

        lifeSpan -= Time.deltaTime;
        if (lifeSpan <= 0f)
        {
            
            Destroy(gameObject);   
        }
    }

    private void TrackTarget()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        // Rotate missile towards target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, turnRate * Time.fixedDeltaTime);

        // Move missile forward
        transform.position += transform.forward * speed * Time.fixedDeltaTime;

        // Check if missile is close enough to hit the target
        if (distanceToTarget < proximityThreshold)
        {
            HitTarget();
        }

        // Check if missile exceeds max range
        if (distanceToTarget > missileRange)
        {
            Debug.Log("Missile exceeded its range.");
            Destroy(gameObject);
        }
    }

    private void HitTarget()
    {
        // Apply damage if target has an IDamageable component
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            
        }

        // Destroy the missile on impact
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform == target)
        {
            
            HitTarget(); // Apply damage and destroy the missile if it hits the target
        }
        else
        {
            
            Destroy(gameObject); // Destroy missile if it hits anything else
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
    }
}
