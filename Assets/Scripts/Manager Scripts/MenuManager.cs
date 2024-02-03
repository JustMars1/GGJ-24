using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour
{
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] SettingsMenu _settingsMenu;
    [SerializeField] PauseMenu _pauseMenu;
    [SerializeField] GameOverMenu _gameOverMenu;
    
    [SerializeField] GameplayUI _gameplayUI;
    public GameplayUI gameplayUI => _gameplayUI;

    [SerializeField] Button _backButton;

    [SerializeField] EventSystem _eventSystem;
    [SerializeField] InputSystemUIInputModule _inputModule;

    public static MenuManager instance { get; private set; }

    Stack<Menu> _menuStack = new Stack<Menu>();

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        instance = null;
    }
#endif
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        UpdateState();
        
        // Main Menu
        _mainMenu.playButton.onClick.AddListener(Play);
        _mainMenu.settingsButton.onClick.AddListener(OpenSettings);
        _mainMenu.quitButton.onClick.AddListener(Quit);
        
        // Game Over
        _gameOverMenu.exitToMainMenuButton.onClick.AddListener(ExitToMainMenu);
        
        // Pause Menu
        _pauseMenu.resumeButton.onClick.AddListener(Resume);
        _pauseMenu.exitToMainMenuButton.onClick.AddListener(ExitToMainMenu);
        _pauseMenu.settingsButton.onClick.AddListener(OpenSettings);
        
        _backButton.onClick.AddListener(PopMenu);
        
        PushMenu(_mainMenu);
    }

    void OnEnable()
    {
        _inputModule.move.action.performed += OnMove;
    }

    void OnDisable()
    {
        _inputModule.move.action.performed -= OnMove;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnEscape();
        }
        else if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
        {
            OnEscape();
        }
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        GameObject selected = _eventSystem.currentSelectedGameObject;
        if (selected != null && selected.activeInHierarchy || _menuStack.Count < 1)
        {
            return;
        }
        
        _menuStack.Peek().defaultSelectable.Select();
    }

    public void OpenGameOverMenu()
    {
        PopAllMenus();
        PushMenu(_gameOverMenu);
        _gameplayUI.gameObject.SetActive(false);
    }
    
    void Play()
    {
        PopAllMenus();
        _gameplayUI.gameObject.SetActive(true);
        GameManager.manager.StartGame();
    }

    void OpenSettings()
    {
        PushMenu(_settingsMenu);
    }

    void Quit()
    {
        Application.Quit();
    }
    
    void Resume()
    {
        PopAllMenus();
        GameManager.manager.UnPauseGame();
    }

    void ExitToMainMenu()
    {
        PopAllMenus();
        Time.timeScale = 1;
        AudioController.ClearCachedSources();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void PushMenu(Menu menu)
    {
        if (_menuStack.Count > 0)
        {
            _menuStack.Peek().Close();
        }
        
        _menuStack.Push(menu);
        menu.Open();
        
        UpdateState();
    }

    void PopMenu()
    {
        if (_menuStack.Count < 1)
        {
            return;
        }
        
        Menu menu = _menuStack.Pop();
        menu.Close();
        
        if (_menuStack.Count > 0)
        {
            _menuStack.Peek().Open();
        }
        
        UpdateState();
    }

    void PopAllMenus()
    {
        if (_menuStack.Count < 1)
        {
            return;
        }

        _menuStack.Peek().Close();
        _menuStack.Clear();
        UpdateState();
    }

    void UpdateState()
    {
        _backButton.gameObject.SetActive(_menuStack.Count > 1);
        _gameplayUI.gameObject.SetActive(_menuStack.Count == 0);
    }

    void OnEscape()
    {
        if (_menuStack.Count == 0)
        {
            if (GameManager.manager.currentState == GameManager.GameState.PLAYING)
            {
                PushMenu(_pauseMenu);
                GameManager.manager.PauseGame();
            }
        }
        else if (_menuStack.Count == 1)
        {
            if (_menuStack.Peek() is PauseMenu)
            {
                Resume();
            }
        }
        else
        {
            PopMenu();
        }
    }
}
