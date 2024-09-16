using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public float GetHealth();
    public void SetHealth(float currentHealth);

    public int GetKeyCount();
    public void SetKeyCount(int currentKeyCount);

    public float GetNormalaizedHealth();
}
