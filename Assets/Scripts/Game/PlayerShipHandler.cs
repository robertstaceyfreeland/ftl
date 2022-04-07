using UnityEngine;
using System;
using System.Collections;

public class PlayerShipHandler : MonoBehaviour
{
    public enum EnemyShipAlertLevel { Red, Yellow, Off}
    public enum EnemyIntruderAlertLevel { Red, Yellow, Off }

    public EnemyShipAlertLevel _EnemyShipAlertLevel = EnemyShipAlertLevel.Off;
    public EnemyIntruderAlertLevel _EnemyIntruderAlertLevel = EnemyIntruderAlertLevel.Off;

    bool _ShowDebug = true;

    //Core Game Object
    public HealthSystem _HealthSystem;
    public Player _Player;
    public EnemyShipHandler _EnemyShipHandler;

    //Systems
    public ShipSystem _Engineering;
    public ShipSystem _Pilot;
    public ShipSystem _Sensor;
    public ShipSystem _Weapon;
    public ShipSystem _Shield;
    public ShipSystem _Medical;
    public ShipSystem _LifeSupport;
    

    //Stations
    public HealingStation _HealingStation;
    public OxygenGenerator _OxygenGenerator;
    public MiniMap _MiniMap;

    //Prefabs
    public GameObject _MissilePrefab;
    public Transform _MissileMuzzle;
    public GameObject _Sfx_HullExplosion;

    //Audio
    public AudioSource _AlarmEnemyShipDetected;
    public AudioSource _AlarmIntruder;
    private float _ExplosionSoundDuration = 3.5f;
    
    //Startup Values
    private float _StartingHealth = 100;                                
    private float _StartingStamina = 100;                               
    private float _RequiredStaminaToRecover = 20;
    private float _StaminaUsedToFireMissile = 10;
    private float _RestingStaminaRecoverAmount = 1f;

    //Ship Data
    private int _PlayerBaseAttackRating = 10;  //TODO: Should be  11
    private int _PlayerBaseDefenseClass = 1;
    private float _PlayerBaseEnergyRecovery = 1f;
    private int _PlayerBaseRateOfFire = 10;
    
    //Missile
    private float _MissileForce = 500;                                    //Speed of Missle
    private int _MissileMinDamage = 6; // TODO: Reset to 6 or Balance
    private int _MissileMaxDamage = 12; // TODO: Reset to 12 or Balance
    private float _MissleHitsDestroyDelay = 4.5f;
    private float _MissleMissesDestroyDelay = 8;

    //Private Variables
    public float _NextShootTime = 0;
    public bool _IsCrewExhausted = false;
    public bool _IsDead = false;
    public bool _IsDogfight = false;

    public float _Next2SecondTick = 0;
    private float _AverageEnergyUsedPerSecond;

    private void Awake()
    {
        _HealthSystem = new HealthSystem(_RestingStaminaRecoverAmount, _StartingHealth, _StartingStamina, _RequiredStaminaToRecover);
    }
    
    void Start()
    {
        _Engineering = GameObject.Find("Helm_Engineering").GetComponent<ShipSystem>();
        _Pilot = GameObject.Find("Helm_Pilot").GetComponent<ShipSystem>();
        _Sensor = GameObject.Find("Helm_Sensor").GetComponent<ShipSystem>();
        _Weapon = GameObject.Find("Helm_Weapon").GetComponent<ShipSystem>();
        _Shield = GameObject.Find("Helm_Shield").GetComponent<ShipSystem>();
        _Medical = GameObject.Find("Helm_Medical").GetComponent<ShipSystem>();
        _LifeSupport = GameObject.Find("Helm_LifeSupport").GetComponent<ShipSystem>();
        
        _OxygenGenerator = GameObject.FindObjectOfType<OxygenGenerator>();
        _HealingStation = GameObject.FindObjectOfType<HealingStation>();
        _MiniMap = GameObject.FindObjectOfType<MiniMap>();
        _Player = GameObject.FindObjectOfType<Player>();

        //Subscribe to Events
        _HealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
        _HealthSystem.OnDeath += HealthSystem_OnDeath;
        _HealthSystem.OnStaminaChanged += _HealthSystem_OnStaminaChanged;
        _HealthSystem.OnStaminaExhausted += _HealthSystem_OnStaminaExhausted;
        _HealthSystem.OnStaminaRecovered += _HealthSystem_OnStaminaRecovered;

        GameHandler.Instance.OnBoardingPartyLaunched += Instance_OnBoardingPartyLaunched;
        GameHandler.Instance.OnEnemyShipDetected += Instance_OnEnemyShipDetected;

        GameHandler.Instance.OnSaveAllData += Instance_OnSaveAllData;

        LoadData();
    }

