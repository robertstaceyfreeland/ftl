using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Steamworks;
using UnityEngine.SocialPlatforms.Impl;
using UnityEditor.VersionControl;

public class GameHandler : MonoBehaviour
{
    #region

    public bool _Testing = false;

    public static GameHandler Instance { get; private set; } //Singleton

    public enum GameState { Start, BeforeLevel, Warp, InLevel, AfterLevel, Starbase, GameOver, Idle }
    public GameState _GameState = GameState.Idle;
    private GameMap _GameMap;

    //Music
    public enum MusicType { SuspenseHigh, SuspenseMedium, SuspenseLow, None }
    private MusicType _CurrentMusicType = MusicType.None;
    public AudioSource _AudioSource_Music;

    private AudioClip _AudioClip_SuspenseHigh;
    private AudioClip _AudioClip_SuspenseMedium;
    private AudioClip _AudioClip_SuspenseLow;


    #region EvenHandlers
    public event EventHandler OnBoardingPartyLaunched;
    public event EventHandler OnEnemyShipDetected;
    public event EventHandler OnJourneyStepChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameLevelChanged;
    #endregion

    #region Enemy Prefabs
    private GameObject _PREFAB_Enemy_A_Mele;
    private GameObject _PREFAB_Enemy_A_Ranged;
    private GameObject _PREFAB_Enemy_A_Boss;
    private GameObject _PREFAB_Enemy_B_Mele;
    private GameObject _PREFAB_Enemy_B_Ranged;
    private GameObject _PREFAB_Enemy_B_Boss;
    private GameObject _PREFAB_Enemy_C_Mele;
    private GameObject _PREFAB_Enemy_C_Ranged;
    private GameObject _PREFAB_Enemy_C_Boss;
    private GameObject _PREFAB_Enemy_D_Mele;
    private GameObject _PREFAB_Enemy_D_Ranged;
    private GameObject _PREFAB_Enemy_D_Boss;
    #endregion

    #region Spawn Transforms
    public Transform _Spawn_01;
    public Transform _Spawn_02;
    public Transform _Spawn_03;
    public Transform _Spawn_04;
    public Transform _Spawn_05;
    public Transform _Spawn_06;
    #endregion

    #region UI Buttons
    [SerializeField] private Button _ButtonJump;
    [SerializeField] private Button _ButtonAttack;
    [SerializeField] private Button _ButtonOpenAllDoors;
    [SerializeField] private Button _ButtonCloseAllDoors;
    [SerializeField] private Button _ButtonHelp;
    #endregion

    public EnemyShipHandler _EnemyShipHandler;
    public PlayerShipHandler _PlayerShipHandler;
    public MiniMap _MiniMap;

    private ShipSystem _DoorSystem;
    public Image _WarpSprite;
    public GameMessage _GameMessage;

    private const float MIN_BOARDING_PARTY_DELAY = 35;
    private const float MAX_BOARDING_PARTY_DELAY = 65;
    private const float LEVEL_DELAY = 29;

    [SerializeField] private GameObject _PREFAB_HitSpecialEffect;
    [SerializeField] private Transform _EnemyShipLocation;

    private string _EnemyShipPrefab = "";
    private GameObject _EnemyShip;
    public GameObject _EnemyHullDisplay;

    private List<Transform> _SpawnPoints = new List<Transform>();
    private List<JourneyStep> _JourneySteps = new List<JourneyStep>();
    private JourneyStep _JourneyStep;
    public int _CurrentLevel;
    public int _WaveCount = 0;

    public float _NextJumpTime;

    private float _NextBoardingPartyTime = 0;

    //private MessageLog _LogHandler;
    private int _AlienIndex;
    private int _MeleeCount;
    private int _RangedCount;
    private int _BossCount;

    private Coroutine _WaitTimer = null;


    private bool _StartBoardingParty = false;

    public bool IsOxygenSystemWorking;
    private bool _IsEnemyShipDestroyed = false;
    private bool _GameLost = false;

