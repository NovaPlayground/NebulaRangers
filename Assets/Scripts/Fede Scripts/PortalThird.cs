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
        GameManager.Instance.SavePlayerData(32.0f);
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
