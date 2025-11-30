using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    public GameObject obstaclePrefabs;

    [Header("Spawn Settings")]
    public float spawnTime = 2f;
    private float timer;

    [Header("Obstacle Lifetime")]
    public float lifeTime = 5f;     // Lama obstacle bertahan

    void Start()
    {
        timer = spawnTime;
    }

    void Update()
    {
        Spawn();
    }

    void Spawn()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Spawn obstacle
            GameObject spawned = Instantiate(obstaclePrefabs, transform.position, transform.rotation);

            // Hapus obstacle setelah beberapa detik
            Destroy(spawned, lifeTime);

            // Reset timer
            timer = spawnTime;
        }
    }
}
