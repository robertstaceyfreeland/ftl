using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer _AudioMixer_Master;
    public AudioMixer _AudioMixer_Music;
    Resolution[] _Resolutions;

    private float _Pp_MusicVolume;
    private float _Pp_MasterVolume;
    private int _Pp_Quality;
    private bool _Pp_FullScreen;
    private int _Pp_Resolution;

    public Slider _Slider_Music;
    public Slider _Slider_Master;
    public Dropdown _Dropdown_Quality;
    public Dropdown _Dropdown_Resolution;
    public Toggle _Toggle_FullScreen;


    private string _Pp_Filename = "PlayerPrefrences.prf";

    private void Start()
    {
        ConfigureResolution();
        LoadPlayerPreferences();
        SetPlayerPreferences();
    }

    private void LoadPlayerPreferences()
    {
        try
        {
            _Pp_MusicVolume = ES3.Load<float>("_Pp_MusicVolume", _Pp_Filename);
            _Pp_MasterVolume = ES3.Load<float>("_Pp_MasterVolume", _Pp_Filename);
            _Pp_Quality = ES3.Load<int>("_Pp_Quality", _Pp_Filename);
            _Pp_FullScreen = ES3.Load<bool>("_Pp_FullScreen", _Pp_Filename);
            _Pp_Resolution = ES3.Load<int>("_Pp_Resolution", _Pp_Filename);
        }
        catch { }
    }

    private void SetPlayerPreferences()
    {
        _Slider_Master.value = _Pp_MasterVolume;
        _Slider_Music.value = _Pp_MusicVolume;
        _Dropdown_Quality.value = _Pp_Quality;
        _Toggle_FullScreen.isOn = _Pp_FullScreen;
    }

    private void ConfigureResolution()
    {
        _Resolutions = Screen.resolutions;

        _Dropdown_Resolution.ClearOptions();

        List<string> _Options = new List<string>();

        int _CurrentResolutionIndex = 0;

        for (int i = 0; i < _Resolutions.Length; i++)
        {
            string option = _Resolutions[i].width + " x " + _Resolutions[i].height;

            _Options.Add(option);

            if (_Resolutions[i].width == Screen.currentResolution.width && _Resolutions[i].height == Screen.currentResolution.height)
            {
                _CurrentResolutionIndex = i;
            }
        }

        _Dropdown_Resolution.AddOptions(_Options);
        _Dropdown_Resolution.value = +_CurrentResolutionIndex;
        _Dropdown_Resolution.RefreshShownValue();
    }

    public void SavePlayerPreferences()
    {
        ES3.Save("_Pp_MusicVolume", _Pp_MusicVolume, _Pp_Filename);
        ES3.Save("_Pp_MasterVolume", _Pp_MasterVolume, _Pp_Filename);
        ES3.Save("_Pp_Quality", _Pp_Quality, _Pp_Filename);
        ES3.Save("_Pp_FullScreen", _Pp_FullScreen, _Pp_Filename);
        ES3.Save("_Pp_Resolution", _Pp_Resolution, _Pp_Filename);
    }

    public void SetMusicVolume(float value)
    {
        _AudioMixer_Music.SetFloat("MusicVolume", value);
        _Pp_MusicVolume = value;
    }

    public void SetSoundVolume(float value)
    {
        _AudioMixer_Master.SetFloat("MasterVolume", value);
        _Pp_MasterVolume = value;
    }

    public void SetQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        _Pp_Quality = value;
    }

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
        _Pp_FullScreen = value;
    }

    public void SetLanguage()
    {
        //LeanLocalization
    }

    public void SetResolution(int value)
    {
        Resolution _Resolution = _Resolutions[value];
        Screen.SetResolution(_Resolution.width, _Resolution.height, Screen.fullScreen);
        _Pp_Resolution = value;
    }
}
