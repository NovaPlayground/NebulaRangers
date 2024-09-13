using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    [SerializeField] private Color hitColor = Color.red; // Color when the shield is hit
    [SerializeField] private float emissionDuration = 0.2f; // How long the color lasts after being hit

    private float shieldHealth;
    private bool isShieldActive;
    private Material shieldMaterial; // Material of the shield
    private Color originalColor; // Original emission color of the shield
    private float hitTime; // Timer to track how long the shield has been hit


    private void Start()
    {
        // Get the material from the shield object
        if (shield != null)
        {
            shieldMaterial = shield.GetComponent<Renderer>().material;

            // Enable emission if it is not enabled
            if (!shieldMaterial.IsKeywordEnabled("_EMISSION"))
            {
                shieldMaterial.EnableKeyword("_EMISSION");
                Debug.Log("Enabled emission on shield material.");
            }

            originalColor = shieldMaterial.GetColor("_EmissionColor");
        }
    }

    public void Initialize(float maxHealth)
    {
        shieldHealth = maxHealth;
        shield.SetActive(false); // set shield
    }

    public void InitializeEnemy(float maxHealth)
    {
        shieldHealth = maxHealth;
    }

    public void Activate()
    {
        isShieldActive = true;
        shield.SetActive(true); // Shows the visual appearance of the shield
    }

    public void Deactivate()
    {
        isShieldActive = false;
        shield.SetActive(false); // Hide the shield
    }

    // Absorb damage using the shield
    public void AbsorbDamage(float damage)
    {
        shieldHealth -= damage;

        // Trigger the hit effect
        hitTime = emissionDuration; // Reset the hit timer to the duration

        if (shieldHealth <= 0f)
        {
            Deactivate();
        }
    }

    private void Update()
    {
        // If the shield has been hit, gradually revert the emission color
        if (hitTime > 0)
        {
            hitTime -= Time.deltaTime;

            // Lerp between hitColor and originalColor based on the remaining hit time
            Color currentColor = Color.Lerp(originalColor, hitColor * Mathf.LinearToGammaSpace(2f), hitTime / emissionDuration);
            shieldMaterial.SetColor("_EmissionColor", currentColor);
        }
        else
        {
            // Ensure the shield returns to its original color when not hit
            shieldMaterial.SetColor("_EmissionColor", originalColor);
        }
    }

    // Check if the barrier is active
    public bool IsShieldActive()
    {
        return isShieldActive;
    }


}
