using CodeMonkey.Utils;
using System;
using UnityEngine;

public class HullDamage : MonoBehaviour, IDamageable
{
    bool _ShowDebug = false;

    private Player _Player;
    private PlayerWeapons _PlayerWeapons;
    private PlayerShipHandler _PlayerShipHandler;
    public DamageSystem _DamageSystem;
    public bool _IsPlateOk = false;
    public float _HullDamageTimer = 0;

    [SerializeField] private float _MinimumDamageScale=.25f;
    [SerializeField] private float _MaximumDamageScale=3.0f;
    [SerializeField] private Transform _DamagePrefabTransform;
    [SerializeField] private SpriteRenderer _MinimapDot;
    [SerializeField] private float _StartingHealth =100;
    [SerializeField] private float _PlayerRepairAmount = 12.5f;
    [SerializeField] private ParticleSystem _DamageParticles;
    [SerializeField] private SpriteRenderer _DamageImage;
    [SerializeField] private AudioSource _DamageSound;
    [SerializeField] private float _HullDamageModifier = 5f;
    [SerializeField] private float _StartDamageSeconds = 15f;
    [SerializeField] private float _IncrementalDamage = 1f;  //Incremental damager every (_StartDamageSeconds)

    private void Awake()
    {
        _DamageSystem = new DamageSystem(_StartingHealth,0,_StartingHealth);
    }

    private void Start()
    {
        _Player = GameObject.FindObjectOfType<Player>();
        _PlayerWeapons = GameObject.FindObjectOfType<PlayerWeapons>();
        _PlayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();

        _DamageSystem_OnStartedWorking(null, null);   //Update Hull Damage Status

        //Events
        _DamageSystem.OnHealthChanged += _DamageSystem_OnHealthChanged;
        _DamageSystem.OnStartedWorking += _DamageSystem_OnStartedWorking;
        _DamageSystem.OnStoppedWorking += _DamageSystem_OnStoppedWorking;

        GameHandler.Instance.OnSaveAllData += Instance_OnSaveAllData;

        LoadData();
    }

    private void Instance_OnSaveAllData(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(this.name + "_IsPlateOk", _IsPlateOk);
        ES3.Save(this.name + "_HullDamageTimer", _HullDamageTimer);

        ES3.Save(this.name + "_DamageSystem._CurrentHealth", _DamageSystem._CurrentHealth);
        ES3.Save(this.name + "_DamageSystem._IsWorking", _DamageSystem._IsWorking);
    }

    public void LoadData()
    {
        try
        {
            _IsPlateOk = ES3.Load<bool>(this.name + "_IsPlateOk");
            _HullDamageTimer = ES3.Load<float>(this.name + "_HullDamageTimer");

            Damage(0, false, true,false);
            Repair(0);

            _DamageSystem._CurrentHealth = ES3.Load<float>(this.name + "_DamageSystem._CurrentHealth");
            _DamageSystem._IsWorking = ES3.Load<bool>(this.name + "_DamageSystem._IsWorking");

            _DamageSystem.SubtractHealth(0);
            _DamageSystem.AddHealth(0);
        }
        catch { }
    }

    private void Update()
    {
        if (_IsPlateOk)
        {
            _HullDamageTimer = 0;
        }
        else
        {
            _HullDamageTimer += Time.deltaTime;
        }
        
        if (_HullDamageTimer > _StartDamageSeconds)
        {
            _PlayerShipHandler.DamageShip(_IncrementalDamage);
            _HullDamageTimer = 0;
        }
    }

    public float GetDamagePercent()
    {
        return _DamageSystem.GetDamagePercent();
    }

    public void Damage(float pAmount, bool IsBullet, bool NoDamageToPlayer, bool IsExplosion)
    {
        if (!IsBullet) _DamageSystem.SubtractHealth(pAmount * _HullDamageModifier);
    }

    public void LowOxygen(float value)
    {
        //Do Not Remove - Required by IDamageable
    }

    public void Repair(float pAmount)
    {
        _DamageSystem.AddHealth(pAmount);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_IsPlateOk)
        {
            return;
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                //TODO: Repair of Hull Damage (_CurrentWeapon) needs to be replace instead of hardcoaded to 4
                if (_PlayerWeapons._CurrentWeapon.GetItemId() == 4)
                {
                    //TODO: Player repair amount should come from the player based on current tool & skill
                    Repair(_PlayerRepairAmount * Time.deltaTime);
                }
            }
        }
    }

    //Events
    private void _DamageSystem_OnStoppedWorking(object sender, System.EventArgs e)
    {
        _IsPlateOk = false;
        _DamageParticles.Play();
        _DamageImage.enabled = true;
        _MinimapDot.enabled = true;
        _DamageSound.Play();

    }
    private void _DamageSystem_OnStartedWorking(object sender, System.EventArgs e)
    {
        _IsPlateOk = true;
        _DamageParticles.Stop();
        _DamageImage.enabled = false;
        _MinimapDot.enabled = false;
        _DamageSound.Stop();
    }
    private void _DamageSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        float _DamageImageScale;

        if (!_IsPlateOk)
        {
            //TODO:  The Hull Damage pitch and volume need to be reworked
            _DamageImageScale = Mathf.Clamp((_DamageSystem.GetDamagePercent() * _MaximumDamageScale), _MinimumDamageScale, _MaximumDamageScale);
            _DamagePrefabTransform.localScale = new Vector3(_DamageImageScale, _DamageImageScale, 1);
            _DamageSound.volume = ((.3f * _DamageSystem.GetDamagePercent()) + .5f);
            _DamageSound.pitch = ((1 * _DamageSystem.GetDamagePercent()) + 1);
        }
    }
}