    Button _Button_Attack;
    private bool _NoIntruder = true;

    ObjectPooler _ObjectPooler;

    public event EventHandler OnSaveAllData;

    public GameObject _IntroText;

    #endregion

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

        _AudioClip_SuspenseLow = Resources.Load<AudioClip>("Sounds/Music/Music_LowSuspense_01");
        _AudioClip_SuspenseMedium = Resources.Load<AudioClip>("Sounds/Music/Music_MediumSuspense_01");
        _AudioClip_SuspenseHigh = Resources.Load<AudioClip>("Sounds/Music/Music_HighSuspense_01");

        _PREFAB_Enemy_A_Mele = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_A_Melee"));
        _PREFAB_Enemy_A_Ranged = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_A_Ranged"));
        _PREFAB_Enemy_A_Boss = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_A_Boss"));
        _PREFAB_Enemy_B_Mele = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_B_Melee"));
        _PREFAB_Enemy_B_Ranged = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_B_Ranged"));
        _PREFAB_Enemy_B_Boss = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_B_Boss"));
        _PREFAB_Enemy_C_Mele = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_C_Melee"));
        _PREFAB_Enemy_C_Ranged = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_C_Ranged"));
        _PREFAB_Enemy_C_Boss = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_C_Boss"));
        _PREFAB_Enemy_D_Mele = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_D_Melee"));
        _PREFAB_Enemy_D_Ranged = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_D_Ranged"));
        _PREFAB_Enemy_D_Boss = (Resources.Load<GameObject>("Prefabs/Enemies/Enemy_Alien_D_Boss"));
    }

    void Start()
    {
        _CurrentLevel = -1;
        _WaveCount = 0;

        _MiniMap = GameObject.FindObjectOfType<MiniMap>();

        PlayMusic(MusicType.SuspenseMedium);

        _DoorSystem = GameObject.Find("Helm_Door").GetComponent<ShipSystem>();
        _GameMap = GameObject.FindObjectOfType<GameMap>();

        PreLoadLevelData();

        _SpawnPoints.Add(_Spawn_01);
        _SpawnPoints.Add(_Spawn_02);
        _SpawnPoints.Add(_Spawn_03);
        _SpawnPoints.Add(_Spawn_04);
        _SpawnPoints.Add(_Spawn_05);
        _SpawnPoints.Add(_Spawn_06);

        _EnemyHullDisplay.SetActive(false);

        _MiniMap.SetMiniMapCamera(MiniMap.MiniMapMode.PlayerShip);

        LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("AdventureStarted", "Adventure Started\n")); 

         _GameState = GameState.Idle;

        _ObjectPooler = ObjectPooler.Instance;

        _ButtonAttack.interactable = false;

        try
        {
            LoadData();
        }
        catch
        {
            Debug.LogWarning("Problem-LoadGameHandler");
        }

        Achievements.Instance.PostAchievement(0);

    }

    private void Update()
    {
        //return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            _CurrentLevel = _CurrentLevel + 1;
            Debug.Log("Current Level Now: " + _CurrentLevel);
        }

        ManageGameState();

        if (_StartBoardingParty)
        {
            if (_NextBoardingPartyTime < Time.time)
            {
                BoardingParty();

                _NextBoardingPartyTime = GetNextBoardingPartyTime();
            }
        }

        if (_IsEnemyShipDestroyed)
        {
            if (!GameObject.FindObjectOfType<Enemy>())
            {
                _NoIntruder = true;

                StartCoroutine("ResetGameHandler");

                _IsEnemyShipDestroyed = false;
            }
        }

    }

    public void SaveAllData()
    {
        OnSaveAllData?.Invoke(this, EventArgs.Empty);
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(this.name + "_CurrentLevel", _CurrentLevel);
        ES3.Save(this.name + "_GameState", _GameState);
    }

    public void LoadData()
    {
        try
        {
            _CurrentLevel = ES3.Load<int>(this.name + "_CurrentLevel");
            _GameState = ES3.Load<GameState>(this.name + "_GameState");


            if (_CurrentLevel < 0)
            {
                _IntroText.SetActive(true);
            }
            else
            {
                _IntroText.SetActive(false);
                _GameState = GameState.Start;
                SetCurrentLevel(_CurrentLevel);
            }

            //PlayerPoints
            PlayerPoints.Instance.LoadData();
        }
        catch { }
    }

    public void SetCurrentLevel(int pValue)
    {
        _CurrentLevel = pValue;

        _GameMap.CurrentLevelChanged(pValue);
    }

    public int GetCurrentLevel()
    {
        return _CurrentLevel;
    }

    public void MessagReturn(GameMessage.MessageType pMessageType)
    {
        switch (pMessageType)
        {
            case GameMessage.MessageType.StartMessage:
                _GameState = GameState.Start;
                break;
            case GameMessage.MessageType.BeforeLevel:
                _GameState = GameState.BeforeLevel;
                break;
            case GameMessage.MessageType.AfterLevel:
                _GameState = GameState.AfterLevel;
                break;
            case GameMessage.MessageType.GameOver:
                break;
            case GameMessage.MessageType.Warp:
                _GameState = GameState.Warp;
                break;
            case GameMessage.MessageType.InLevel:
                _GameState = GameState.InLevel;
                break;
            case GameMessage.MessageType.Idle:
                break;
            default:
                break;
        }
    }

    private void ManageGameState()
    {


        switch (_GameState)
        {
            case GameState.Start:

                _GameMessage.NewMessage(Lean.Localization.LeanLocalization.GetTranslationText("Text_Start_01", "TNF"),
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_Start_02", "TNF"),
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_Start_03", "TNF"),
                                        GameMessage.MessageType.Idle);
                _GameState = GameState.Idle;

                _WaitTimer = StartCoroutine("Wait30Seconds");

                break;
            case GameState.BeforeLevel:
                string _Text_01 = Lean.Localization.LeanLocalization.GetTranslationText("Text_BeforeLevel_01", "TNF");
                string _Text_02 = Lean.Localization.LeanLocalization.GetTranslationText("Text_BeforeLevel_02", "TNF");
                string _Text_03 = Lean.Localization.LeanLocalization.GetTranslationText("", "");

                _GameMessage.NewMessage(_Text_01,
                                        _Text_02 + _JourneySteps[_CurrentLevel + 1].SectorName,
                                        _Text_03,
                                        GameMessage.MessageType.Warp);
                _GameState = GameState.Idle;
                break;
            case GameState.InLevel:
                LoadNextLevel();
                //_GameState = GameState.Idle;
                break;
            case GameState.AfterLevel:
                if (_CurrentLevel == 13)
                {
                    _GameMessage.NewMessage(
                                        Lean.Localization.LeanLocalization.GetTranslationText("LevelComplete", "TNF"),
                                        Lean.Localization.LeanLocalization.GetTranslationText("Congratulations", "TNF"),
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_AfterWin_03", "TNF") + _JourneySteps[_CurrentLevel].Name +
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_AfterWin_04", "TNF") + "\n" +
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_AfterWin_05", "TNF") + "\n" +
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_AfterWin_06", "TNF") + PlayerPoints.Instance.GetPlayerPoints(),
                                        GameMessage.MessageType.GameOver);

                    return;


                }
                _GameMessage.NewMessage(Lean.Localization.LeanLocalization.GetTranslationText("LevelComplete", "TNF"),
                                        Lean.Localization.LeanLocalization.GetTranslationText("Congratulations", "TNF"),
                                        Lean.Localization.LeanLocalization.GetTranslationText("Congratulations", "TNF") + _JourneySteps[_CurrentLevel].Name + "\n" +
                                        Lean.Localization.LeanLocalization.GetTranslationText("Text_Start_03", "TNF"),
                                        GameMessage.MessageType.Idle);

                _GameState = GameState.Idle;

                _WaitTimer = StartCoroutine("Wait30Seconds");
                break;
            case GameState.Starbase:
                //Starbase
                //_ButtonJump.interactable = true;

                _PlayerShipHandler.RepairShip();

                _GameState = GameState.Idle;
                _WaitTimer = StartCoroutine("Wait30Seconds");
                break;
            case GameState.GameOver:
                GameOver();
                SceneManager.LoadScene(0);
                SceneManager.UnloadSceneAsync(1);
                break;
            case GameState.Idle:
                //Do Nothing
                break;
            case GameState.Warp:
                StartCoroutine("WarpEffect");
                _GameState = GameState.Idle;
                break;
            default:
                break;
        }
    }

    private float GetModifierBoardingPartyDelay()
    {
        if (_CurrentLevel == -1) return 0;
        if (_IsEnemyShipDestroyed) return 0;

        float BoardingPartyDelay;

        BoardingPartyDelay = _JourneySteps[_CurrentLevel].BoardinPartyDelay;

        //Sensors
        BoardingPartyDelay = BoardingPartyDelay + _PlayerShipHandler._Sensor.GetHelmModifier(ShipSystem.ModifierType.BoardingPartyDelay);
        BoardingPartyDelay = BoardingPartyDelay + _PlayerShipHandler._Sensor.GetSystemModifier(ShipSystem.ModifierType.BoardingPartyDelay);

        //Door
        BoardingPartyDelay = BoardingPartyDelay + _DoorSystem.GetHelmModifier(ShipSystem.ModifierType.BoardingPartyDelay);
        BoardingPartyDelay = BoardingPartyDelay + _DoorSystem.GetSystemModifier(ShipSystem.ModifierType.BoardingPartyDelay);

        //Wave
        BoardingPartyDelay = BoardingPartyDelay - (_WaveCount * 2);

        return BoardingPartyDelay;
    }

    private float GetNextBoardingPartyTime()
    {
        float RandomDelay;

        RandomDelay = UnityEngine.Random.Range(MIN_BOARDING_PARTY_DELAY, MAX_BOARDING_PARTY_DELAY);

        return (RandomDelay + GetModifierBoardingPartyDelay()) + Time.time;

    }

    public JourneyStep GetJourneyStep()
    {
        JourneyStep _JourneyStep;
        _JourneyStep = _JourneySteps[_CurrentLevel];
        return _JourneyStep;
    }

    public void ForceJump()
    {
        StopCoroutine(_WaitTimer);

        ProceedToJump();

        //_GameState = GameState.BeforeLevel;
    }

    IEnumerator Wait30Seconds()
    {
        _ButtonJump.interactable = true;

        yield return new WaitForSeconds(LEVEL_DELAY);

        ProceedToJump();
    }

    public void ProceedToJump()
    {
        if (Time.time > _NextJumpTime)
        {
            _GameState = GameState.BeforeLevel;
            _ButtonJump.interactable = false;

            _NextJumpTime = Time.time + 10;
        }
    }

    IEnumerator WarpEffect()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Warp, transform.position, SoundManager.AudioMixerGroupName.Warp);

        const float _EFFECT_DELAY = .00f;

        _WarpSprite.enabled = true;
        _WarpSprite.color = new Color(0, 0, 0, 0);

        for (float i = 0; i < 1; i += .01f)
        {
            _WarpSprite.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(_EFFECT_DELAY);
        }
        yield return new WaitForSeconds(3f);

        for (float i = 1; i > 0; i -= .01f)
        {
            _WarpSprite.color = new Color(0, 0, 0, i);
            yield return new WaitForSeconds(_EFFECT_DELAY);
        }

        _WarpSprite.enabled = false;

        _GameState = GameState.InLevel;

    }

    public void InstantiateEnemyShip()
    {
        //TODO: Check for existence of Enemy Ship and Destroy() if found

        switch (_JourneySteps[_CurrentLevel].AlieneIndex)
        {
            case 1:
                _EnemyShipPrefab = "EnemyShip_A";
                break;
            case 2:
                _EnemyShipPrefab = "EnemyShip_B";
                break;
            case 3:
                _EnemyShipPrefab = "EnemyShip_C";
                break;
            case 4:
                _EnemyShipPrefab = "EnemyShip_D";
                break;
            default:
                break;
        }

        //_EnemyShip = Instantiate(_EnemyShipPrefab, _EnemyShipLocation.position, Quaternion.identity);

        GameObject _EnemyShip = _ObjectPooler.SpawnFromPool(_EnemyShipPrefab, _EnemyShipLocation.position, Quaternion.identity);

        _EnemyShipHandler = _EnemyShip.GetComponent<EnemyShipHandler>();

        _PlayerShipHandler.EnemyShipDetected(_EnemyShipHandler);

        _ButtonAttack.interactable = true;

        PlayMusic(MusicType.SuspenseHigh);

        if (!_EnemyShipHandler) Debug.LogError("EnemyShipHandler was NOT found!");
    }

    public void LoadNextLevel()
    {
        _CurrentLevel++;

        Debug.Log("LoadNextLevel()...Level Set to: " + _CurrentLevel);

        if (_JourneySteps[_CurrentLevel].IsStarbase)
        {
            _GameState = GameState.Starbase;
            //_GameState = GameState.Idle;
            return;
        }

        InstantiateEnemyShip();

        _EnemyHullDisplay.SetActive(true);
        _EnemyHullDisplay.GetComponent<EnemyHullDisplay>().StartEnemyHullDisplay();

        //Ship Data
        _EnemyShipHandler._HealthSystem.SetCurrentHealth(_JourneySteps[_CurrentLevel].EnemyShipStrength);
        _EnemyShipHandler._EnemyBaseRateOfFire = _JourneySteps[_CurrentLevel].EnemyMissileRateOfFire;
        _EnemyShipHandler._EnemyBaseAttackRating = _JourneySteps[_CurrentLevel]._EnemyBaseAttackRating;
        _EnemyShipHandler._EnemyBaseDefenseClass = _JourneySteps[_CurrentLevel]._EnemyBaseDefenseClass;

        //Alien Data
        _AlienIndex = _JourneySteps[_CurrentLevel].AlieneIndex;
        _MeleeCount = _JourneySteps[_CurrentLevel].MeleCount;
        _RangedCount = _JourneySteps[_CurrentLevel].RangedCount;
        _BossCount = _JourneySteps[_CurrentLevel].BossCount;



        _MiniMap.SetMiniMapCamera(MiniMap.MiniMapMode.Dogfight);

        OnEnemyShipDetected?.Invoke(this, EventArgs.Empty);

        _EnemyShipHandler.IsDogFight(true);

        _IsEnemyShipDestroyed = false;

        if (_CurrentLevel == 0)
        {
            PlayerPoints.Instance.StartGameTimer();
        }

        PlayerPoints.Instance.StartLevelTimer();

        StartBoardingParty();

        _GameState = GameState.Idle;
    }

    public void StartBoardingParty()
    {
        float RandomDelay = 0;
        RandomDelay = UnityEngine.Random.Range(10, 20);
        _NextBoardingPartyTime = RandomDelay + Time.time; //First Boarding Party
        _StartBoardingParty = true;
    }

    public void EnemyShipDestroyed()
    {
        _PlayerShipHandler.EnemyShipDestroyed();

        _StartBoardingParty = false;
        _NextBoardingPartyTime = float.MaxValue;

        _IsEnemyShipDestroyed = true;

        _GameMap.ChangeButtonColor(_CurrentLevel, Color.green);

        PlayerPoints.Instance.CalculateLevelPoints(_CurrentLevel);

        Achievements.Instance.EndOfBattleAchievements(_CurrentLevel);

        _ButtonAttack.interactable = false;

        PlayMusic(MusicType.SuspenseLow);
    }

    private IEnumerator ResetGameHandler()
    {
        yield return new WaitForSeconds(5);

        _MiniMap.SetMiniMapCamera(MiniMap.MiniMapMode.PlayerShip);

        _EnemyHullDisplay.SetActive(false);

        _GameState = GameState.AfterLevel;

        LogHandler.Instance.NewLogEntry("Saving your progress!");

        _WaveCount = 0;

        SaveAllData();

    }

    private Vector3 GetSpawnPoint(int pSpawnIndex)
    {
        float _X;
        float _Y;
        _X = UnityEngine.Random.Range(-20, 20);
        _Y = UnityEngine.Random.Range(-20, 20);

        return new Vector3(_SpawnPoints[pSpawnIndex].position.x + _X, _SpawnPoints[pSpawnIndex].position.y + _Y, _SpawnPoints[pSpawnIndex].position.z);
    }

    public void BoardingParty()
    {
        //return;

        OnBoardingPartyLaunched?.Invoke(this, EventArgs.Empty);

        int SpawnIndex = 0;

        Achievements.Instance._BoardindPartyCount++;

        switch (_AlienIndex)
        {
            case 1:

                for (int i = 0; i < _MeleeCount + _WaveCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);

                    Instantiate(_PREFAB_Enemy_A_Mele, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _RangedCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_A_Ranged, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _BossCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_A_Boss, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }
                break;
            case 2:

                for (int i = 0; i < _MeleeCount + _WaveCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_B_Mele, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _RangedCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_B_Ranged, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _BossCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_B_Boss, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }
                break;
            case 3:

                for (int i = 0; i < _MeleeCount + _WaveCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_C_Mele, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _RangedCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_C_Ranged, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _BossCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_C_Boss, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }
                break;
            case 4:

                for (int i = 0; i < _MeleeCount + _WaveCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_D_Mele, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _RangedCount + _WaveCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_D_Ranged, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }

                for (int i = 0; i < _BossCount; i++)
                {
                    SpawnIndex = UnityEngine.Random.Range(0, 5);
                    Instantiate(_PREFAB_Enemy_D_Boss, GetSpawnPoint(SpawnIndex), Quaternion.identity);
                }
                break;

            default:
                break;
        }

        _WaveCount += 1;
    }

    public void GameOver()
    {
        try
        {
            _EnemyShipHandler.IsDogFight(false);
            _PlayerShipHandler.IsDogFight(false);
        }
        catch { }

        _StartBoardingParty = false;


        _GameMessage.NewMessage("Game Over",
                                "You Scored: " + PlayerPoints.Instance.GetPlayerPoints(),
                                "Scored Posted to Global High Score Database.",
                                GameMessage.MessageType.GameOver);
        Time.timeScale = 0;


        //TODO: Add the necessary elements from Leaderboard to post High Scores
        //PostHighScore _PostHighScore = GetComponent<PostHighScore>();
        //_PostHighScore.UploadScoreToLeaderboard(100);

        if (_GameLost)
        {
            //Do Nothig
        }
        else
        {
            GameWon();
        }
    }

    private void Hull_OnHullDeath(object sender, EventArgs e)
    {
        _GameLost = true;

        GameOver();
    }

    private void GameWon()
    {
        Achievements.Instance.EndOfGameAchievements();
        
        //TODO: Add Victory Song
        PlayMusic(MusicType.None);

        //TODO: Message with Image

    }

    public void PlayMusic(MusicType pMusicType)
    {
        if (_CurrentMusicType == pMusicType) return;

        switch (pMusicType)
        {
            case MusicType.SuspenseHigh:
                _CurrentMusicType = pMusicType;
                _AudioSource_Music.clip = _AudioClip_SuspenseHigh;
                break;
            case MusicType.SuspenseMedium:
                _CurrentMusicType = pMusicType;
                _AudioSource_Music.clip = _AudioClip_SuspenseMedium;
                break;
            case MusicType.SuspenseLow:
                _CurrentMusicType = pMusicType;
                _AudioSource_Music.clip = _AudioClip_SuspenseLow;
                break;
            case MusicType.None:
                _CurrentMusicType = pMusicType;
                break;
            default:
                _CurrentMusicType = MusicType.None;
                break;
        }

        _AudioSource_Music.Stop();

        if (_CurrentMusicType == MusicType.None) return;

        _AudioSource_Music.Play();
    }

    private void PreLoadLevelData()
    {
        JourneyStep MyStep = new JourneyStep();

        MyStep.Name = "You have survived the encounter with the Slugs!";
        MyStep.SectorName = "Sector Sluggi Alpha I";
        MyStep.Index = 1;
        MyStep.AlieneIndex = 1;
        MyStep.JourneyDisplayName = "Jump_01";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 15f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 1;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 60;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 13;
        MyStep._EnemyBaseDefenseClass = 1;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Slug Battle!";
        MyStep.SectorName = "Sector Sluggi Alpha II";
        MyStep.Index = 2;
        MyStep.AlieneIndex = 1;
        MyStep.JourneyDisplayName = "Jump_02";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 14f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 70;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 12;
        MyStep._EnemyBaseDefenseClass = 2;

        _JourneySteps.Add(MyStep);


        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Slug Boss!";
        MyStep.SectorName = "Sector Sluggi Alpha III";
        MyStep.Index = 3;
        MyStep.AlieneIndex = 1;
        MyStep.JourneyDisplayName = "Jump_03";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 13f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 1;
        MyStep.EnemyShipStrength = 70;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 11;
        MyStep._EnemyBaseDefenseClass = 2;

        _JourneySteps.Add(MyStep);


        MyStep = new JourneyStep();
        MyStep.Name = "You have refueled and repaired at Starbase Alpha";
        MyStep.SectorName = "A";
        MyStep.Index = 4;
        MyStep.AlieneIndex = 0;
        MyStep.JourneyDisplayName = "Repair and Run!";
        MyStep.IsFiringMissiles = false;
        MyStep.EnemyMissileRateOfFire = 00;
        MyStep.MeleCount = 0;
        MyStep.RangedCount = 0;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 0;
        MyStep.IsSkirmish = false;
        MyStep.IsStarbase = true;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 0;
        MyStep._EnemyBaseDefenseClass = 0;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the encounter with the Flys!";
        MyStep.SectorName = "Sector Venus I";
        MyStep.Index = 5;
        MyStep.AlieneIndex = 2;
        MyStep.JourneyDisplayName = "Jump_04";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 12f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 1;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 75;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 10;
        MyStep._EnemyBaseDefenseClass = 3;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the fly trap Battle!";
        MyStep.SectorName = "Sector Venus II";
        MyStep.Index = 6;
        MyStep.AlieneIndex = 2;
        MyStep.JourneyDisplayName = "Jump_05";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 11f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 75;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 9;
        MyStep._EnemyBaseDefenseClass = 3;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Fly Boss!";
        MyStep.SectorName = "Sector Venus III";
        MyStep.Index = 7;
        MyStep.AlieneIndex = 2;
        MyStep.JourneyDisplayName = "Jump_06";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 10f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 1;
        MyStep.EnemyShipStrength = 75;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 8;
        MyStep._EnemyBaseDefenseClass = 3;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have refueled and repaired at Starbase Bravo";
        MyStep.SectorName = "Sector B";
        MyStep.Index = 8;
        MyStep.AlieneIndex = 0;
        MyStep.JourneyDisplayName = "Base_02";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 0;
        MyStep.MeleCount = 0;
        MyStep.RangedCount = 0;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 0;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = true;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 0;
        MyStep._EnemyBaseDefenseClass = 0;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the encounter with the Bots!";
        MyStep.SectorName = "Sector Silicon I";
        MyStep.Index = 9;
        MyStep.AlieneIndex = 3;
        MyStep.JourneyDisplayName = "Jump_07";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 9f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 1;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 80;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 7;
        MyStep._EnemyBaseDefenseClass = 5;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Bot Battle!";
        MyStep.SectorName = "Sector Silicon II";
        MyStep.Index = 10;
        MyStep.AlieneIndex = 3;
        MyStep.JourneyDisplayName = "Jump_08";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 8f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 80;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 6;
        MyStep._EnemyBaseDefenseClass = 5;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Bot Boss!";
        MyStep.SectorName = "Sector Silicon III";
        MyStep.Index = 11;
        MyStep.AlieneIndex = 3;
        MyStep.JourneyDisplayName = "Jump_09";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 8f;
        MyStep.MeleCount = 2;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 1;
        MyStep.EnemyShipStrength = 80;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 6;
        MyStep._EnemyBaseDefenseClass = 5;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have refueled and repaired at Starbase Charlie";
        MyStep.SectorName = "Sector C";
        MyStep.Index = 12;
        MyStep.AlieneIndex = 0;
        MyStep.JourneyDisplayName = "Base_03";
        MyStep.IsFiringMissiles = false;
        MyStep.EnemyMissileRateOfFire = 0;
        MyStep.MeleCount = 0;
        MyStep.RangedCount = 0;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 00;
        MyStep.IsSkirmish = false;
        MyStep.IsStarbase = true;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 0;
        MyStep._EnemyBaseDefenseClass = 0;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Evil Federation Patrol!";
        MyStep.SectorName = "Outer Rim";
        MyStep.Index = 13;
        MyStep.AlieneIndex = 4;
        MyStep.JourneyDisplayName = "Jump_10";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 8f;
        MyStep.MeleCount = 3;
        MyStep.RangedCount = 1;
        MyStep.BossCount = 0;
        MyStep.EnemyShipStrength = 90;
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = false;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 6;
        MyStep._EnemyBaseDefenseClass = 5;

        _JourneySteps.Add(MyStep);

        MyStep = new JourneyStep();
        MyStep.Name = "You have survived the Federation Command Ship Attack!";
        MyStep.SectorName = "System - SOL (Our Solar System)";
        MyStep.Index = 14;
        MyStep.AlieneIndex = 4;
        MyStep.JourneyDisplayName = "Jump_10";
        MyStep.IsFiringMissiles = true;
        MyStep.EnemyMissileRateOfFire = 8f;
        MyStep.MeleCount = 3;
        MyStep.RangedCount = 2;
        MyStep.BossCount = 1;
        MyStep.EnemyShipStrength = 1; //TODO: Return to 100
        MyStep.IsSkirmish = true;
        MyStep.IsStarbase = false;
        MyStep.IsFinalBattle = true;
        MyStep.IsFiringLasers = false;
        MyStep.EnemyBaseLaserRateOfFire = 0;
        MyStep._EnemyBaseAttackRating = 6;
        MyStep._EnemyBaseDefenseClass = 0; //TODO: return to 6

        _JourneySteps.Add(MyStep);

        for (int i = 0; i < _JourneySteps.Count; i++)
        {
            //Debug.Log("Name: " + _JourneySteps[i].Name );
            //Debug.Log("Step Index: " + _JourneySteps[i].Index);
            //Debug.Log("Alien Index: " + _JourneySteps[i].AlieneIndex);
        }
    }

    public int OneDeeTwenty()
    {
        int DiceRoll = 0;

        DiceRoll = UnityEngine.Random.Range(1, 20);

        return DiceRoll;
    }

    public int DamageRoll(int MinDamage, int MaxDamage)
    {
        return UnityEngine.Random.Range(MinDamage, MaxDamage);
    }

    public Vector3 NewVector3WithRandom(Vector3 pCurrentVector3, float pSpread, float pXOffset = 0, float pYOffset = 0)
    {
        float _NewX;
        float _NewY;
        Vector3 _NewVector3;

        _NewX = pCurrentVector3.x;
        _NewY = pCurrentVector3.y;

        _NewX = UnityEngine.Random.Range((_NewX - (pSpread * .5f)), (_NewX + (pSpread * .5f)));
        _NewY = UnityEngine.Random.Range((_NewY - (pSpread * .5f)), (_NewY + (pSpread * .5f)));

        _NewX += pXOffset;
        _NewY += pYOffset;

        _NewVector3 = new Vector3(_NewX, _NewY, pCurrentVector3.z);

        return _NewVector3;

    }

}