    private void Instance_OnSaveAllData(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(this.name + "_EnemyShipAlertLevel", _EnemyShipAlertLevel);
        ES3.Save(this.name + "_EnemyIntruderAlertLevel", _EnemyIntruderAlertLevel);
        ES3.Save(this.name + "_NextShootTime", _NextShootTime);
        ES3.Save(this.name + "_IsCrewExhausted", _IsCrewExhausted);
        ES3.Save(this.name + "_IsDead", _IsDead);
        ES3.Save(this.name + "_IsDogfight", _IsDogfight);
        ES3.Save(this.name + "_Next2SecondTick", _Next2SecondTick);

        ES3.Save(this.name + "_HealthSystem._CurrentHealth", _HealthSystem._CurrentHealth);
        ES3.Save(this.name + "_HealthSystem._CurrentStamina", _HealthSystem._CurrentStamina);
        ES3.Save(this.name + "_HealthSystem._IsInvincible", _HealthSystem._IsInvincible);
        ES3.Save(this.name + "_HealthSystem._IsExhausted", _HealthSystem._IsExhausted);
        ES3.Save(this.name + "_HealthSystem._StaminaUsedThisTick", _HealthSystem._StaminaUsedThisTick);
    }

    public void LoadData()
    {
        try
        {
            _EnemyShipAlertLevel = ES3.Load<EnemyShipAlertLevel>(this.name + "_EnemyShipAlertLevel");
            _EnemyIntruderAlertLevel = ES3.Load<EnemyIntruderAlertLevel>(this.name + "_EnemyIntruderAlertLevel");
            _NextShootTime = 0;
            _IsCrewExhausted = ES3.Load<bool>(this.name + "_IsCrewExhausted");
            _IsDead = ES3.Load<bool>(this.name + "_IsDead");
            _IsDogfight = ES3.Load<bool>(this.name + "_IsDogfight");
            _Next2SecondTick = ES3.Load<float>(this.name + "_Next2SecondTick");

            _HealthSystem._CurrentHealth = ES3.Load<float>(this.name + "_HealthSystem._CurrentHealth");
            _HealthSystem._CurrentStamina = ES3.Load<float>(this.name + "_HealthSystem._CurrentStamina");
            _HealthSystem._IsInvincible = ES3.Load<bool>(this.name + "_HealthSystem._IsInvincible");
            _HealthSystem._IsExhausted = ES3.Load<bool>(this.name + "_HealthSystem._IsExhausted");
            _HealthSystem._StaminaUsedThisTick = ES3.Load<float>(this.name + "_HealthSystem._StaminaUsedThisTick");

            _HealthSystem.AddHealth(0);
            _HealthSystem.SubtractHealth(0);
            _HealthSystem.AddStamina(0);
            _HealthSystem.SubtractStamina(0);
        }
        catch { }
    }

    public void UseStamina(float pAmount)
    {
        _HealthSystem.SubtractStamina(pAmount);
    }

    public int GetAttackRating()
    {
        return _PlayerBaseAttackRating;
    }

    private void Instance_OnEnemyShipDetected(object sender, EventArgs e)
    {
        _EnemyShipAlertLevel = EnemyShipAlertLevel.Red;
        
        AlarmCheck();
        
        //_AlarmEnemyShipDetected.Play();
    }

    private void Instance_OnBoardingPartyLaunched(object sender, EventArgs e)
    {
        _AlarmIntruder.Play();
        
    }

