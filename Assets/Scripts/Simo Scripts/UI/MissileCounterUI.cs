using UnityEngine;
using UnityEngine.UI;

public class MissileCounterUI : MonoBehaviour
{
    [SerializeField] private GameObject[] missileCounter;
    [SerializeField] private PlayerThird player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerThird>();    
    }

    // Update is called once per frame
    void Update()
    {      
        UpdateMissileCounter();
    }



    private void UpdateMissileCounter()
    {
        float missileCooldown = player.GetMissileCooldown();

        
        if (missileCooldown >= 0f && missileCooldown <= 6f)
        {
            // Calculate the index of the corresponding image
            int index = Mathf.FloorToInt(missileCooldown); //Round down

           
            SetActiveCounter(6 - index); // 6 is the initial value, it decreases to 0
        }
        else
        {
            SetActiveCounter(-1); // Hide images if the value is out of range
        }
    }

    void SetActiveCounter(int index)
    {
        // Deactivate all children and activate the correct one
        for (int i = 0; i < missileCounter.Length; i++)
        {
            missileCounter[i].SetActive(i == index);
        }
    }
}
