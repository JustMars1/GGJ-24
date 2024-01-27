using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanSpawner : MonoBehaviour
{
    public GameObject humanPrefab;
    public int numberOfHumans = 10;
    public float spawnRadius = 10f;

    void Start()
    {
        StartCoroutine(SpawnHumans());
    }

    IEnumerator SpawnHumans()
    {
        for (int i = 0; i < numberOfHumans; i++)
        {
            Vector3 randomPosition = GetRandomPosition();

            // Check if the position is valid
            if (!float.IsInfinity(randomPosition.x))
            {
                GameObject newHuman = Instantiate(humanPrefab, randomPosition, Quaternion.identity);

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