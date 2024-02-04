using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    [SerializeField] RectTransform _menu;
    public RectTransform menu => _menu;

    [SerializeField] Selectable _defaultSelectabe;
    public Selectable defaultSelectable => _defaultSelectabe;
    
    public abstract void Open();
    public abstract void Close();
}
