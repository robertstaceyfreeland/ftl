using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Oxygen : MonoBehaviour
{

    public event EventHandler OnOxygenChanged;
    public event EventHandler OnDoesNotHaveOxygen;
    public event EventHandler OnDoesHaveOxygen;

    public HullDamage _HullDamage;
    
    private OxygenGenerator _OxygenGenerator;
    private SpriteRenderer _OxygenLayer;

    public float _CurrentOxygen = 100;
    private float _MaxOxygen = 100;
    private float _RequiredOxygenToBreath = 75;
    private float _LeakConstant = 12.5f;
    private float _OxygenConstantLoss = .004f;
    private float _LeakAmount = 0;
    private float _LeakPercent = 0;
    private float _NextTakeDamageTime = 0;
    private float _OxygenDeprivationConstant = 3.5f;

    //Sprite will turn red from (_RequiredOxygenToBreath) - 0
    //Sprite will be clear at Greater than (_RequiredOxygenToBreath) and will transition to a transparent red at 0
    //Alpha will range from 0 (Clear) to 1 (Semi Red)!  NOTE: 0-1 in api not 255
    //The base color will be Red;

    private void Awake()
    {
        _OxygenLayer = transform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetAlpha();

        _OxygenGenerator = GameObject.Find("Helm_LifeSupport").GetComponent<OxygenGenerator>();

        GameHandler.Instance.OnSaveAllData += Instance_OnSaveAllData;

        LoadData();
    }

    private void Instance_OnSaveAllData(object sender, EventArgs e)
    {
        SaveData();
    }

    public void SaveData()
    {
        ES3.Save(this.name + "_CurrentOxygen", _CurrentOxygen);
    }

    public void LoadData()
    {
        try
        {
            _CurrentOxygen = ES3.Load<float>(this.name + "_CurrentOxygen");
        }
        catch { }
    }

    void Update()
    {
        SetAlpha();
    }

    private void FixedUpdate()
    {
        LeakCheck();
        ConstantOxygenLoss();
        RequestOxygenFromLifeSupport();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_NextTakeDamageTime<Time.time)
        {
            IDamageable[] _Damageable = collision.GetComponents<IDamageable>();

            for (int i = 0; i < _Damageable.Length; i++)
            {
                if (_Damageable[i] != null)
                {
                    _Damageable[i].LowOxygen(GetOxygenDeprivationPercent() * _OxygenDeprivationConstant);
                }
            }
            _NextTakeDamageTime = Time.time + 1;
        }
    }

    private void LeakCheck()
    {
        _LeakAmount = 0;

        if (_HullDamage == null) return;
        
        _LeakPercent = _HullDamage.GetDamagePercent();

        if (_LeakPercent > 0)
        {
            _LeakAmount =  _LeakPercent * _LeakConstant * Time.fixedDeltaTime;
            
            SubtractOxygen(_LeakAmount);
        }
    }

    private void RequestOxygenFromLifeSupport()
    {
        float _OxygenAmountFromLifeSupport = 0;
        float _OxygenNeeded = 0;

        _OxygenNeeded = Mathf.Clamp((_MaxOxygen - _CurrentOxygen), 0, _MaxOxygen);

        _OxygenAmountFromLifeSupport = _OxygenGenerator.RequestOxygen(_OxygenNeeded);

        AddOxygen(_OxygenAmountFromLifeSupport);
    }

    private void ConstantOxygenLoss()
    {
        SubtractOxygen((_OxygenConstantLoss*_CurrentOxygen) * Time.fixedDeltaTime);
    }

    private void SetAlpha()
    {
        float Alpha;
        Alpha = Mathf.Clamp(_CurrentOxygen, 0, _RequiredOxygenToBreath);
        Alpha = Alpha / _RequiredOxygenToBreath;
        Alpha = 1 - Alpha;

        Color tmp = _OxygenLayer.color;
        tmp.a = (Alpha*.4f);
        _OxygenLayer.color = tmp;
    }

    public void SubtractOxygen(float pOxygenAmount)
    {
        _CurrentOxygen = Mathf.Clamp((_CurrentOxygen - pOxygenAmount), 0, _MaxOxygen);

        if (_CurrentOxygen <= _RequiredOxygenToBreath)
        {
            OnDoesNotHaveOxygen?.Invoke(this, EventArgs.Empty);
        }

        OnOxygenChanged?.Invoke(this,EventArgs.Empty);
    }

    public void AddOxygen(float Amount)
    {
        _CurrentOxygen = Mathf.Clamp((_CurrentOxygen + Amount), 0, _MaxOxygen);

        if (_CurrentOxygen > _RequiredOxygenToBreath)
        {
            OnDoesHaveOxygen?.Invoke(this, EventArgs.Empty);
        }

        OnOxygenChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetCurrentOxygenAmount()
    {
        return _CurrentOxygen;
    }

    public float GetOxygenDeprivationPercent()
    {
        return (1 - Mathf.Clamp((_CurrentOxygen / _RequiredOxygenToBreath), 0, 1));
    }
}
