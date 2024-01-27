using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Needs a collider to be able to be eaten.
/// </summary>
[RequireComponent(typeof(Animator))]
public abstract class Eatable : MonoBehaviour
{
    protected Animator animator;

    public int scoreWorth;
    [SerializeField]
    protected int eatThreshold;

    [SerializeField]
    [Tooltip("The length in seconds how long the eating animation lasts.")]
    protected float eatDuration;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool CheckThreshold()
    {
        return eatThreshold <= GameManager.manager.score;
    }

    protected virtual void PlayAnimation(Transform eatPoint)
    {
        StartCoroutine(GetEaten(eatPoint));
    }

    public virtual void Consume(Transform eatPoint)
    {
        PlayAnimation(eatPoint);
        GameManager.manager.AddScore(scoreWorth);
    }

    IEnumerator GetEaten(Transform eatPoint)
    {
        float elapsedtime = 0;
        while(elapsedtime < eatDuration)
        {
            transform.position = Vector3.Lerp(transform.position, eatPoint.position, elapsedtime / eatDuration);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, elapsedtime / eatDuration);

            elapsedtime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}