using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum EnemyType { Melee, Ranged, Boss}
    public enum BloodType { Blood_Red_Large, Blood_Red_Small, Blood_Red_Death, Blood_Green_Large, Blood_Green_Small, Blood_Green_Death, Blood_Black_Large, Blood_Black_Small, Blood_Black_Death, Blood_Orange_Large, Blood_Orange_Small, Blood_Orange_Death, }

    //Core Game Object
    public HealthSystem _HealthSystem;

    public EnemyType _EnemyType = EnemyType.Melee;
    public BloodType _Blood_Large = BloodType.Blood_Red_Large;
    public BloodType _Blood_Small = BloodType.Blood_Red_Small;
    public BloodType _Blood_Death = BloodType.Blood_Red_Death;
    
    public float _StartingHealth = 100;
    private int _BloodCount;

    
    private ObjectPooler _ObjectPooler;

    private void Awake()
    {
        _HealthSystem = new HealthSystem(0, _StartingHealth, 0, 0);
    }

    private void Start()
    {
        _ObjectPooler = ObjectPooler.Instance;

        //Subscribe to Events
        _HealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
        _HealthSystem.OnDeath += HealthSystem_OnDeath;
    }

    public void Damage(float Amount,bool IsBullet, bool NoDamageToPlayer, bool IsExplosion)
    {
        float _CriticalChance = 10f;

        if (UnityEngine.Random.Range(0,100)<=_CriticalChance)
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

        if (_HealthSystem._CurrentHealth == 0)
        {
            if(IsBullet == false && NoDamageToPlayer == false && IsExplosion == false)
            {
                Achievements.Instance._EnemyKilledByHands = true;
            }
        }

        SplatterBlood(Amount);
    }

    public void LowOxygen(float value)
    {
        //Do Not Remove - Required by IDamageable
        _HealthSystem.SubtractHealth(value);
    }

    private void SplatterBlood(float pSize)
    {
        float _SplatterX = UnityEngine.Random.Range(-6f, 6f);
        float _SplatterY = UnityEngine.Random.Range(-6f, 6f);
        float LargeSplatterSize = 5f;

        Vector3 _NewPosition = new Vector3(this.transform.position.x + _SplatterX,
                                                this.transform.position.y + _SplatterY,
                                                this.transform.position.z);

        if (pSize > LargeSplatterSize)
        {
            if (_BloodCount >5)
            {
                GameObject _Blood = _ObjectPooler.SpawnFromPool(_Blood_Large.ToString(), _NewPosition, transform.rotation);
            }
            _BloodCount++;
            
        }
        else
        {
            if (_BloodCount >3 )
            {
                GameObject _Blood = _ObjectPooler.SpawnFromPool(_Blood_Large.ToString(), _NewPosition, transform.rotation);
            }
            _BloodCount++;
        }
    }

    public HealthSystem GetHealthSystem()
    {
        return _HealthSystem;
    }

    //Events
    private void _HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        //TODO:_HealthSystem_OnHealthChanged
    }
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {
        GameObject _Blood = _ObjectPooler.SpawnFromPool(_Blood_Death.ToString(), transform.position, transform.rotation);

        Achievements.Instance.EnemyKilled();

        switch (_EnemyType)
        {
            case EnemyType.Melee:
                PlayerPoints.Instance.AddPoints(100,"Killing an Alien Grunt");
                break;
            case EnemyType.Ranged:
                PlayerPoints.Instance.AddPoints(200, "Killing an Alien Sniper");
                break;
            case EnemyType.Boss:
                PlayerPoints.Instance.AddPoints(400, "Killing an Alien Boss");
                break;
            default:
                break;
        }

        Destroy(this.transform.gameObject);
    }
}
