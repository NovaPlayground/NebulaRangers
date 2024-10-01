using UnityEngine;

public class Key : MonoBehaviour ,IPickable
{
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private int keyIndex; // Used to identify the key level
    [SerializeField] private GameObject portalToSpawn;

    private void Start()
    {
        if (keyIndex < 1 || keyIndex > 4)
        {
            keyIndex = 1; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 pos = new Vector3(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
            Instantiate(portalToSpawn, pos, Quaternion.identity);
            PickUp(other.gameObject);
        }
    }

    public void PickUp(GameObject picker)
    {
        Destroy(gameObject);
    }


    public int Value
    {
        get { return keyIndex; }
    }
}
