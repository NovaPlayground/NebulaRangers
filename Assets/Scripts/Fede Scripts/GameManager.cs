using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private Vector3 startingPosition;

    public static GameManager Instance;

    private PlayerData playerData;
    private IPlayer player;
    
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
        player = GameObject.FindWithTag("Player").GetComponent<IPlayer>();

        if (player != null)
        {
            LoadPlayerData();
        }
    }

    private void LoadPlayerData()
    {
        if (playerData != null)
        {
            player.SetHealth(playerData.Health);
            player.SetKeyCount(playerData.KeyCount);
        }
    }

    public void SavePlayerData(float currentHealth, int keyCount)
    {
        playerData = new PlayerData(currentHealth, keyCount);
        Debug.Log("PlayerData created health: " + currentHealth + " --- keycount : " + keyCount);
    }

    public void ResetScene()
    {
        //player.transform.position= startingPosition;
    }
}
