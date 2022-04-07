using UnityEngine;
using Steamworks;
using ES3Types;
using TMPro;
using System;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance { get; private set; } //Singleton

    public TextMeshProUGUI _TextLevelTimer;
    public TextMeshProUGUI _TextGameTimer;
    public TextMeshProUGUI _TextBonsuPoints;
    public TextMeshProUGUI _TextPlayerPoints;

    float _LevelTimer = 360;
    float _GameTimer = 750;
    float _BonusPoints = 100;
    float _PlayerPoints = 1500;

    float _NextTick = 0;

    private void Awake()
    {
        #region //Singleton

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    private void Start()
    {
        

    }

    private void FixedUpdate()
    {
        if (_NextTick < Time.time)
        {
            Tick();

            _NextTick = Time.time + 1;
        }
    }

    private void Tick()
    {
        //_LevelTimer++;
        //_GameTimer++;

        RefreshBoard();
    }

    private void RefreshBoard()
    {
        _TextLevelTimer.text = TimeSpan.FromSeconds(_LevelTimer).ToString(@"hh\:mm\:ss");
        _TextGameTimer.text = TimeSpan.FromSeconds(_GameTimer).ToString(@"hh\:mm\:ss");
        _TextBonsuPoints.text = _BonusPoints.ToString("N0");
        _TextPlayerPoints.text = _PlayerPoints.ToString("N0");
    }


    public void SetLevelTimer(float pValue)
    {
        _LevelTimer = pValue;
    }

    public void SetGameTimer(float pValue)
    {
        _GameTimer = pValue;
    }

    public void SetBonsuPoints(float pValue)
    {
        _BonusPoints = pValue;
    }

    public void SetPlayerPoints(float pValue)
    {
        _PlayerPoints = pValue;
    }
}

   
