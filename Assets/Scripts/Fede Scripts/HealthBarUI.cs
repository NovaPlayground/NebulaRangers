using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject player;

    private IPlayer playerInterface;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInterface = player.GetComponent<IPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = playerInterface.GetNormalaizedHealth();
    }
}
