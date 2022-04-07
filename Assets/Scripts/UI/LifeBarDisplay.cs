using UnityEngine.UI;
using UnityEngine;
using System;

public class LifeBarDisplay : MonoBehaviour
{
    public Player _Player;
    private HealthSystem _HealthSystem;
    //public TextMeshProUGUI _HullHealthText;
    public Image _HealthBar;
    public Image _StaminaBar;

    void Start()
    {
        _Player = GameObject.FindObjectOfType<Player>();
        _HealthSystem = _Player.GetHealthSystem();

        _HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        _HealthSystem.OnStaminaExhausted += HealthSystem_OnStaminaExhausted;
        _HealthSystem.OnStaminaChanged += HealthSystem_OnStaminaChanged;
        _HealthSystem.OnStaminaRecovered += HealthSystem_OnStaminaRecovered;

        //First Call to set Bar
        HealthSystem_OnHealthChanged(this, EventArgs.Empty);
    }

    private void HealthSystem_OnStaminaRecovered(object sender, EventArgs e)
    {
        //_StaminaBar.color = Color.green;
    }
    private void HealthSystem_OnStaminaExhausted(object sender, EventArgs e)
    {
        //_StaminaBar.color = Color.gray;
    }
    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        float _HealthPercent = _HealthSystem.GetHealthPercent();
        _HealthBar.rectTransform.localScale = new Vector3(_HealthPercent,1, 1);
    }
    private void HealthSystem_OnStaminaChanged(object sender, EventArgs e)
    {
        float _StaminaPercent = _HealthSystem.GetStaminaPercent();
        _StaminaBar.rectTransform.localScale = new Vector3(_StaminaPercent, 1, 1);
    }
}