    void Update()
    {

        //TODO:  This needs adjustment
        //Debug.Log("Ships Current Stamina: " + _HealthSystem.GetStaminaAmount());

        AlarmCheck();

        float _EnergyRecoveryAmount = GetEnergyRecoveryAmount() * Time.deltaTime;


        _HealthSystem.Rest(_EnergyRecoveryAmount);

        if (_Next2SecondTick <Time.time)
        {
            _AverageEnergyUsedPerSecond = (_HealthSystem.GetStaminaUsed() / 2);
            
            _Next2SecondTick = Time.time + 2;
        }


        if (!_IsDead)
        {
            if (_IsDogfight)
            {
                Dogfight();
            }
        }
        
        _Player.SetHealthModifier(GetPlayerHealthModifier());
        _Player.SetStaminaModifier(GetPlayerStaminaModifier());
        _HealingStation.SetHealModifier(GetRateOfHealModifier());
        _OxygenGenerator.SetOxygenGeneratedModifier(GetOxygenGeneratedModifier());
        
        

        ScanShip();
    }  

    public float GetEnergyUsedPerSecond()
    {
        return _AverageEnergyUsedPerSecond;
    }

    public void RepairShip()
    {
        _HealthSystem.AddHealth(100);
        LogHandler.Instance.NewLogEntry("Your Ship is fully repaired.");
    }

    public void DamageShip(float pAmount)
    {
        _HealthSystem.SubtractHealth(pAmount);
        LogHandler.Instance.NewLogEntry("Your Ship Takes " + pAmount + "of Damage!");
    }

    public void ScanShip()
    {
        GameObject[] _EnemyLifeForms = GameObject.FindGameObjectsWithTag("Enemy");
        
        //Debug.Log("Number of Enemies: " + _EnemyLifeForms.Length);
        
        if (_EnemyLifeForms.Length > 0)
        {
            if (_EnemyIntruderAlertLevel == EnemyIntruderAlertLevel.Off)
            {
                _EnemyIntruderAlertLevel = EnemyIntruderAlertLevel.Red;

                _MiniMap.SoundRedAlert(true);
            }
            else
            {
                //Do Nothing - All is Set
            }
        }
        else
        {
            _EnemyIntruderAlertLevel = EnemyIntruderAlertLevel.Off;
            _MiniMap.SoundRedAlert(false);
        }
    }

    public HealthSystem GetHealthSystem() 
    { return _HealthSystem; }

    public ShipSystem GetShieldSystem()
    { return _Shield; }

    public void IsDead(bool value)
    {
        _IsDead = value;
    }

    public void IsDogFight(bool value)
    {
        _IsDogfight = value;
    }

    public void EnemyShipDetected(EnemyShipHandler pEnemyShipHandler)
    {
        //Debug.Log("EnemyShipDetected!");
        _EnemyShipHandler = pEnemyShipHandler;
        _EnemyShipAlertLevel = EnemyShipAlertLevel.Red;
    }

    public void EnemyShipDestroyed()
    {
        _IsDogfight = false;

        _EnemyShipAlertLevel = EnemyShipAlertLevel.Off;

        _HealthSystem.AddStamina(100);

    }

    public void Dogfight()
    {
        if (!_IsCrewExhausted)
        {
            _HealthSystem.SubtractStamina(_StaminaUsedToFireMissile/ 8 * Time.deltaTime);

            if (Time.time > _NextShootTime)
            {
                LaunchMissile();

                //_HealthSystem.SubtractStamina(_StaminaUsedToFireMissile);
                
                _NextShootTime = Time.time + GetRateOfFire();
            }
        }
        else
        {
            _NextShootTime += Time.deltaTime;
        }
    }

    public float GetOxygenGeneratedModifier()
    {
        float _ModifiedOxygenGenerated;

        _ModifiedOxygenGenerated = 0;

        //Life Support
        _ModifiedOxygenGenerated = _ModifiedOxygenGenerated + _LifeSupport.GetHelmModifier(ShipSystem.ModifierType.OxygenFlow);
        _ModifiedOxygenGenerated = _ModifiedOxygenGenerated + _LifeSupport.GetSystemModifier(ShipSystem.ModifierType.OxygenFlow);

        _ModifiedOxygenGenerated = UnityEngine.Random.Range((_ModifiedOxygenGenerated * .95f), (_ModifiedOxygenGenerated * 1.05f));

        return _ModifiedOxygenGenerated;
    }

