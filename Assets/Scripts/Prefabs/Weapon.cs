using System;
using UnityEngine;

public class Weapon : MonoBehaviour, IInventoryItem
{
    //Events
    public event EventHandler OnHeatChanged;
    public event EventHandler OnStoppedWorking;
    public event EventHandler OnStartedWorking;


    public int _ItemId;
    public string _Name;
    public float _CurrentHeat;
    public float _MaxHeat;
    public float _MinHeat;
    public float _MaxOperatingTemp;
    public float _MinOperatingTemp;
    public float _CoolAmount;
    public float _ShootHeat;
    public float _FireRate;
    //public float _BulletForce;
    //public float _BulletLifespan = 2;
    public Transform _Muzzle;
    public Transform _WeaponSpriteImage;
    public string _BulletPrefab = "Bullet_Player_LaserPistol";
    private float _NextTimeFire = 0;
    public SpriteRenderer _SpriteRenderer;

    
    public bool _IsCurrentWeapon = false;
    public bool _IsOverHeated = false;
    public bool _IsInInventory = true;

    ObjectPooler _ObjectPooler;


    private void Start()
    {
        _ObjectPooler = ObjectPooler.Instance;
    }

    private void FixedUpdate()
    {
        SubtractHeat((_CoolAmount * Time.deltaTime)); //Cooling
    }
    
    public int GetItemId()
    {
        return _ItemId;
    }

    public void Fire()
    {
        if (_IsOverHeated) return;

        if (Time.time > _NextTimeFire)
        {

            AddHeat(_ShootHeat); //Used to Generate Heat on the Weapon
            GameObject _Bullet = _ObjectPooler.SpawnFromPool(_BulletPrefab, _Muzzle.position, _Muzzle.rotation);


            //Todo: Fix this Constant
            _NextTimeFire = Time.time + _FireRate;
        }
    }

    public void SubtractHeat(float Amount)
    {
        _CurrentHeat = Mathf.Clamp((_CurrentHeat - Amount), _MinHeat, _MaxHeat);

        if (_CurrentHeat <= _MinOperatingTemp)
        {
            if (_IsOverHeated)
            {
                _IsOverHeated = false;
                OnStartedWorking?.Invoke(this, EventArgs.Empty);
            }
        }

        HeatChanged();
    }

    public void AddHeat(float Amount)
    {
        _CurrentHeat = Mathf.Clamp((_CurrentHeat + Amount), _MinHeat, _MaxHeat);

        if (_CurrentHeat >= _MaxOperatingTemp)
        {
            if (_IsOverHeated == false)
            {
                _IsOverHeated = true;
                OnStoppedWorking?.Invoke(this, EventArgs.Empty);
            }
        }
        HeatChanged();
    }

    private void HeatChanged()
    {
        OnHeatChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetHeatPercentage()
    {
        return (_CurrentHeat / _MaxHeat);
    }

}

