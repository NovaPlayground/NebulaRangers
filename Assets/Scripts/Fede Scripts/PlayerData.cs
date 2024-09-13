using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float Health;
    public int KeyCount;
    public PlayerData(float hp, int kc)
    {
        Health = hp;
        KeyCount = kc;
    }
}
