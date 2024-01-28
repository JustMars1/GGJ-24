using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    [SerializeField] RectTransform _menu;
    public RectTransform menu => _menu;
    public abstract void Open();
    public abstract void Close();
}
