using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerData PlayerData;

    private GameObject player;
    
    private GameManager() { }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded+= OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded-= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            LoadPlayerData();
        }
    }

    private void LoadPlayerData()
    {
        if (PlayerData != null)
        {
            //player.SetCurrentHealth(PlayerData.health);
        }
    }

    public void SavePlayerData(float currentHealth)
    {
        PlayerData = new PlayerData(currentHealth);
        Debug.Log("PlayerData created health: " + currentHealth);
    }
}
