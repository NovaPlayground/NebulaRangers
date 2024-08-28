using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomb : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5;
    [SerializeField] private Transform player;
    [SerializeField] public int Damage = 1;

    private bool isPlayerInRange = false;
    


    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRadius) 
        {
            if (!isPlayerInRange) 
            {
                isPlayerInRange = true;
                OnPlayerEnterRange();
            }
        }
       
    }



    public void OnPlayerEnterRange()
    {
        player.GetComponent<PlayerThird>().TakeDamage(1);
        Debug.Log("Player entered in the radius and took damage.");
    }
}



