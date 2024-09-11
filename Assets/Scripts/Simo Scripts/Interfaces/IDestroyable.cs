using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyable 
{
    event System.Action<GameObject> OnDestroyed;
}
