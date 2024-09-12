using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private string sceneToLoad;
    
    private bool isPlayerInRange;
    private PlayerControllerTop playerControllerTop;

    void Update()
    {
        if (isPlayerInRange && playerControllerTop.GetInteract() > 0)
        {
            Interact();
        }    
    }

    public void Interact()
    { 
        IPlayer player = playerControllerTop.GetComponent<IPlayer>();
        GameManager.Instance.SavePlayerData(player.GetHealth(), player.GetKeyCount());
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerControllerTop = other.GetComponent<PlayerControllerTop>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerControllerTop = null;
        }
    }
}
