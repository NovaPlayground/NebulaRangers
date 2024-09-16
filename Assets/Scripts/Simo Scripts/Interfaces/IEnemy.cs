using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy 
{
    float GetHealth();
    float GetMaxHealth();

    void SetHealth(float health);

    Rigidbody GetRigidbody();


}
