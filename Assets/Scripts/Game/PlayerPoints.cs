using UnityEngine;
using Steamworks;
using ES3Types;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerPoints : MonoBehaviour
{
    public static PlayerPoints Instance { get; private set; } //Singleton
    
    public float _CurrentPlayerPoints = 0;
    public float _ElapsedGameTime;
    private float _ElapsedLevelTime;

    private float _NextTick = 0;

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

    private void Update()
    {
        _ElapsedGameTime += Time.deltaTime;
        _ElapsedLevelTime += Time.deltaTime;

        if (_NextTick < Time.time)
        {
            Tick();

            _NextTick = Time.time + 1;
        }
    }

    private void Tick()
    {
        ScoreBoard.Instance.SetBonsuPoints(GetBonusTimePoints());
        ScoreBoard.Instance.SetPlayerPoints(_CurrentPlayerPoints);
        ScoreBoard.Instance.SetLevelTimer(_ElapsedLevelTime);
        ScoreBoard.Instance.SetGameTimer(_ElapsedGameTime);
    }

    public void SaveData()
    {
        ES3.Save(this.name + "_CurrentPlayerPoints", _CurrentPlayerPoints);
        ES3.Save(this.name + "_ElapsedGameTime", _ElapsedGameTime);
    }

    public void LoadData()
    {
        _CurrentPlayerPoints = ES3.Load<float>(this.name + "_CurrentPlayerPoints");
        _ElapsedGameTime = ES3.Load<float>(this.name + "_ElapsedGameTime");
    }

    public void StartGameTimer()
    {
        _ElapsedGameTime = 0;
        ScoreBoard.Instance.SetGameTimer(_ElapsedGameTime);
    }

    public void StartLevelTimer()
    {
        _ElapsedLevelTime = 0;
        ScoreBoard.Instance.SetLevelTimer(_ElapsedLevelTime);

    }

    public void AddPoints(float _Amount, string _Event)
    {
        _CurrentPlayerPoints += _Amount;


        LogHandler.Instance.NewLogEntry("Points Scored for " + _Event + ".");
        LogHandler.Instance.NewLogEntry("Total Points: " + _CurrentPlayerPoints.ToString("N0") + "\n");
    }

    public int GetPlayerPoints()
    {
        return (int)_CurrentPlayerPoints;
    }

    public float CalculateLevelPoints(int _CurrentLevel)
    {
        

        _CurrentPlayerPoints += ((_CurrentLevel + 1) * 2000);
        _CurrentPlayerPoints += GetBonusTimePoints();

        LogHandler.Instance.NewLogEntry("PLAYER Scored " + GetBonusTimePoints().ToString("N0") + " Bonus Time Points!");
        LogHandler.Instance.NewLogEntry("Total Points: " + _CurrentPlayerPoints.ToString("N0") + "\n");

        return 0;
    }

    private float GetBonusTimePoints()
    {
        float _StandardLevelTime = 600;
        float _AllotedLevelTime = (_StandardLevelTime + (GameHandler.Instance._CurrentLevel * 30)) + 600;
        float _BonusTime = Mathf.Clamp((_AllotedLevelTime - _ElapsedLevelTime), 0, 10000);
        float _BonusTimePoints = (_BonusTime * 3);

        return _BonusTimePoints;
    }
}


