using UnityEngine;
using System;
public class DamageSystem
{
    //Events
    public event EventHandler OnHealthChanged;
    public event EventHandler OnStoppedWorking;
    public event EventHandler OnStartedWorking;

    public float _CurrentHealth;
    public float _MaxHealth;
    public float _AutomaticRecoverAmount;
    public float _RequiredHealthToWork;
    public bool _IsWorking = true;

    public DamageSystem(float Health, float AutomaticRecoverAmount, float RequiredHealthToWork)
    {
        _CurrentHealth = Health;
        _MaxHealth = Health;
        _AutomaticRecoverAmount = AutomaticRecoverAmount;
        _RequiredHealthToWork = RequiredHealthToWork;
    }
    
    public void SubtractHealth(float Amount)
    {
        _CurrentHealth = Mathf.Clamp((_CurrentHealth - Amount), 0, _MaxHealth);

        if (_CurrentHealth < _RequiredHealthToWork)
        {
            _IsWorking = false;
            OnStoppedWorking?.Invoke(this, EventArgs.Empty);
        }
        
        HealthChanged();
    }

    public void AddHealth(float Amount)
    {
        _CurrentHealth = Mathf.Clamp((_CurrentHealth + Amount), 0, _MaxHealth);

        if (_CurrentHealth >= _RequiredHealthToWork)
        {
            _IsWorking = true;
            OnStartedWorking?.Invoke(this, EventArgs.Empty);
        }

        HealthChanged();
    }

    public void Repair(float DeltaTime)
    {
        if (_CurrentHealth == _MaxHealth) return;

        float Value = (_AutomaticRecoverAmount * DeltaTime);
        
        AddHealth(Value);
    }

    public void HealthChanged()
    {
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercent()
    {
        return (_CurrentHealth / _MaxHealth);
    }
    
    public float GetDamagePercent()
    {
        return (1 - (_CurrentHealth / _MaxHealth));
    }

    public float GetHealthAmount()
    {
        return _CurrentHealth;
    }

}
