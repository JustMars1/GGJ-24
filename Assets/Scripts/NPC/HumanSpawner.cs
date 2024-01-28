using System.Collections;
using System.Collections.Generic;
using NPC;
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

    [SerializeField ]List<ColorPalette> _palettes = new List<ColorPalette>();
    [SerializeField ]List<BigColorPalette> _bigPalettes = new List<BigColorPalette>();

    private GameManager manager;
    private int lastSpawnedScore;
    private bool enemiesSpawnedForCurrentInterval;
    static readonly int _baseColor = Shader.PropertyToID("_BaseColor");
    static readonly int _index = Shader.PropertyToID("_index");

    void Start()
    {
        manager = GameManager.manager; // Assuming GameManager is a singleton or has a static reference
        lastSpawnedScore = manager.GetScore();
        enemiesSpawnedForCurrentInterval = false;
        StartCoroutine(SpawnHumans());
    }

    void Update()
    {
        int currentScore = manager.GetScore();

        // Check if the score has crossed a 10-score interval and enemies haven't been spawned for the current interval
        if (currentScore > lastSpawnedScore && (currentScore / 10 > lastSpawnedScore / 10) && !enemiesSpawnedForCurrentInterval)
        {
            // Spawn enemies based on the score difference
            int numberOfEnemiesToSpawn = 5; // 5 enemies per 10 score

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                Vector3 randomPosition = GetRandomPosition();

                // Check if the position is valid
                if (!float.IsInfinity(randomPosition.x))
                {
                    GameObject newEnemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
                    //Debug how many enemies are spawned and what is the current score
                    Debug.Log("Enemy spawned at " + randomPosition + " with score " + currentScore);
                    
                }
            }

            lastSpawnedScore = currentScore;
            enemiesSpawnedForCurrentInterval = true; // Set the flag to true to indicate enemies have been spawned for the current interval
        }

        // Reset the flag if the score goes back to a previous 50-score interval
        if (currentScore / 10 > lastSpawnedScore / 10)
        {
            enemiesSpawnedForCurrentInterval = false;
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
                
                Renderer renderer = newNormalCivil.GetComponentInChildren<Renderer>(true);

                ColorPalette palette = _palettes[Random.Range(0, _palettes.Count)];
                
                renderer.materials[0].SetColor(_baseColor, palette.coat);
                renderer.materials[1].SetColor(_baseColor, palette.boots);
                renderer.materials[2].SetColor(_baseColor, palette.neckThing);
                renderer.materials[3].SetColor(_baseColor, palette.pants);
                renderer.materials[4].SetColor(_baseColor, palette.gloves);
                
                renderer.materials[5].SetInt(_index, Random.Range(0, 9));
                
                renderer.materials[6].SetColor(_baseColor, palette.hat);
                renderer.materials[6].SetColor(_baseColor, palette.scarf);
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
                
                Renderer renderer = newBigCivil.GetComponentInChildren<Renderer>(true);
                
                BigColorPalette palette = _bigPalettes[Random.Range(0, _bigPalettes.Count)];
                
                renderer.materials[0].SetInt(_index, Random.Range(0, 4));
                renderer.materials[1].SetColor(_baseColor, palette.coat);
                renderer.materials[2].SetColor(_baseColor, palette.pants);
                renderer.materials[3].SetColor(_baseColor, palette.neckThing);
                renderer.materials[4].SetColor(_baseColor, palette.gloves);
                renderer.materials[5].SetColor(_baseColor, palette.boots);
                
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