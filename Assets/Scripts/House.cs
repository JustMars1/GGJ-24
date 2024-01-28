using System.Collections;
using UnityEngine;

public class House : MonoBehaviour
{
    public int residentsAmount;
    public int maxResidents;
    public float force;
    public GameObject normalCivilPrefab;
    public GameObject bigCivilPrefab;
    GameObject doorCollider;
    public float spawnRadius = 10f;
    public int destroyableThreshold;

    // Start is called before the first frame update
    void Start()
    {
        residentsAmount = 0;
        doorCollider = transform.Find("DoorCollider").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(doorCollider && residentsAmount == maxResidents)
        {
            DestroyDoorCollider();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && CheckThreshold())
        {
            Debug.Log("Player hit trigger");
            DestroyDoorCollider();
            Rigidbody[] rbList = gameObject.GetComponentsInChildren<Rigidbody>();
            Vector3 playerDirection = other.transform.forward;

            foreach (Rigidbody rb in rbList)
            {
                rb.isKinematic = false;
                Vector3 forceDirection = new Vector3(playerDirection.x, 0.0f, playerDirection.z);
                rb.AddForce(Vector3.Normalize(forceDirection) * force, ForceMode.Impulse);
            }

            Transform[] childList = GetChildren();
            foreach(Transform child in childList)
            {
                // Triggering the particle systems
                if(child.name == "particleBuildingCollapse")
                {
                    int children = child.childCount;

                    for (int i = 0; i < children; ++i)
                    {
                        if(child.GetChild(i).GetComponent<ParticleSystem>())
                        {
                            child.GetChild(i).GetComponent<ParticleSystem>().Play();
                        }
                    }
                }

                // Start shrinking and fading out the child gameobjects
                StartCoroutine(ShrinkAndFadeOut(child));
            }

            StartCoroutine(SpawnHumans());
        }
    }

    IEnumerator ShrinkAndFadeOut(Transform child)
    {
        if(child.GetComponent<Renderer>() != null)
        {
            float duration = 4f; // Total duration for the shrinking and fading animation

            Debug.Log("Coroutine started");
            
            // Initial values
            Vector3 initialScale = child.localScale;
            Color initialColor = child.GetComponent<Renderer>().material.color;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Interpolate scale
                child.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / duration);

                // Interpolate alpha
                Color newColor = initialColor;
                newColor.a = Mathf.Lerp(initialColor.a, 0f, elapsedTime / duration);
                child.GetComponent<Renderer>().material.color = newColor;

                //Debug.Log("Scale: " + child.localScale);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }

    void DestroyDoorCollider()
    {
        if (doorCollider != null)
        {
            Debug.Log("Found door collider");
            Destroy(doorCollider);
        }
    }

    Transform[] GetChildren()
    {
        int children = transform.childCount;
        Transform[] childrenList = new Transform[children];

        for (int i = 0; i < children; ++i)
        {
            childrenList[i] = transform.GetChild(i);
        }

        return childrenList;
    }

    IEnumerator SpawnHumans()
    {
        for (int i = 0; i < residentsAmount; i++)
        {
            float randomValue = Random.value;
            float normalCivilProbability = 0.7f;
            float bigCivilProbability = 1.0f - normalCivilProbability;
            GameObject selectedPrefab = randomValue < normalCivilProbability ? normalCivilPrefab : bigCivilPrefab;

            GameObject newHuman = Instantiate(selectedPrefab, transform.position, Quaternion.identity);
            yield return null;
        }
    }

    public bool CheckThreshold()
    {
        return destroyableThreshold < GameManager.manager.score;
    }
}
