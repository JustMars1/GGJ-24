using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    public TMP_Text scoreText => _scoreText;
    
    [SerializeField] Image _hungerMeter;
    public Image hungerMeter => _hungerMeter;
}
