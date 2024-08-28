using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
    public void PickUp(GameObject picker);
    int Value { get; }
}
