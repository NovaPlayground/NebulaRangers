using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // this class manage the damage 
    // check what object contains the interface IDamageable, if the object contain it, call the function to take damage on them 

    private void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if(damageable != null) 
        {
            damageable.TakeDamage(1.0f);
        }
    }
}
