using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{

    [SerializeField] private Image crosshair;  

    // Start is called before the first frame update
    void Start()
    {
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTarget(Transform target) 
    {
        if (crosshair != null) 
        {
            crosshair.gameObject.SetActive(true);

            // Convert target position to screen coordinates and update crosshair position
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
            //crosshair.rectTransform.position = screenPosition;

            // Check if the position is valid (in front of the camera)
            if (screenPosition.z > 0)
            {
                
                crosshair.gameObject.SetActive(true);
                crosshair.rectTransform.position = screenPosition;
            }
            else
            {
                // Hide the crosshair if the target is behind the camera
                ClearTarget();
            }

        }
    }

    
    public void ClearTarget()
    {
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(false);
        }
    }
}
