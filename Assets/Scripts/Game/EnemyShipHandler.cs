using UnityEngine;
using System;
using System.Collections;

public class EnemyShipHandler : MonoBehaviour
{
    bool _ShowDebug = true;
    
    //TODO: Fix this below
    //public Transform _MissileTarget; 
    
    //Core Game Object
    public HealthSystem _HealthSystem;
    public PlayerShipHandler _PlayerShipHandler;
    public string _MissilePrefab = "Missile_Enemy_A";
    public Transform _MissileMuzzle;
    
    public GameObject _Sfx_HullExplosion;
    public GameObject _Sfx_ShipExplodes;
    private float _ExplosionSoundDuration = 2.0f;
    [SerializeField] private float _StaminaUsedToFireMissile = 8;

    [SerializeField]

    private float _RestingStaminaRecoverAmount = 1.75f;                  
    private float _StartingHealth = 100;                                
    private float _StartingStamina = 100;                               
    private float _RequiredStaminaToRecover = 40;

    public int _EnemyBaseAttackRating = 12;
    public int _EnemyBaseDefenseClass = 1;
    public float _EnemyBaseRateOfFire = 10;
    

    private float _MissileForce = 500;                                  
    private float _MissileHitsDestroyDelay = 3.5f;
    private float _MissileMissesDestroyDelay = 8;

    //This should come from the missile
    private int _MissileMinDamage = 6;
    private int _MissileMaxDamage = 12;
    
    //Private Variables
    private float _NextShootTime = 0;
    private bool _IsCrewExhausted = false;
    private bool _IsDead = false;
    private bool _IsDogfight = false;

    public Transform _MissileTarget;

    Vector3 _RandomMissileHitPosition;

    ObjectPooler _ObjectPooler;

    private void Awake()
    {
        _HealthSystem = new HealthSystem(_RestingStaminaRecoverAmount, _StartingHealth, _StartingStamina, _RequiredStaminaToRecover);
    }
    
    void Start()
    {
        //Subscribe to Events
        _HealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
        _HealthSystem.OnDeath += HealthSystem_OnDeath;
        _HealthSystem.OnStaminaChanged += _HealthSystem_OnStaminaChanged;
        _HealthSystem.OnStaminaExhausted += _HealthSystem_OnStaminaExhausted;
        _HealthSystem.OnStaminaRecovered += _HealthSystem_OnStaminaRecovered;

        _PlayerShipHandler = GameObject.Find("PlayerShip").GetComponent<PlayerShipHandler>();

        LogHandler.Instance.NewLogEntry("Enemy Ship Detected by the Sensors!\n");

        _ObjectPooler = ObjectPooler.Instance;
        
    }

    void Update()
    {
        if (!_IsDead)
        {
            if (_IsDogfight)
            {
                Dogfight();
            }
            _HealthSystem.Rest(_RestingStaminaRecoverAmount * Time.deltaTime);
        }
    }

    public int GetDefenseClass()
    {
        return _EnemyBaseDefenseClass;
    }

    public HealthSystem GetHealthSystem()
    { return _HealthSystem; }

    public void IsDead(bool value)
    {
        _IsDead = value;
    }

    public void IsDogFight(bool value)
    {
        _IsDogfight = value;
    }

    public void Dogfight()
    {
        if (!_IsCrewExhausted)
        {
            if (Time.time > _NextShootTime)
            {
                LaunchMissile();

                _HealthSystem.SubtractStamina(_StaminaUsedToFireMissile);

                _NextShootTime = Time.time + (_EnemyBaseRateOfFire - (2 * _HealthSystem.GetStaminaPercent())) ;
            }
        }
    } //Two ships are in proximity and are fighting

