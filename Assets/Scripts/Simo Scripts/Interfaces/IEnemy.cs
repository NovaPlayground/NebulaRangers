using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy 
{
    public float GetHealth();

    public float GetMaxHealth();

    public void SetHealth(float health);

    public Rigidbody GetRigidbody();

    public void SetCanvasActive(bool isActive);
}
