using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatBar : MonoBehaviour
{
    public Weapon _Weapon;
    public SpriteRenderer _Background;
    public SpriteRenderer _Bar;

    [SerializeField] private Transform _Target;
    [SerializeField] private float X_Offset = 0;
    [SerializeField] private float Y_Offset = 0;

    void Start()
    {
        _Background.color = Color.green;
        _Bar.color = Color.red;

        _Weapon.OnHeatChanged += _Weapon_OnHeatChanged;
        _Weapon.OnStartedWorking += _Weapon_OnStartedWorking;
        _Weapon.OnStoppedWorking += _Weapon_OnStoppedWorking;
    }

    private void _Weapon_OnStoppedWorking(object sender, EventArgs e)
    {
        _Background.color = Color.grey;
        _Bar.color = Color.yellow;
    }
    private void _Weapon_OnStartedWorking(object sender, EventArgs e)
    {
        _Background.color = Color.green;
        _Bar.color = Color.red;
    }
    private void _Weapon_OnHeatChanged(object sender, EventArgs e)
    {
        //Debug.Log("Heat Changed");
        transform.Find("Bar").localScale = new Vector3(_Weapon.GetHeatPercentage(), 1);
    }
}
