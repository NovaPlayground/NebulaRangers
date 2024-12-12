using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    public void LoadMainMenu() 
    {
        SceneManager.LoadScene("MainMenu");
    }
}
