using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _soundVolumeSlider;
    [SerializeField] Slider _voiceVolumeSlider;
    
    [SerializeField] TMP_Dropdown _graphicsQualityDropdown;
    [SerializeField] TMP_Dropdown _resolutionDropdown;

    [SerializeField] Toggle _fullScreenToggle;
    [SerializeField] Toggle _vSyncToggle;

    List<Resolution> _resolutions = new List<Resolution>();

    Vector2Int _previousScreenSize;
    Resolution _previousRes;
    FullScreenMode _previousFullScreenMode;

    void Awake()
    {
        _graphicsQualityDropdown.ClearOptions();
        _graphicsQualityDropdown.AddOptions(QualitySettings.names.ToList());
        
        _fullScreenToggle.isOn = Screen.fullScreen;
        _vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        
        UpdateResolutions(Screen.fullScreen);
        
        // Callbacks
        
        _graphicsQualityDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);
        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        
        _fullScreenToggle.onValueChanged.AddListener(OnFullScreenChanged);
        _vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        
        _masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChanged(); });
        _musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(); });
        _soundVolumeSlider.onValueChanged.AddListener(delegate { OnSoundVolumeChanged(); });
        _voiceVolumeSlider.onValueChanged.AddListener(delegate { OnVoiceVolumeChanged(); });
    }
    
    void LateUpdate()
    {
        bool resolutionChanged = false;

        if (!Screen.fullScreen)
        {
            Vector2Int size = new Vector2Int(Screen.width, Screen.height);
            if (size != _previousScreenSize)
            {
                resolutionChanged = true;
            }

            _previousScreenSize = size;
        }
        else
        {
            Resolution res = Screen.currentResolution;
            if (_previousRes.width != res.width || _previousRes.height != res.height || Math.Abs(_previousRes.refreshRateRatio.value - res.refreshRateRatio.value) > Mathf.Epsilon)
            {
                resolutionChanged = true;
            }

            _previousRes = res;
        }
        
        if (Screen.fullScreenMode != _previousFullScreenMode)
        {
            resolutionChanged = true;
        }

        _previousFullScreenMode = Screen.fullScreenMode;
        
        if (resolutionChanged)
        {
            UpdateResolutions(Screen.fullScreen);
        }
    }

    void UpdateResolutions(bool isFullScreen)
    {
        _resolutions = Screen.resolutions.ToList();

        Resolution currentRes;
        
        if (!Screen.fullScreen)
        {
            currentRes = new Resolution { height = Screen.height, width = Screen.width, refreshRateRatio = Screen.currentResolution.refreshRateRatio };
        }
        else
        {
            currentRes = Screen.currentResolution;
        }
        
        if (!_resolutions.Exists(res => res.width == currentRes.width && res.height == currentRes.height && Math.Abs(res.refreshRateRatio.value - currentRes.refreshRateRatio.value) <= Mathf.Epsilon))
        {
            _resolutions.Insert(0, currentRes);
        }
        
        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(_resolutions.Select(res => res.width + " x " + res.height + " @ " + res.refreshRateRatio.value.ToString("F0") + " Hz").ToList());
        _resolutionDropdown.SetValueWithoutNotify(0);
        
        _resolutionDropdown.interactable = isFullScreen;

    }

    void OnMasterVolumeChanged()
    {
        
    }
    
    void OnMusicVolumeChanged()
    {
        
    }
    
    void OnSoundVolumeChanged()
    {
        
    }
    
    void OnVoiceVolumeChanged()
    {
        
    }

    void OnGraphicsQualityChanged(int value)
    {
        int vSyncCount = QualitySettings.vSyncCount;
        QualitySettings.SetQualityLevel(value);
        QualitySettings.vSyncCount = vSyncCount;
    }

    void OnResolutionChanged(int value)
    {
        Resolution res = _resolutions[value];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    void OnFullScreenChanged(bool isOn)
    {
        if (Screen.fullScreen == isOn)
        {
            return;
        }

        Screen.fullScreen = isOn;
        if (isOn)
        {
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        }
    }

    void OnVSyncChanged(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
    }
}
