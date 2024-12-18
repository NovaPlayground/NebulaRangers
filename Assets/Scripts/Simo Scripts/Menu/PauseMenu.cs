using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Cursor.visible = true;
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
        Cursor.visible = false;
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 1 the game run normal. 
        GameIsPaused = false;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 0 freeze the game. 
        GameIsPaused = true;
        Cursor.visible = true; //ADDED CURSOR VISIBLE
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ResetNextSceneIndex(); // reset scene
        //SceneManager.LoadScene(0);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