    public float GetEnergyRecoveryAmount()
    {
        float _PlayerModifiedEnergyRecovery;

        _PlayerModifiedEnergyRecovery = _PlayerBaseEnergyRecovery;

        //Engineering
        _PlayerModifiedEnergyRecovery = _PlayerModifiedEnergyRecovery + _Engineering.GetHelmModifier(ShipSystem.ModifierType.EnergyRecovery);
        _PlayerModifiedEnergyRecovery = _PlayerModifiedEnergyRecovery + _Engineering.GetSystemModifier(ShipSystem.ModifierType.EnergyRecovery);

        _PlayerModifiedEnergyRecovery = UnityEngine.Random.Range((_PlayerModifiedEnergyRecovery * .95f), (_PlayerModifiedEnergyRecovery*1.05f));

        return Mathf.Clamp(_PlayerModifiedEnergyRecovery,0,99) ;
    }

    public int GetRateOfFire()
    {
        int _PlayerModifiedRateofFire;

        _PlayerModifiedRateofFire = _PlayerBaseRateOfFire;
        
        //Sensor
        _PlayerModifiedRateofFire = _PlayerModifiedRateofFire - _Sensor.GetHelmModifier(ShipSystem.ModifierType.RateOfFire);
        _PlayerModifiedRateofFire = _PlayerModifiedRateofFire - _Sensor.GetSystemModifier(ShipSystem.ModifierType.RateOfFire);

        //Weapon
        _PlayerModifiedRateofFire = _PlayerModifiedRateofFire - _Weapon.GetHelmModifier(ShipSystem.ModifierType.RateOfFire);
        _PlayerModifiedRateofFire = _PlayerModifiedRateofFire - _Weapon.GetSystemModifier(ShipSystem.ModifierType.RateOfFire);


        return _PlayerModifiedRateofFire;
    }

    public float GetRateOfHealModifier()
    {
        float _ModifiedRateOfHeal;

        _ModifiedRateOfHeal = 0;

        //Medical
        _ModifiedRateOfHeal = _ModifiedRateOfHeal +( _Medical.GetHelmModifier(ShipSystem.ModifierType.RateOfHealing) /10);
        _ModifiedRateOfHeal = _ModifiedRateOfHeal + (_Medical.GetSystemModifier(ShipSystem.ModifierType.RateOfHealing) / 10);

        
        return _ModifiedRateOfHeal;
    }

    public int GetPlayerHealthModifier()
    {
        int _PlayerModifiedHealth;

        _PlayerModifiedHealth = 0;

        //Medical
        _PlayerModifiedHealth = _PlayerModifiedHealth + _Medical.GetHelmModifier(ShipSystem.ModifierType.PlayerHealth);
        _PlayerModifiedHealth = _PlayerModifiedHealth + _Medical.GetSystemModifier(ShipSystem.ModifierType.PlayerHealth);

        //Life Support
        _PlayerModifiedHealth = _PlayerModifiedHealth + _LifeSupport.GetHelmModifier(ShipSystem.ModifierType.PlayerHealth);
        _PlayerModifiedHealth = _PlayerModifiedHealth + _LifeSupport.GetSystemModifier(ShipSystem.ModifierType.PlayerHealth);

        return _PlayerModifiedHealth;
    }

    public int GetPlayerStaminaModifier()
    {
        int _PlayerModifiedStamina;

        _PlayerModifiedStamina = 0;

        //Medical
        _PlayerModifiedStamina = _PlayerModifiedStamina + _Medical.GetHelmModifier(ShipSystem.ModifierType.PlayerStamina);
        _PlayerModifiedStamina = _PlayerModifiedStamina + _Medical.GetSystemModifier(ShipSystem.ModifierType.PlayerStamina);

        //Life Support
        _PlayerModifiedStamina = _PlayerModifiedStamina + _LifeSupport.GetHelmModifier(ShipSystem.ModifierType.PlayerStamina);
        _PlayerModifiedStamina = _PlayerModifiedStamina + _LifeSupport.GetSystemModifier(ShipSystem.ModifierType.PlayerStamina);

        return _PlayerModifiedStamina;
    }

