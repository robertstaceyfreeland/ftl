using UnityEngine;
using System;

public class HealthSystem
{
    //Events
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDeath;
    public event EventHandler OnStaminaChanged;
    public event EventHandler OnStaminaExhausted;
    public event EventHandler OnStaminaRecovered;

    public float _CurrentHealth;
    //private float _MinHealth;
    public float _MaxHealth;
    public float _CurrentStamina;
    //private float _MinStamina;
    public float _MaxStamina;
    public float _RestingStaminaRecoverAmount;
    public float _RequiredStaminaToRecover;

    public bool _IsInvincible;
    public bool _IsExhausted = false;

    public float _StaminaUsedThisTick = 0;

    public HealthSystem(float RestingStaminaRecoverAmount, float Health, float Stamina, float RequiredStaminaToRecover)
    {
        _IsInvincible = false;
        _RestingStaminaRecoverAmount = RestingStaminaRecoverAmount;
        _CurrentHealth = Health;
        _MaxHealth = Health;
        _CurrentStamina = Stamina;
        _MaxStamina = Stamina;
        _RequiredStaminaToRecover = RequiredStaminaToRecover;
        //_MinStamina = 0;
    }

    public void SetCurrentHealth(float pAmount)
    {
        if (pAmount > _MaxHealth)
        {
            _MaxHealth = pAmount;
        }
        
        _CurrentHealth = Mathf.Clamp(pAmount,0,_MaxHealth);
        
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetMaxHealth(float pAmount)
    {
        _MaxHealth = pAmount;

        _CurrentHealth = Mathf.Clamp(_CurrentHealth, 0, _MaxHealth);

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetMaxStamina(float pAmount)
    {
        _MaxStamina = pAmount;

        _CurrentStamina = Mathf.Clamp(_CurrentStamina, 0, _MaxStamina);

        OnStaminaChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetCurrentStamina(float pAmount)
    {
        if (pAmount > _MaxStamina)
        {
            _MaxStamina = pAmount;
        }

        _CurrentStamina = Mathf.Clamp(pAmount, 0, _MaxStamina);

        OnStaminaChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SubtractStamina(float pStaminaAmount)
    {
        pStaminaAmount = UnityEngine.Random.Range((pStaminaAmount * .95f), (pStaminaAmount * 1.05f));

        if (pStaminaAmount > _CurrentStamina) pStaminaAmount = _CurrentStamina;

        _CurrentStamina -= pStaminaAmount;

        _StaminaUsedThisTick += pStaminaAmount;

        if (_CurrentStamina < 1)
        {
            if (_IsExhausted == false)
            {
                _IsExhausted = true;
                OnStaminaExhausted?.Invoke(this, EventArgs.Empty);
            }
        }

        StaminaChanged();
    }

    public float GetStaminaUsed()
    {
        float returnValue;

        returnValue =  _StaminaUsedThisTick;
        _StaminaUsedThisTick = 0;

        return returnValue;
    }
    
    public void AddStamina(float Amount)
    {
        _CurrentStamina = Mathf.Clamp((_CurrentStamina + Amount), 0, _MaxStamina);

        if (_CurrentStamina > _RequiredStaminaToRecover)
        {
            if (_IsExhausted)
            {
                _IsExhausted = false;
                OnStaminaRecovered?.Invoke(this, EventArgs.Empty);
            }
        }

        StaminaChanged();
    }

    public void StaminaChanged()
    {
        OnStaminaChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Rest(float Amount)
    {
        if (_CurrentHealth >0)
        {
            AddStamina((Amount));
        }
    }

    public float SubtractHealth(float Amount)
    {
        float _Value = 0;

        if (_IsInvincible) return _Value;

        _Value = _CurrentHealth;
        
        _CurrentHealth = Mathf.Clamp((_CurrentHealth - Amount),0,_MaxHealth);

        _Value = _Value - _CurrentHealth;

        if (_CurrentHealth == 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
        }

        HealthChanged();

        return Mathf.Abs(_Value);
    }

    public float AddHealth(float Amount)
    {
        float _Value;

        _Value = _CurrentHealth;
        
        _CurrentHealth = Mathf.Clamp((_CurrentHealth + Amount), 0, _MaxHealth);

        _Value = _CurrentHealth -  _Value;

        HealthChanged();

        return Mathf.Abs(_Value);
    }

    public void HealthChanged()
    {
        //Debug.Log("Health: " + _CurrentHealth);

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercent()
    {
        return (_CurrentHealth / _MaxHealth);
    }

    public float GetHealthAmount()
    {
        return _CurrentHealth;
    }
    
    public float GetStaminaPercent()
    {
        return (_CurrentStamina / _MaxStamina);
    }

    public float GetStaminaAmount()
    {
        return _CurrentStamina;
    }
    
}
