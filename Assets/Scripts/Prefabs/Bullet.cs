using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Bullet : MonoBehaviour, IPooledObject
{
    private SpriteRenderer _SpriteRenderer;

    public string _Name;
    public float _BulletDamage;
    public float _BulletForce;
    public GameObject _HitSpecialEffectPrefab;
    public bool _IsPlayerBullet = true;
    public float _SpecialEffectLifespan;
    public float _BulletLifespan = 2f;
    
    private const float _DAMAGE_MODIFIER_MIN = .5f;
    private const float _DAMAGE_MODIFIER_MAX = 1.5f;

    private bool IsFirstTime = true;


    public void OnObjectSpawn()
    {
        Rigidbody2D _BulletRigidbody;

        _BulletRigidbody = this.GetComponent<Rigidbody2D>();
        _BulletRigidbody.AddForce(transform.up * _BulletForce, ForceMode2D.Impulse);
        
        _SpriteRenderer = this.GetComponent<SpriteRenderer>();
        _SpriteRenderer.enabled = true;

        StartCoroutine( DisableAfterDelay(this.gameObject, _BulletLifespan));

        IsFirstTime = true;
    }

    private void PlaySpecialEffect()
    {
        GameObject _HitSpecialEffect = Instantiate(_HitSpecialEffectPrefab, transform.position, Quaternion.identity);
        
        Destroy(_HitSpecialEffect, _SpecialEffectLifespan); 
    }

    private void DeactivateBullet()
    {
        _SpriteRenderer.enabled = false;
    }

    private IEnumerator DisableAfterDelay(GameObject pObject, float pValue)
    {
        yield return new WaitForSeconds(pValue);

        pObject.SetActive(false);
    }

    private void ApplyDamage(IDamageable[] pCollision)
    {
        for (int i = 0; i < pCollision.Length; i++)
        {
            if (pCollision[i] != null)
            {
                float _Damage = _BulletDamage;
                if (_Name == "Hands")
                {
                    _Damage = Random.Range((_Damage * _DAMAGE_MODIFIER_MIN), (_Damage * _DAMAGE_MODIFIER_MAX));
                    pCollision[i].Damage(_Damage, false, false, false);
                }
                else
                {
                    _Damage = Random.Range((_Damage * _DAMAGE_MODIFIER_MIN), (_Damage * _DAMAGE_MODIFIER_MAX));
                    pCollision[i].Damage(_Damage, true, false, false);
                }
                
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsFirstTime) return;
        IsFirstTime = false;
        
        if (_IsPlayerBullet)
        {
            switch (collision.gameObject.tag)
            {
                case "Player":
                    DeactivateBullet();
                    IsFirstTime = false;
                    break;
                case "Enemy":
                case "Helm":
                    PlaySpecialEffect();
                    DeactivateBullet();
                    ApplyDamage(collision.gameObject.GetComponents<IDamageable>());
                    IsFirstTime = false;
                    break;
                default:
                    PlaySpecialEffect();
                    DeactivateBullet();
                    IsFirstTime = false;
                    break;
            }
        }
        else
        {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                    DeactivateBullet();
                    IsFirstTime = false;
                    break;
                case "Player":
                case "Helm":
                    PlaySpecialEffect();
                    DeactivateBullet();
                    ApplyDamage(collision.gameObject.GetComponents<IDamageable>());
                    IsFirstTime = false;
                    break;
                default:
                    PlaySpecialEffect();
                    DeactivateBullet();
                    IsFirstTime = false;
                    break;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsFirstTime) return;
        
        if (_IsPlayerBullet)
        {
            switch (collision.tag)
            {
                case "Player":
                    break;
                case "Enemy":
                    PlaySpecialEffect();
                    DeactivateBullet();
                    ApplyDamage(collision.GetComponents<IDamageable>());
                    IsFirstTime = false;
                    break;
                case "Helm":
                    //Do Nothing
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (collision.tag)
            {
                case "Enemy":
                    break;
                case "Player":
                    PlaySpecialEffect();
                    DeactivateBullet();
                    ApplyDamage(collision.GetComponents<IDamageable>());
                    IsFirstTime = false;
                    break;
                case "Helm":
                    //Do Nothing
                    break;
                default:
                    break;
            }
        }
    }
}