    public void LaunchMissile()
    {
        int _RollNeededToHit = 0;
        int _DeeTwentyDiceRoll;
        int _NetRoll;

        //GameObject _Missile = Instantiate(_MissilePrefab, _MissileMuzzle.position, _MissileMuzzle.rotation);  //Replacing with Object Pool

        GameObject _Missile = _ObjectPooler.SpawnFromPool(_MissilePrefab, _MissileMuzzle.position, _MissileMuzzle.rotation);

        

        Rigidbody2D _MissileRigidbody = _Missile.GetComponent<Rigidbody2D>();
        _MissileRigidbody.AddForce(_MissileMuzzle.up * _MissileForce, ForceMode2D.Impulse);

        _RollNeededToHit = _EnemyBaseAttackRating + _PlayerShipHandler.GetDefenseClass();


        LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("EnemyLaunched") + _RollNeededToHit + " to hit.");
        //LogHandler.Instance.NewLogEntry("Attack Rating (" + _EnemyBaseAttackRating + ") + Player Ship Defense Class (" + _PlayerShipHandler.GetDefenseClass() + ") =" + _RollNeededToHit);

        _DeeTwentyDiceRoll = GameHandler.Instance.OneDeeTwenty();
        _NetRoll = _DeeTwentyDiceRoll;

        if (_DeeTwentyDiceRoll == 20)
        {
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("EnemyRolledTwenty") + "\n");
            //Destroy(_Missile, _MissileHitsDestroyDelay);
            StartCoroutine(DisableAfterDelay(_Missile, _MissileHitsDestroyDelay));
            StartCoroutine(MissileExplosion());
            return;
        }

        if (_DeeTwentyDiceRoll == 1)
        {
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("EnemyRolledOne") + "\n");
            //Destroy(_Missile, _MissileMissesDestroyDelay);
            StartCoroutine(DisableAfterDelay(_Missile, _MissileMissesDestroyDelay));
            return;
        }


        LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("EnemyRolledOne") + (_DeeTwentyDiceRoll));

        if (_NetRoll >= (_RollNeededToHit))
        {
            //Missile Hits
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("MissileHitsPlayer") + "\n");
            
            StartCoroutine(DisableAfterDelay(_Missile, _MissileHitsDestroyDelay));

            float Random_X;
            float Random_Y;
            Random_X = UnityEngine.Random.Range(-500f, 800f);
            Random_Y = UnityEngine.Random.Range(-300f, 200f);
            _RandomMissileHitPosition = new Vector3(Random_X, Random_Y, 5);
            
            //TODO: Fix This
            _MissileTarget.position = _RandomMissileHitPosition;
            _MissileTarget.gameObject.SetActive(true);
            StartCoroutine(MissileExplosion());
        }
        else
        {
            //Missile Misses
            LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("MissileMissesPlayer") + "\n");
            //Destroy(_Missile, _MissileMissesDestroyDelay);

            StartCoroutine(DisableAfterDelay(_Missile, _MissileMissesDestroyDelay));
        }
        
    }

    private IEnumerator DisableAfterDelay(GameObject pObject, float pValue)
    {
        yield return new WaitForSeconds(pValue);

        pObject.SetActive(false);
    }

    public IEnumerator MissileExplosion()
    {
        yield return new WaitForSeconds(_MissileHitsDestroyDelay + .1f);

        _PlayerShipHandler._HealthSystem.SubtractHealth(GameHandler.Instance.DamageRoll(_MissileMinDamage, _MissileMaxDamage));

        Achievements.Instance._BombBlastCount++;

        //TODO: Fix This
        _MissileTarget.gameObject.SetActive(false);

        GameObject _Effect = Instantiate(_Sfx_HullExplosion, _RandomMissileHitPosition, Quaternion.identity);
        Destroy(_Effect, _ExplosionSoundDuration);
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
    }
    private void _HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        
    }
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        _IsDead = true;
        _IsDogfight = false;

        LogHandler.Instance.NewLogEntry(Lean.Localization.LeanLocalization.GetTranslationText("MissileMissesPlayer"));

        Vector3 _MissileHitPosition = new Vector3(1450,1000,0);

        GameObject _Effect = Instantiate(_Sfx_ShipExplodes, _MissileHitPosition, Quaternion.identity);
        Destroy(_Effect, _ExplosionSoundDuration);
        
        GameHandler.Instance.EnemyShipDestroyed();

        this.transform.gameObject.SetActive(false);
        //Destroy(this, 5);
    }
}
