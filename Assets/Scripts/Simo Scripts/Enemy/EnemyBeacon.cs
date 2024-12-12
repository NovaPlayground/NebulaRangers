using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeacon : MonoBehaviour
{
    [SerializeField] private Light beaconLight; 
    [SerializeField] private Transform player; 
    [SerializeField] private float flickerRate = 2f; // Flicker speed
    [SerializeField] private float detectionRange = 10f; // Distance at which the light will turn on or off
    [SerializeField] private float lightIntensityOn = 1f; 
    [SerializeField] private float lightIntensityOff = 0f; 
    [SerializeField] private float blinkRate = 0.5f; // Frequency of the blink (seconds per blink)
    private float nextBlinkTime = 0f; // Time for the next blink
    private bool isLightOn = false; // Stato della luce (accesa o spenta)

    void Start()
    {
        
        if (beaconLight == null)
        {
            beaconLight = GetComponentInChildren<Light>(); 
        }

        if (player == null)
        {
            player = GameObject.Find("PlayerThird 1 1").transform; 
        }

       
    }

    void Update()
    {
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
       
        if (distanceToPlayer >= detectionRange)
        {
            // Check if it's time to change the state of the light
            if (Time.time >= nextBlinkTime)
            {
                // Change the state of the light 
                isLightOn = !isLightOn;

                // Set the light intensity based on the state
                beaconLight.intensity = isLightOn ? lightIntensityOn : lightIntensityOff;

                
                nextBlinkTime = Time.time + blinkRate;
            }
        }
        else
        {
            // If the player is within range, turn off the light
            beaconLight.intensity = lightIntensityOff;
        }
    }
}
