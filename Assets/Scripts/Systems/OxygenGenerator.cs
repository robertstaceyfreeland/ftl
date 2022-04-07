using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenGenerator : MonoBehaviour
{

    private float _CurrentStoredOxygen = 0;
    private float _MaxOxygenStored = 200;
    private float _MaxOxygenPerRequest = 2.0f;
    private float _CurrentOxygenGeneratedPerSecond = 8;
    private float _StaringOxygenGeneratedPerSecond = 8;
    private float _OxygenUsedDuringTick = 0;
    private float _OxygenUsedPerSecond = 0;
    private float _OxygenMadeDuringTick = 0;
    private float _OxygenMadePerSecond = 0;
    private float _NextTickTime = 0;
    
    private void FixedUpdate()
    {
        GenerateOxygen();

        if (_NextTickTime < Time.time)
        {
            _OxygenUsedPerSecond = _OxygenUsedDuringTick / 2;
            _OxygenMadePerSecond = _OxygenMadeDuringTick / 2;

            _OxygenUsedDuringTick = 0;
            _OxygenMadeDuringTick = 0;

            _NextTickTime = Time.time + 2;
        }
    }
    public float GetOxygenUsedPerSecond()
    {
        return _OxygenUsedPerSecond;
    }
    public float GetOxygenMadePerSecond()
    {
        return _OxygenMadePerSecond;
    }

    public float GetStoredOxygenAmount()
    {
        return _CurrentStoredOxygen;
    }
    public void SetOxygenGeneratedModifier(float pModifierAmount)
    {
        _CurrentOxygenGeneratedPerSecond = _StaringOxygenGeneratedPerSecond + pModifierAmount;
    }

    public float GetOxygenGeneratedAmount()
    {
        return _CurrentOxygenGeneratedPerSecond;
    }

    public void SubtractOxygen(float Amount)
    {
        Amount = UnityEngine.Random.Range(Amount * .95f, Amount * 1.05f);

        _CurrentStoredOxygen = Mathf.Clamp((_CurrentStoredOxygen - Amount), 0, _MaxOxygenStored);
        
        _OxygenUsedDuringTick += Amount;
    }

    public void AddOxygen(float Amount)
    {
        Amount = UnityEngine.Random.Range(Amount * .95f, Amount * 1.05f);

        _CurrentStoredOxygen = Mathf.Clamp((_CurrentStoredOxygen + Amount), 0, _MaxOxygenStored);

        _OxygenMadeDuringTick += Amount;
    }

    public float RequestOxygen(float Amount)
    {
        Amount = Amount * Time.fixedDeltaTime;

        Amount = Mathf.Clamp((Amount), 0, _MaxOxygenPerRequest * Time.fixedDeltaTime);
        
        if (Amount > _CurrentStoredOxygen) Amount = _CurrentStoredOxygen;
        
        SubtractOxygen(Amount);

        return Amount;
    }

    void GenerateOxygen()
    {
        AddOxygen(_CurrentOxygenGeneratedPerSecond * Time.fixedDeltaTime);
    }
}
