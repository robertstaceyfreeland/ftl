using UnityEngine;
using System;


public class Player : MonoBehaviour, IDamageable
{
    public HealthSystem _HealthSystem { get; private set; }
    
    public string _Blood_Large = "Blood_Red_Large";
    public string _Blood_Small = "Blood_Red_Small";

    public float _RestingStaminaAmount = 10;
    public float _StartingHealth = 100;
    public float _StartingStamina = 100;
    public float _RequiredStaminaToRecover = 80;
    public float _LowOxygenStaminaModifier = 3;
    private float _BloodCount;

    ObjectPooler _ObjectPooler;

    private void Awake() //Instantiates the HealthSystem for the Player
    {
        _HealthSystem = new HealthSystem(_RestingStaminaAmount, _StartingHealth, _StartingStamina, _RequiredStaminaToRecover);
    }

    private void Start()
    {
        _ObjectPooler = ObjectPooler.Instance;

        _HealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
        _HealthSystem.OnDeath += HealthSystem_OnDeath;
        _HealthSystem.OnStaminaChanged += _HealthSystem_OnStaminaChanged;
        
        GameHandler.Instance.OnSaveAllData += Instance_OnSaveAllData;

        LoadData();
    }

    private void FixedUpdate()
    {
        _HealthSystem.Rest(_RestingStaminaAmount*Time.deltaTime);
    }

    private void Instance_OnSaveAllData(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(this.name + "transform.position", transform.position);

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
            //Player
            transform.position = ES3.Load<Vector3>(this.name + "transform.position");

            //Player Health
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

    public void SetHealthModifier(float pAmount)
    {
        _HealthSystem.SetMaxHealth( _StartingHealth + pAmount);
    }

    public void SetStaminaModifier(float pAmount)
    {
        _HealthSystem.SetMaxStamina(_StartingStamina + pAmount);
    }

    public void Heal(float Amount)
    {
        float AmountHealed;

        AmountHealed = _HealthSystem.AddHealth(Amount);

        Achievements.Instance._HealingCount += AmountHealed;
    }

    public void Damage(float Amount, bool IsBullet, bool NoDamageToPlayer, bool IsExplosion) 
    {
        if (!NoDamageToPlayer)
        {
            float _CriticalChance = 10f;

            if (UnityEngine.Random.Range(0, 100) <= _CriticalChance)
            {
                //Critical Hit
                Amount = (Amount * 1.5f);
                Popup.Create(transform.position, (int)Amount, true);
            }
            else
            {
                Popup.Create(transform.position, (int)Amount, false);
            }
            _HealthSystem.SubtractHealth(Amount);

            SplatterBlood(Amount);
        }

        if (IsExplosion)
        {
            Achievements.Instance._BombBlastCount = 0;
        }
        
    }

    public void LowOxygen(float value)
    {
        //Do Not Remove - Required by IDamageable
        _HealthSystem.SubtractHealth(value);

        if (_HealthSystem._CurrentHealth == 0)
        {
            Achievements.Instance._EnemySuffocated = true;
        }
        _HealthSystem.SubtractStamina(value * _LowOxygenStaminaModifier);
    }

    private void SplatterBlood(float pSize)  //Spawn Blood Splatter based on amount of damage
    {
        _BloodCount += 1;

        if (_BloodCount >= 10)
        {
            float _SplatterX = UnityEngine.Random.Range(-6f, 6f);
            float _SplatterY = UnityEngine.Random.Range(-6f, 6f);
            float LargeSplatterSize = 3f;

            Vector3 _NewPosition = new Vector3(this.transform.position.x + _SplatterX,
                                                   this.transform.position.y + _SplatterY,
                                                   this.transform.position.z);

            //TODO: convert this to one object that is scaled based on damage
            if (pSize > LargeSplatterSize)
            {
                GameObject _Blood = _ObjectPooler.SpawnFromPool(_Blood_Large, _NewPosition, transform.rotation);
            }
            else
            {
                GameObject _Blood = _ObjectPooler.SpawnFromPool(_Blood_Small, _NewPosition, transform.rotation);
            }
        }
    }

    public HealthSystem GetHealthSystem()
    {
        return _HealthSystem;
    }

    //Events
    private void _HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        if (_HealthSystem.GetHealthPercent() <= .05f)
        {
            Achievements.Instance._IsFivePercentHealth = true;
        }
    }
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        GameObject obj = this.transform.gameObject;
        Destroy(obj.gameObject);

        GameHandler.Instance.GameOver();
    }
    private void _HealthSystem_OnStaminaChanged(object sender, EventArgs e)
    {
        //TODO:_HealthSystem_OnStaminaChanged
    }



}
