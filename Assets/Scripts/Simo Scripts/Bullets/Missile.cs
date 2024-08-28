using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.XR;

public class Missile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float lifeSpan;
    //[SerializeField] private float lockOnRange = 50f; // Target search radius

    //HomingAccuracy value:
    // Higher values (closer to 1) : the missile will quicly orient itself toward the target, following the path very precisely;
    // Lower values (closer to 0 ) :the missile will track the target with less precision, and its rotation toward the target will be slower and more gradual;
    [SerializeField] private float homingAccuracy = 0.95f; 
    
    private Transform target;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        damage = 100f;
        
    }


    private void FixedUpdate()
    {
       
        if (target != null) 
        {
            // IF U KEEP THE CODE LIKE THIS U CAN HAVE A "REMOTE-GUIDED" MISSILE
            //Vector3 directionToTarget = (target.position - transform.position).normalized;
            //transform.position += directionToTarget * speed * Time.fixedDeltaTime;
            //transform.LookAt(target); 


            // direction  to target
            Vector3 direction = (target.position - transform.position).normalized;

            // rotation to target
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Rotate the missile to target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, homingAccuracy * Time.fixedDeltaTime);

            // Move the missile towards its forward
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

        }
       

    }

    // Update is called once per frame
    void Update()
    {
        // Missile's lifetime 
        lifeSpan -= Time.deltaTime;

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
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

  
}
