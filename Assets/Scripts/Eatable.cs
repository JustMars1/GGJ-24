using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Eatable : MonoBehaviour
{
    public int eatThreshold;

    public bool CheckThreshold()
    {
        return eatThreshold > GameManager.manager.score;
    }

    public abstract void Consume();
}
