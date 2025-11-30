using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private int scoreValue = 100;

    [Header("SFX")]
    [SerializeField] private int sfxIndex = 0; 

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Rotasi
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // Efek naik-turun
        float bob = Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position = startPos + new Vector3(0, bob, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerLuringJoystick player = other.GetComponent<PlayerLuringJoystick>();
        if (player != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);

            // Mainkan SFX
            SoundManager.Instance.PlaySFX(sfxIndex);
            Debug.Log("Coin picked, playing SFX...");


            Destroy(gameObject);
        }
    }
}
