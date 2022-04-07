using ES3Types;
using System;
using UnityEngine;


public class Helm : MonoBehaviour, IDamageable
{
    private Player _Player;
    private PlayerWeapons _PlayerWeapons;
    private AudioSource _AudioSource;
    
    public enum HelmType 
    {
        Shields, Weapons, Medical, Engineering, Pilot, Doors, Sensor, LifeSupport
    }

    public enum HelmState
    {
        Manned, Automatic, Impaired
    }

    
    public HelmType _HelmType;
    public HelmState _HelmState;
    public DamageSystem _DamageSystem;
    public Transform _RedBoardTransform;
    public float _StartingHealth = 100;
    private float _AutomaticRecoverAmount = 0f;
    public float _RequiredHealthToWork = 100f;
    private float _PlayerRepairAmount = 4.5f;
    public SpriteRenderer _Symbol;
    public bool _IsWorking = true;
    private float _HelmDamageModifier = 10;
    public bool _IsManned = false;
    public GameObject _LevelSymbol_I;
    public GameObject _LevelSymbol_II;
    public GameObject _LevelSymbol_III;
    public Transform _UpgradeFill;
    private float _NextAudioStopTime;


    private void Awake()
    {
        _DamageSystem = new DamageSystem(_StartingHealth,_AutomaticRecoverAmount,_RequiredHealthToWork);
    }

    private void Start()
    {
        _Player = GameObject.FindObjectOfType<Player>();
        _PlayerWeapons = GameObject.FindObjectOfType<PlayerWeapons>();
        _RedBoardTransform.localScale = new Vector3(0, 2.5f, 1);

        float _FillBarScale = _DamageSystem.GetDamagePercent();
        _RedBoardTransform.localScale = new Vector3(_FillBarScale, 1, 1);

        _AudioSource = GetComponent<AudioSource>();

        _Symbol.color = Color.grey;

        SetUpgradeFill(.5f);

        //Events
        _DamageSystem.OnHealthChanged += _DamageSystem_OnHealthChanged;
        _DamageSystem.OnStartedWorking += _DamageSystem_OnStartedWorking;
        _DamageSystem.OnStoppedWorking += _DamageSystem_OnStoppedWorking;

        GameHandler.Instance.OnSaveAllData += Instance_OnSaveAllData;

        LoadData();
    }

    private void Update()
    {
        CheckHelmState();

        if (_NextAudioStopTime < Time.time)
        {
            _AudioSource.Stop();
        }
    }

    public void LowOxygen(float value)
    {
        //Do Not Remove - Required by IDamageable
    }

    private void Instance_OnSaveAllData(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(_HelmType.ToString() + "_HelmType", _HelmType);
        ES3.Save(_HelmType.ToString() + "_HelmState", _HelmState);
        ES3.Save(_HelmType.ToString() + "_IsWorking", _IsWorking);
        ES3.Save(_HelmType.ToString() + "_IsManned", _IsManned);

        ES3.Save(_HelmType.ToString() + "_DamageSystem._CurrentHealth", _DamageSystem._CurrentHealth);
        ES3.Save(_HelmType.ToString() + "_DamageSystem._IsWorking", _DamageSystem._IsWorking);
    }

    public void LoadData()
    {
        try
        {
            //Helm
            _HelmType = ES3.Load<HelmType>(_HelmType.ToString() + "_HelmType");
            _HelmState = ES3.Load<HelmState>(_HelmType.ToString() + "_HelmState");
            _IsWorking = ES3.Load<bool>(_HelmType.ToString() + "_IsWorking");
            _IsManned = ES3.Load<bool>(_HelmType.ToString() + "_IsManned");

            _DamageSystem.SubtractHealth(0);
            _DamageSystem.AddHealth(0);

            //Damage
            _DamageSystem._CurrentHealth = ES3.Load<float>(_HelmType.ToString() + "_DamageSystem._CurrentHealth");
            _DamageSystem._IsWorking = ES3.Load<bool>(_HelmType.ToString() + "_DamageSystem._IsWorking");

            _DamageSystem.SubtractHealth(0);
            _DamageSystem.AddHealth(0);
        }
        catch { }
    }

    public HelmState GetHelmState()
    {
        return _HelmState;
    }

    public void SetUpgradeFill (float pPercent)
    {
        float _SetFillValue = 16.5f * pPercent;

        _UpgradeFill.localScale = new Vector3(_SetFillValue, 3.89f, 1);
    }

    public void SetUpgradeSymbols(bool pIsLevel_I = false, bool pIsLevel_II = false, bool pIsLevel_III = false)
    {
        _LevelSymbol_I.SetActive(pIsLevel_I);
        _LevelSymbol_II.SetActive(pIsLevel_II);
        _LevelSymbol_III.SetActive(pIsLevel_III);
    }

    private void FixedUpdate()
    {
        _DamageSystem.Repair(Time.deltaTime);
    }

    public void Damage(float pAmount, bool IsBullet, bool NoDamageToPlayer, bool IsExplosion)
    {
        if (IsBullet)
        {
            _DamageSystem.SubtractHealth(((pAmount * _HelmDamageModifier)/5));
        }
        else
        {
            _DamageSystem.SubtractHealth(pAmount * _HelmDamageModifier);
        }
    }

    public void Repair(float pAmount)
    {
        _DamageSystem.AddHealth(pAmount);
    }

    private void CheckHelmState()
    {
        if (_IsWorking)
        {
            if (_IsManned)
            {
                _Symbol.color = Color.green;
                _HelmState = HelmState.Manned;
            }
            else
            {
                _Symbol.color = Color.grey;
                _HelmState = HelmState.Automatic;
            }
        }
        else
        {
            _Symbol.color = Color.red;
            _HelmState = HelmState.Impaired;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _IsManned = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _PlayerWeapons._CurrentWeapon.GetItemId() == 4)
        {
            _NextAudioStopTime = Time.time + 0.1f;

            if (!_AudioSource.isPlaying) _AudioSource.Play();

            if (_DamageSystem.GetDamagePercent() <= 0)
            {
                _IsManned = true;
            }
            else
            {
                _IsManned = false;
                Repair(_PlayerRepairAmount * Time.deltaTime);
            }
        }
        else
        {
            _IsManned = false;
        }
    }

    //Events
    private void _DamageSystem_OnStoppedWorking(object sender, System.EventArgs e)
    {
        _Symbol.color = Color.red;
        _IsWorking = false;
        _HelmState = HelmState.Impaired;
    }
    private void _DamageSystem_OnStartedWorking(object sender, System.EventArgs e)
    {
        _Symbol.color = Color.grey;
        _IsWorking = true;
        _HelmState = HelmState.Automatic;
    }
    private void _DamageSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        float _FillBarScale = _DamageSystem.GetDamagePercent();
        _RedBoardTransform.localScale = new Vector3(_FillBarScale, 1, 1);
    }
}
