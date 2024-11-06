using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalThird : MonoBehaviour
{
    [SerializeField] private int nextSceneIndex;
    [SerializeField] private bool isPlayerInRange;
    private PlayerControllerThird playerControllerThird;

    private void Start()
    {
        nextSceneIndex = GameManager.Instance.GetNextSceneIndex();
        Debug.Log("portalSpawn");
    }

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
        SceneManager.LoadScene(nextSceneIndex);
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