    public int GetDefenseClass()
    {
        int _PlayerModifiedDefenseClass;

        _PlayerModifiedDefenseClass = _PlayerBaseDefenseClass;

        //Shield
        _PlayerModifiedDefenseClass = _PlayerModifiedDefenseClass + _Shield.GetHelmModifier(ShipSystem.ModifierType.Defense);
        _PlayerModifiedDefenseClass = _PlayerModifiedDefenseClass + _Shield.GetSystemModifier(ShipSystem.ModifierType.Defense);

        //Sensor
        _PlayerModifiedDefenseClass = _PlayerModifiedDefenseClass + _Sensor.GetHelmModifier(ShipSystem.ModifierType.Defense);
        _PlayerModifiedDefenseClass = _PlayerModifiedDefenseClass + _Sensor.GetSystemModifier(ShipSystem.ModifierType.Defense);

        //Pilot
        _PlayerModifiedDefenseClass = _PlayerModifiedDefenseClass + _Pilot.GetHelmModifier(ShipSystem.ModifierType.Defense);
        _PlayerModifiedDefenseClass = _PlayerModifiedDefenseClass + _Pilot.GetSystemModifier(ShipSystem.ModifierType.Defense);

        return _PlayerModifiedDefenseClass;
    }

    public int GetAttackModifier()
    {
        int _PlayerModifiedAttackRating;
        
        _PlayerModifiedAttackRating = 0;
        //Shield
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Shield.GetHelmModifier(ShipSystem.ModifierType.Attack);
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Shield.GetSystemModifier(ShipSystem.ModifierType.Attack);
        
        //Sensor
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Sensor.GetHelmModifier(ShipSystem.ModifierType.Attack);
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Sensor.GetSystemModifier(ShipSystem.ModifierType.Attack);

        //Weapon
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Weapon.GetHelmModifier(ShipSystem.ModifierType.Attack);
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Weapon.GetSystemModifier(ShipSystem.ModifierType.Attack);

        //Engineering
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Engineering.GetHelmModifier(ShipSystem.ModifierType.Attack);
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Engineering.GetSystemModifier(ShipSystem.ModifierType.Attack);

        //Pilot
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Pilot.GetHelmModifier(ShipSystem.ModifierType.Attack);
        _PlayerModifiedAttackRating = _PlayerModifiedAttackRating + _Pilot.GetSystemModifier(ShipSystem.ModifierType.Attack);

        return _PlayerModifiedAttackRating;
    }

    public void LaunchMissile()
    {
        int _RollNeededToHit = 0;
        int _DeeTwentyDiceRoll;
        int _NetRoll;

        GameObject _Missile = Instantiate(_MissilePrefab, _MissileMuzzle.position, _MissileMuzzle.rotation);
        Rigidbody2D _MissileRigidbody = _Missile.GetComponent<Rigidbody2D>();
        _MissileRigidbody.AddForce(_MissileMuzzle.up * _MissileForce, ForceMode2D.Impulse);
        
        _RollNeededToHit = _PlayerBaseAttackRating + _EnemyShipHandler.GetDefenseClass();


        //GetAttackModifier()
        //Chance to Hit
        //Base Change to HIT = 14
        //Roll 1-D20 (1-20) + Attack Rating (0-8) = 1-28
        //Ships Attack Rating (1 - 20)
        //Ships Critical Rating (19) or (19-20)
        //Then Enemies Defense Class is 2;
        //Example: Players Ship has AR of 14 and does critical on 20 but the Enemy has an DC of 2 ::(16-20) is a Hit and (20) is a critical

        LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("PlayerLaunched") + _RollNeededToHit + " to hit.");
        //LogHandler.Instance.NewLogEntry("Attack Rating (" + _PlayerBaseAttackRating + ") + Enemy Ship Defense Class (" + _EnemyShipHandler.GetDefenseClass() + ") =" + _RollNeededToHit );

