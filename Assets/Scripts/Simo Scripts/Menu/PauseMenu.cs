using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (GameIsPaused) 
            {
                Resume();
            }
            else 
            {
                Pause();
            }
        }
    }

    public void Resume() 
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 1 the game run normal. 
        GameIsPaused = false;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 0 freeze the game. 
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
