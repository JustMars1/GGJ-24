using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Menu
{
    [SerializeField] Button _exitToMainMenuButton;
    public Button exitToMainMenuButton => _exitToMainMenuButton;

    [SerializeField] TMP_Text _finalScoreText;
    
    public override void Open()
    {
        menu.gameObject.SetActive(true);
        _finalScoreText.text = GameManager.manager.score.ToString();
    }

    public override void Close()
    {
        menu.gameObject.SetActive(false);
    }
}
