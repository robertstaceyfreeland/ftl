using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flames : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Bullet Trigger Fired!");

        IDamageable[] _Damageable = collision.GetComponents<IDamageable>();

        for (int i = 0; i < _Damageable.Length; i++)
        {
            if (_Damageable[i] != null)
            {
                _Damageable[i].Damage(.30f, false, true,false);
            }
        }
    }
}
