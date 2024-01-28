using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
    [SerializeField] Button _resumeButton;
    public Button resumeButton => _resumeButton;

    [SerializeField] Button _settingsButton;
    public Button settingsButton => _settingsButton;
    
    [SerializeField] Button _exitToMainMenuButton;
    public Button exitToMainMenuButton => _exitToMainMenuButton;

    public override void Open()
    {
        menu.gameObject.SetActive(true);
    }

    public override void Close()
    {
        menu.gameObject.SetActive(false);
    }
}