        _DeeTwentyDiceRoll = GameHandler.Instance.OneDeeTwenty();
        _NetRoll = _DeeTwentyDiceRoll + GetAttackModifier();

        if (_DeeTwentyDiceRoll == 20)
        {
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("PlayerRolledTwenty") +"\n");
            Destroy(_Missile, _MissleHitsDestroyDelay);
            StartCoroutine(MissileExplosion());

            return;
        }

        if (_DeeTwentyDiceRoll == 1)
        {
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("PlayerRolledOne") +"\n");
            Destroy(_Missile, _MissleMissesDestroyDelay);
            return;
        }

        LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("PlayerRolled") + " (" + (_DeeTwentyDiceRoll) + " + " + GetAttackModifier() + ") = " + _NetRoll );



        if (_NetRoll >= (_RollNeededToHit))
        {
            //Missile Hits
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("MissileHitsEnemy") + "\n");
            Destroy(_Missile, _MissleHitsDestroyDelay);
            StartCoroutine(MissileExplosion());
        }
        else
        {
            //Missile Misses
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("MissileMissiesEnemy") + "\n");
            Destroy(_Missile, _MissleMissesDestroyDelay);
        }
    }

    public IEnumerator MissileExplosion()
    {
        yield return new WaitForSeconds(_MissleHitsDestroyDelay + .1f);

        float Random_X;
        float Random_Y;
        
        _EnemyShipHandler._HealthSystem.SubtractHealth(GameHandler.Instance.DamageRoll(_MissileMinDamage, _MissileMaxDamage));

        //if (_ShowDebug) Debug.Log("Explosion Special Effect!");

        Vector3 _RandomMissileHitPosition;

        Random_X = UnityEngine.Random.Range(1500f, 1600f);
        Random_Y = UnityEngine.Random.Range(1300f, 1500f);
        
        _RandomMissileHitPosition = new Vector3(Random_X, Random_Y, 5);

        GameObject _Effect = Instantiate(_Sfx_HullExplosion, _RandomMissileHitPosition, Quaternion.identity);

        Destroy(_Effect, _ExplosionSoundDuration);
    }

    public void AlarmCheck()
    {
        if (_EnemyIntruderAlertLevel == EnemyIntruderAlertLevel.Off)
        {
            _AlarmIntruder.Stop();
        }
        if (_EnemyIntruderAlertLevel == EnemyIntruderAlertLevel.Yellow)
        {
            _AlarmIntruder.Stop();
        }
        if (_EnemyIntruderAlertLevel == EnemyIntruderAlertLevel.Red)
        {
            if (!_AlarmIntruder.isPlaying)
            {
                _AlarmIntruder.Play();
            }
        }
        
        //Debug.Log("Alert Level: " + _EnemyShipAlertLevel);

        if (_EnemyShipAlertLevel == EnemyShipAlertLevel.Off)
        {
            _AlarmEnemyShipDetected.Stop();
        }
        if (_EnemyShipAlertLevel == EnemyShipAlertLevel.Yellow)
        {
            _AlarmEnemyShipDetected.Stop();
        }
        if (_EnemyShipAlertLevel == EnemyShipAlertLevel.Red)
        {
            if (!_AlarmEnemyShipDetected.isPlaying)
            {
                _AlarmEnemyShipDetected.Play();
            }
        }
    }

    //Events
    private void _HealthSystem_OnStaminaRecovered(object sender, EventArgs e)
    {
        _IsCrewExhausted = false;
    }
    private void _HealthSystem_OnStaminaExhausted(object sender, EventArgs e)
    {
        _IsCrewExhausted = true;
    }
    private void _HealthSystem_OnStaminaChanged(object sender, EventArgs e)
    {
        if (_HealthSystem.GetStaminaPercent() < .01f)
        {
            Achievements.Instance._RanOutOfPower = true;
        }
    }
    private void _HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        if (_HealthSystem.GetHealthPercent() <.05f)
        {
            Achievements.Instance._IsFivePercentHull = true;
        }
    }
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        _IsDead = true;
        _IsDogfight = false;

        GameHandler.Instance.GameOver();
    }
}
