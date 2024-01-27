using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Eatable : MonoBehaviour
{
    protected int eatThreshold;
    protected int scoreWorth;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected string eatenAnimationName;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected bool CheckThreshold()
    {
        return eatThreshold > GameManager.manager.score;
    }

    protected virtual void PlayAnimation()
    {
        animator.Play(eatenAnimationName);
    }

    public virtual void Consume()
    {
        PlayAnimation();
        GameManager.manager.AddScore(scoreWorth);
    }
}