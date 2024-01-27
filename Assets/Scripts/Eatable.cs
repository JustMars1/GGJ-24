using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Eatable : MonoBehaviour
{
    protected Animator animator;

    protected int eatThreshold;
    protected int scoreWorth;

    [SerializeField]
    protected string eatenAnimationName;

    protected virtual void Start()
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