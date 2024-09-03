using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float health;

    public PlayerData(float hp)
    {
        health = hp;
    }
}
