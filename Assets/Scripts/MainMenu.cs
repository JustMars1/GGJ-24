using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [SerializeField] Button _playButton;
    public Button playButton => _playButton;
    
    [SerializeField] Button _settingsButton;
    public Button settingsButton => _settingsButton;
    
    [SerializeField] Button _quitButton;
    public Button quitButton => _quitButton;

    public override void Open()
    {
        menu.gameObject.SetActive(true);
    }

    public override void Close()
    {
        menu.gameObject.SetActive(false);
    }
}
