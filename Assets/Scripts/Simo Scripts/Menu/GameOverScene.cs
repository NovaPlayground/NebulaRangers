using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string endGameMenuSceneName = "EndGameMenu";

    public void TriggerGameOver() 
    {
        SceneManager.LoadScene(endGameMenuSceneName);
    }

    public void Restart() 
    {
        // Reloads the current scene
       // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu() 
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
