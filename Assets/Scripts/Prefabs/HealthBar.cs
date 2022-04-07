using UnityEngine;
using System;

public class HealthBar : MonoBehaviour
{
    private HealthSystem _HealthSystem;
    public Enemy _Enemy;
    public SpriteRenderer _BarSprite;

    void Start()
    {
        //_Enemy = GameObject.FindObjectOfType<Enemy>();
        _HealthSystem = _Enemy.GetHealthSystem();

        
        _HealthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    
    
    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        float _HealthPercent = _HealthSystem.GetHealthPercent();
        _BarSprite.transform.localScale = new Vector3((_HealthPercent *10), 1, 1);
    }
    



}
