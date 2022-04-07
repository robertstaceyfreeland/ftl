using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerHullDisplay : MonoBehaviour
{
    private PlayerShipHandler _PlayerShipHandler;
    //private EnemyShipHandler _EnemyShipHandler;
    private HealthSystem _ShipHealthSystem;
    private OxygenGenerator _OxygenGenerator;
    
    public TextMeshProUGUI _TextHullHealth;

    public Text _TextAttackRating;
    public Text _TextAttackModifier;
    public Text _TextDefenseClass;
    public Text _TextRateOfFire;
    public Text _TextOxygen;
    public Text _TextOxygenProduction;
    public Text _TextOxygenConsumption;
    public Text _TextEnergy;
    public Text _TextEnergyProduction;
    public Text _TextEnergyConsumption;


    public Image _HullStaminaBar;
    private float _DisplayRefreshRate = .075f;
    private float _NextUpdate = 0;
    private float _CurrentHealth;
    private float _CurrentStamina;
    private float _DisplayHealth;
    private float _DisplayStamina;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _PlayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();
        //_EnemyShipHandler = GameObject.FindObjectOfType<EnemyShipHandler>();
        _ShipHealthSystem = _PlayerShipHandler.GetHealthSystem();
        _OxygenGenerator = FindObjectOfType<OxygenGenerator>();
        _DisplayHealth = _ShipHealthSystem.GetHealthAmount();
        _CurrentHealth = _DisplayHealth;
        _DisplayStamina = _ShipHealthSystem.GetStaminaPercent();
        _CurrentStamina = _DisplayStamina;

        //Subscribe to Events
        _ShipHealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
        _ShipHealthSystem.OnDeath += HealthSystem_OnDeath;
        _ShipHealthSystem.OnStaminaChanged += _HealthSystem_OnStaminaChanged;
        _ShipHealthSystem.OnStaminaExhausted += _HealthSystem_OnStaminaExhausted;
        _ShipHealthSystem.OnStaminaRecovered += _HealthSystem_OnStaminaRecovered;
        
    }

    private void Update()
    {
        if(Time.time > _NextUpdate)
        {
            //Stamina
            _DisplayStamina = CalculateStaminaDisplay(_CurrentStamina, _DisplayStamina);
            _HullStaminaBar.rectTransform.localScale = new Vector3(1, _DisplayStamina, 1);
            
            //Health
            _DisplayHealth =  CalculateHealthDisplay(_CurrentHealth,_DisplayHealth);
            _TextHullHealth.text = _DisplayHealth.ToString();

            _TextAttackRating.text = (20 - _PlayerShipHandler.GetAttackRating()).ToString("N0");
            _TextAttackModifier.text = _PlayerShipHandler.GetAttackModifier().ToString("N0");
            _TextDefenseClass.text = _PlayerShipHandler.GetDefenseClass().ToString("N0");
            _TextRateOfFire.text = _PlayerShipHandler.GetRateOfFire().ToString("N0") + "s";
            _TextOxygen.text = _PlayerShipHandler._OxygenGenerator.GetStoredOxygenAmount().ToString("N0") + "L";
            _TextOxygenProduction.text = _PlayerShipHandler._OxygenGenerator.GetOxygenMadePerSecond().ToString("N1") + "L";
            _TextOxygenConsumption.text = _PlayerShipHandler._OxygenGenerator.GetOxygenUsedPerSecond().ToString("N1") + "L";
            _TextEnergy.text = _PlayerShipHandler._HealthSystem.GetStaminaAmount().ToString("N0");
            _TextEnergyProduction.text = (_PlayerShipHandler.GetEnergyRecoveryAmount()*10).ToString("N1");
            _TextEnergyConsumption.text = (_PlayerShipHandler.GetEnergyUsedPerSecond()*10).ToString("N1");




            _NextUpdate = Time.time + _DisplayRefreshRate;
        }
    }

    private float CalculateHealthDisplay(float pCurrent, float pHealthDisplay)
    {
        float value = 0;
        if (pCurrent == pHealthDisplay)
        {
            return pHealthDisplay;
        }

        if (pCurrent > pHealthDisplay)
        {
            value = pHealthDisplay + 1;

        }
        else
        {
            value = pHealthDisplay - 1;
        }
        return value;
    }

    private float CalculateStaminaDisplay(float pCurrent, float pStaminaDisplay)
    {
        float value = 0;
        if (pCurrent == pStaminaDisplay)
        {
            return pStaminaDisplay;
        }

        if (pCurrent > pStaminaDisplay)
        {
            value = pStaminaDisplay + .01f;

        }
        else
        {
            value = pStaminaDisplay - .01f;
        }
        return value;
    }

    //Events
    private void _HealthSystem_OnStaminaRecovered(object sender, EventArgs e)
    {

    }
    private void _HealthSystem_OnStaminaExhausted(object sender, EventArgs e)
    {

    }
    private void _HealthSystem_OnStaminaChanged(object sender, EventArgs e)
    {
        _CurrentStamina = _ShipHealthSystem.GetStaminaPercent();
    }
    private void _HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        _CurrentHealth = _ShipHealthSystem.GetHealthAmount();
    }
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {

    }
}
