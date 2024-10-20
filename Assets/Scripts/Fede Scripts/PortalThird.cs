using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalThird : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    [SerializeField]private bool isPlayerInRange;
    private PlayerControllerThird playerControllerThird;

    void Update()
    {
        if (isPlayerInRange && playerControllerThird.GetInteract() > 0)
        {
            Interact();
        }
    }

    public void Interact()
    {
        IPlayer player = playerControllerThird.GetComponent<IPlayer>();
        GameManager.Instance.SavePlayerData(player.GetHealth(), player.GetKeyCount());
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerControllerThird = other.GetComponent<PlayerControllerThird>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerControllerThird = null;
        }
    }
}
