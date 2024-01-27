using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class HumanSpawner : MonoBehaviour
{
    public GameObject normalCivilPrefab;
    public GameObject bigCivilPrefab;
    public GameObject enemyPrefab;
    public int numberOfNormalCivil = 10;
    public int numberOfBigCivil = 5;
    public float spawnRadius = 10f;

    private GameManager manager;
    private int lastSpawnedScore;

    void Start()
    {
        manager = GameManager.manager; // Assuming GameManager is a singleton or has a static reference
        lastSpawnedScore = manager.GetScore();
        StartCoroutine(SpawnHumans());
    }

    void Update()
    {
        int currentScore = manager.GetScore();

        // Check if the score has crossed a threshold since the last time enemies were spawned
        if (currentScore > lastSpawnedScore)
        {
            // Spawn enemies based on the score difference
            int scoreDifference = currentScore - lastSpawnedScore;
            int numberOfEnemiesToSpawn = (int)(currentScore / 50) * 10; // 10 enemies per 50 score

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                Vector3 randomPosition = GetRandomPosition();

                // Check if the position is valid
                if (!float.IsInfinity(randomPosition.x))
                {
                    GameObject newEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
                }
            }

            lastSpawnedScore = currentScore;
        }
    }

    IEnumerator SpawnHumans()
    {
        // Spawn normal civilians
        for (int i = 0; i < numberOfNormalCivil; i++)
        {
            Vector3 randomPosition = GetRandomPosition();

            // Check if the position is valid
            if (!float.IsInfinity(randomPosition.x))
            {
                GameObject newNormalCivil = Instantiate(normalCivilPrefab, randomPosition, Quaternion.identity);
            }

            yield return null;
        }

        // Spawn big civilians
        for (int i = 0; i < numberOfBigCivil; i++)
        {
            Vector3 randomPosition = GetRandomPosition();

            // Check if the position is valid
            if (!float.IsInfinity(randomPosition.x))
            {
                GameObject newBigCivil = Instantiate(bigCivilPrefab, randomPosition, Quaternion.identity);
            }

            yield return null;
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, spawnRadius, 1))
        {
            return hit.position;
        }

        // Return a position indicating failure (you can customize this)
        return new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }
}