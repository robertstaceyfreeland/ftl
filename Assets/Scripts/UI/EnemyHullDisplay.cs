using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EnemyHullDisplay : MonoBehaviour
{
    private PlayerShipHandler _PlayerShipHandler;
    private EnemyShipHandler _EnemyShipHandler;
    private HealthSystem _HealthSystem;

    public TextMeshProUGUI _TextHullHealth;

    //public TextMeshProUGUI _TextLabelAttackRating;
    //public TextMeshProUGUI _TextLabelDefenseClass;
    //public TextMeshProUGUI _TextLabelRateOfFire;
    public Text _TextAttackRating;
    public Text _TextDefenseClass;
    public Text _TextRateOfFire;


    public Image _HullHealthBar;
    private float _DisplayRefreshRate = .075f;
    private float _NextUpdate = 0;
    private float _CurrentHealth;
    private float _CurrentStamina;
    private float _DisplayHealth;
    private float _DisplayStamina;

    [SerializeField] private Button _ButtonEnemyDetected;

    private Image _ButtonImage;



    // Start is called before the first frame update
    void Start()
    {
        if (_EnemyShipHandler)
        {
            _EnemyShipHandler = GameObject.FindObjectOfType<EnemyShipHandler>();
            _PlayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();

            _HealthSystem = _EnemyShipHandler.GetHealthSystem();
            _DisplayHealth = _HealthSystem.GetHealthAmount();
            _CurrentHealth = _DisplayHealth;
            _DisplayStamina = _HealthSystem.GetStaminaPercent();
            _CurrentStamina = _DisplayStamina;

            //Subscribe to Events
            _HealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
            _HealthSystem.OnDeath += HealthSystem_OnDeath;
            _HealthSystem.OnStaminaChanged += _HealthSystem_OnStaminaChanged;
            _HealthSystem.OnStaminaExhausted += _HealthSystem_OnStaminaExhausted;
            _HealthSystem.OnStaminaRecovered += _HealthSystem_OnStaminaRecovered;
        }

        _ButtonImage = _ButtonEnemyDetected.GetComponent<Image>();
        _ButtonImage.color = Color.red;

    }

    public void ToggleEnemyDetectedButton()
    {
        if (!_PlayerShipHandler) _PlayerShipHandler = GameObject.FindObjectOfType<PlayerShipHandler>();

        if (_ButtonImage.color == Color.grey)
        {
            //Do Nothing - No Intruders
        }

        else
        {
            if (_ButtonImage.color == Color.red)
            {
                _ButtonImage.color = Color.yellow;
                _PlayerShipHandler._EnemyShipAlertLevel = PlayerShipHandler.EnemyShipAlertLevel.Yellow;
            }
            else
            {
                _ButtonImage.color = Color.red;
                _PlayerShipHandler._EnemyShipAlertLevel = PlayerShipHandler.EnemyShipAlertLevel.Red;
            }
        }

    }

    public void StartEnemyHullDisplay()
    {
        _EnemyShipHandler = GameObject.FindObjectOfType<EnemyShipHandler>();

        _HealthSystem = _EnemyShipHandler.GetHealthSystem();
        _DisplayHealth = _HealthSystem.GetHealthAmount();
        _CurrentHealth = _DisplayHealth;
        _DisplayStamina = _HealthSystem.GetStaminaPercent();
        _CurrentStamina = _DisplayStamina;

        //Subscribe to Events
        _HealthSystem.OnHealthChanged += _HealthSystem_OnHealthChanged;
        _HealthSystem.OnDeath += HealthSystem_OnDeath;
        _HealthSystem.OnStaminaChanged += _HealthSystem_OnStaminaChanged;
        _HealthSystem.OnStaminaExhausted += _HealthSystem_OnStaminaExhausted;
        _HealthSystem.OnStaminaRecovered += _HealthSystem_OnStaminaRecovered;

        if (_ButtonImage != null)
        {
            _ButtonImage.color = Color.red;
        } 
    }

    private void Update()
    {
        
        if(Time.time > _NextUpdate)
        {
            //Stamina
            _DisplayStamina = CalculateStaminaDisplay(_CurrentStamina, _DisplayStamina);
            _HullHealthBar.rectTransform.localScale = new Vector3(1, _DisplayStamina, 1);
            
            //Health
            _DisplayHealth =  CalculateHealthDisplay(_CurrentHealth,_DisplayHealth);
            _TextHullHealth.text = _DisplayHealth.ToString();

            if(_EnemyShipHandler != null)
            { 
                _TextAttackRating.text = (20 - _EnemyShipHandler._EnemyBaseAttackRating).ToString();
                _TextDefenseClass.text = _EnemyShipHandler.GetDefenseClass().ToString();
                _TextRateOfFire.text = _EnemyShipHandler._EnemyBaseRateOfFire.ToString();
            }

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
        _CurrentStamina = _HealthSystem.GetStaminaPercent();
    }
    private void _HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        _CurrentHealth = _HealthSystem.GetHealthAmount();
    }
    private void HealthSystem_OnDeath(object sender, EventArgs e)
    {

    }
}
