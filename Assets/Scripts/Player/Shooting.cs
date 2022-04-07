using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeMonkey;
using CodeMonkey.Utils;

public class Shooting : MonoBehaviour
{
    public Transform _Muzzle;
    public GameObject _BulletPrefab;
    public float _BulletForce = 160f;
    

    public void Shoot()
    {
        GameObject _Bullet = Instantiate(_BulletPrefab, _Muzzle.position, _Muzzle.rotation);
        Rigidbody2D _BulletRigidbody = _Bullet.GetComponent<Rigidbody2D>();

        _BulletRigidbody.AddForce(_Muzzle.up *_BulletForce, ForceMode2D.Impulse);
    }
}
