using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject mine;
    private float currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        currentRotation = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentRotation += Time.fixedDeltaTime;

        transform.Rotate(Vector3.up, currentRotation);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            PlayerTop player = collision.collider.GetComponent<PlayerTop>();

            explosionVFX.gameObject.SetActive(true);
            mine.gameObject.SetActive(false);

            player.TakeDamage();
        }
    }
}
