using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    [SerializeField] private int nextSceneIndex;

    private bool isPlayerInRange;
    private PlayerControllerTop playerControllerTop;
    private void Start()
    {
        nextSceneIndex = GameManager.Instance.GetNextSceneIndex();

        if (nextSceneIndex <= 0 || nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            
            nextSceneIndex = 2; // default value / scene
        }
    }

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
        SceneManager.LoadScene(nextSceneIndex);
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
